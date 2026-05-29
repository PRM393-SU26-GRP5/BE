using CourtManager.APIs.Configuration;
using CourtManager.APIs.Models.SePay;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Application.Features.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing payments.
/// Provides payment processing and transaction history endpoints.
/// </summary>
[ApiController]
[Route("api/v1/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;
    private readonly SePaySettings _sePaySettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentsController(
        IMediator mediator,
        ILogger<PaymentsController> logger,
        IOptions<SePaySettings> sePaySettings,
        IHttpClientFactory httpClientFactory)
    {
        _mediator = mediator;
        _logger = logger;
        _sePaySettings = sePaySettings.Value;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Gets a specific payment by ID.
    /// </summary>
    /// <param name="id">The payment ID</param>
    /// <returns>Payment details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payment {PaymentId}", id);
        var payment = await _mediator.Send(new GetPaymentByIdQuery(id, GetCurrentUserId(), IsOwnerOrAdmin()), cancellationToken);
        return Ok(payment);
    }

    /// <summary>
    /// Generates a SePay QR payment URL for a specific payment.
    /// </summary>
    /// <param name="paymentId">The payment ID</param>
    /// <returns>SePay QR URL and payment details</returns>
    [HttpGet("{paymentId:guid}/sepay/qr")]
    public async Task<ActionResult<SePayQrResponseDto>> GetSePayQr(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _mediator.Send(new GetPaymentByIdQuery(paymentId, GetCurrentUserId(), IsOwnerOrAdmin()), cancellationToken);

        var bankId = _sePaySettings.BankId;
        var accountNo = _sePaySettings.AccountNo;
        var accountName = _sePaySettings.AccountName;
        var transactionCode = NormalizeLegacySePayTransactionCode(payment.TransactionCode);
        var description = $"CM{transactionCode}";
        var qrUrl = $"https://qr.sepay.vn/img?acc={Uri.EscapeDataString(accountNo)}&bank={Uri.EscapeDataString(bankId)}&amount={payment.Amount:0}&des={Uri.EscapeDataString(description)}";

        return Ok(new SePayQrResponseDto
        {
            QrUrl = qrUrl,
            Amount = payment.Amount,
            Description = description,
            PaymentId = payment.Id,
            Status = payment.PaymentStatus,
            BankInfo = new BankInfoDto
            {
                BankId = bankId,
                AccountNo = accountNo,
                AccountName = accountName
            }
        });
    }

    /// <summary>
    /// Returns SePay checkout form data for a specific payment.
    /// The frontend should auto-submit a hidden HTML form with the returned fields
    /// to the checkoutUrl (POST), which redirects the user to SePay's payment page.
    /// </summary>
    /// <param name="paymentId">The payment ID</param>
    /// <returns>JSON containing form action URL and hidden fields for auto-submit</returns>
    [HttpGet("{paymentId:guid}/sepay/checkout")]
    public async Task<ActionResult<SePayCheckoutResponseDto>> GetSePayCheckout(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _mediator.Send(new GetPaymentByIdQuery(paymentId, GetCurrentUserId(), IsOwnerOrAdmin()), cancellationToken);

        if (payment == null)
        {
            return NotFound(new { message = $"Payment with ID {paymentId} not found." });
        }

        if (string.IsNullOrWhiteSpace(_sePaySettings.EffectiveMerchantId) ||
            string.IsNullOrWhiteSpace(_sePaySettings.EffectiveSecretKey))
        {
            return BadRequest(new { message = "SePay merchant_id/secret_key is not configured." });
        }

        var fields = BuildSePayCheckoutFields(payment);
        fields["signature"] = SignSePayCheckoutFields(fields, _sePaySettings.EffectiveSecretKey);

        // SePay /v1/checkout/init is a browser form POST endpoint (not a REST API).
        // Return the form action URL + fields so frontend can auto-submit a hidden form.
        var checkoutUrl = $"{_sePaySettings.CheckoutBaseUrl}/v1/checkout/init";

        _logger.LogInformation("SePay checkout prepared for payment {PaymentId}, URL={Url}, Fields={@Fields}",
            paymentId, checkoutUrl, fields.Keys);

        return Ok(new SePayCheckoutResponseDto
        {
            CheckoutUrl = checkoutUrl,
            FormFields = fields,
            Success = true
        });
    }

    /// <summary>
    /// Browser-friendly checkout redirect. Opens this URL in a browser and it auto-submits
    /// a hidden form to SePay, redirecting the user to the payment page.
    /// </summary>
    [HttpGet("{paymentId:guid}/sepay/redirect")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/html")]
    public async Task<IActionResult> SePayCheckoutRedirect(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _mediator.Send(new GetPaymentByIdQuery(paymentId, GetCurrentUserId(), IsOwnerOrAdmin()), cancellationToken);

        if (payment == null)
            return NotFound($"Payment {paymentId} not found.");

        if (string.IsNullOrWhiteSpace(_sePaySettings.EffectiveMerchantId))
            return BadRequest("SePay merchant is not configured.");

        var fields = BuildSePayCheckoutFields(payment);
        fields["signature"] = SignSePayCheckoutFields(fields, _sePaySettings.EffectiveSecretKey);

        var checkoutUrl = $"{_sePaySettings.CheckoutBaseUrl}/v1/checkout/init";

        // Build a self-submitting HTML form
        var hiddenFields = string.Join("\n            ",
            fields.Select(f =>
                $"<input type=\"hidden\" name=\"{WebUtility.HtmlEncode(f.Key)}\" value=\"{WebUtility.HtmlEncode(f.Value)}\" />"));

        var html = $@"<!DOCTYPE html>
<html>
<head><title>Redirecting to SePay...</title></head>
<body>
    <p>Đang chuyển hướng đến trang thanh toán SePay...</p>
    <form id=""sepay-form"" action=""{WebUtility.HtmlEncode(checkoutUrl)}"" method=""POST"">
        {hiddenFields}
    </form>
    <script>document.getElementById('sepay-form').submit();</script>
</body>
</html>";

        return Content(html, "text/html");
    }

    [HttpGet("payment/success")]
    [AllowAnonymous]
    public IActionResult SePayCheckoutSuccess()
    {
        var url = !string.IsNullOrWhiteSpace(_sePaySettings.SuccessUrl) 
            ? _sePaySettings.SuccessUrl 
            : "/";
        return Redirect(url);
    }

    [HttpGet("payment/error")]
    [AllowAnonymous]
    public IActionResult SePayCheckoutError()
    {
        var url = !string.IsNullOrWhiteSpace(_sePaySettings.ErrorUrl) 
            ? _sePaySettings.ErrorUrl 
            : "/";
        return Redirect(url);
    }

    [HttpGet("payment/cancel")]
    [AllowAnonymous]
    public IActionResult SePayCheckoutCancel()
    {
        var url = !string.IsNullOrWhiteSpace(_sePaySettings.CancelUrl) 
            ? _sePaySettings.CancelUrl 
            : "/";
        return Redirect(url);
    }

    [HttpPost("sepay/callback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SePayCallback([FromBody] SePayWebhookDto callback, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(">>> SEPAY CALLBACK RECEIVED: Content='{Content}', Amount={Amount}", callback.Content, callback.TransferAmount);

        if (!TryValidateSePayApiKey())
        {
            _logger.LogError("!!! SEPAY ERROR: Unauthorized. API Key mismatch.");
            return Unauthorized(new { success = false, message = "Invalid SePay API key" });
        }

        if (!IsExpectedSePayAccount(callback.AccountNumber))
        {
            _logger.LogWarning("Ignored SePay webhook for unexpected account {AccountNumber}", callback.AccountNumber);
            return Ok(new PaymentGatewayCallbackResultDto
            {
                StatusCode = StatusCodes.Status200OK,
                Success = false,
                Message = "Ignored unexpected SePay account."
            });
        }

        var result = await _mediator.Send(
            new ProcessSePayWebhookCommand(
                callback.Id,
                callback.Gateway,
                callback.AccountNumber,
                callback.TransferType,
                callback.Content,
                callback.TransferAmount,
                callback.ReferenceCode),
            cancellationToken);

        _logger.LogInformation("SEPAY callback processed with status {StatusCode}: {Message}", result.StatusCode, result.Message);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("deposit")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PaymentDto>> PayDeposit([FromBody] ProcessPaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ProcessDepositPaymentCommand(GetCurrentUserId(), request), cancellationToken);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
    }

    [HttpPost("full")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PaymentDto>> PayFull([FromBody] ProcessPaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(new ProcessFullPaymentCommand(GetCurrentUserId(), request), cancellationToken);
            return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                details = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Refunds a payment.
    /// </summary>
    /// <param name="id">The payment ID</param>
    /// <returns>Refund status</returns>
    [NonAction]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaymentDto>> RefundPayment(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refunding payment {PaymentId}", id);
        var result = await _mediator.Send(new RefundPaymentCommand(id, GetCurrentUserId(), IsOwnerOrAdmin()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("momo/callback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MomoCallback([FromBody] PaymentGatewayCallbackDto callback, CancellationToken cancellationToken = default)
    {
        return await HandleGatewayCallback(callback, "MoMo", cancellationToken);
    }

    [HttpPost("vnpay/callback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> VNPayCallback([FromBody] PaymentGatewayCallbackDto callback, CancellationToken cancellationToken = default)
    {
        return await HandleGatewayCallback(callback, "VNPay", cancellationToken);
    }

    private async Task<IActionResult> HandleGatewayCallback(PaymentGatewayCallbackDto callback, string gateway, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ProcessPaymentGatewayCallbackCommand(callback, gateway), cancellationToken);
        _logger.LogInformation("{Gateway} callback processed with status {StatusCode}: {Message}", gateway, result.StatusCode, result.Message);
        return StatusCode(result.StatusCode, result);
    }

    private bool TryValidateSePayApiKey()
    {
        var expectedApiKey = _sePaySettings.EffectiveWebhookApiKey;

        if (string.IsNullOrWhiteSpace(expectedApiKey))
        {
            _logger.LogError("SePay webhook API key is not configured.");
            return false;
        }

        var candidates = new[]
        {
            Request.Headers["X-API-Key"].ToString(),
            Request.Headers["X-SePay-Api-Key"].ToString(),
            Request.Headers["Api-Key"].ToString(),
            ExtractApiKeyFromAuthorization(Request.Headers.Authorization.ToString())
        };

        return candidates.Any(candidate =>
            !string.IsNullOrWhiteSpace(candidate) &&
            FixedTimeEquals(candidate.Trim(), expectedApiKey));
    }

    private static string ExtractApiKeyFromAuthorization(string authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return string.Empty;
        }

        var parts = authorizationHeader.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 &&
               (parts[0].Equals("ApiKey", StringComparison.OrdinalIgnoreCase) ||
                parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            ? parts[1]
            : authorizationHeader.Trim();
    }

    private bool IsExpectedSePayAccount(string? accountNumber)
    {
        return string.IsNullOrWhiteSpace(_sePaySettings.AccountNo) ||
               string.IsNullOrWhiteSpace(accountNumber) ||
               accountNumber.Equals(_sePaySettings.AccountNo, StringComparison.OrdinalIgnoreCase);
    }

    private static bool FixedTimeEquals(string actual, string expected)
    {
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return actualBytes.Length == expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }

    private Dictionary<string, string> BuildSePayCheckoutFields(PaymentDto payment)
    {
        var transactionCode = NormalizeLegacySePayTransactionCode(payment.TransactionCode);
        var description = $"CM{transactionCode}";
        // Field order MUST match SePay's required signing order exactly
        return new Dictionary<string, string>
        {
            ["merchant"] = _sePaySettings.EffectiveMerchantId,
            ["operation"] = "PURCHASE",
            ["payment_method"] = "BANK_TRANSFER",
            ["order_amount"] = payment.Amount.ToString("0"),
            ["currency"] = "VND",
            ["order_invoice_number"] = transactionCode,
            ["order_description"] = description,
            ["customer_id"] = payment.BookingId.ToString("N"),
            ["success_url"] = GetConfiguredOrFallbackUrl(_sePaySettings.SuccessUrl, payment.Id, "success"),
            ["error_url"] = GetConfiguredOrFallbackUrl(_sePaySettings.ErrorUrl, payment.Id, "error"),
            ["cancel_url"] = GetConfiguredOrFallbackUrl(_sePaySettings.CancelUrl, payment.Id, "cancel")
        };
    }

    private static string NormalizeLegacySePayTransactionCode(string transactionCode)
    {
        const string legacySeparator = " | SePayRef:";
        var separatorIndex = transactionCode.IndexOf(legacySeparator, StringComparison.OrdinalIgnoreCase);

        return separatorIndex >= 0
            ? transactionCode[..separatorIndex].Trim()
            : transactionCode.Trim();
    }

    private string GetConfiguredOrFallbackUrl(string configuredUrl, Guid paymentId, string status)
    {
        if (!string.IsNullOrWhiteSpace(configuredUrl))
        {
            return configuredUrl;
        }

        return $"{Request.Scheme}://{Request.Host}/api/v1/payments/{paymentId}?checkoutStatus={status}";
    }

    private static string SignSePayCheckoutFields(IReadOnlyDictionary<string, string> fields, string secretKey)
    {
        // SePay requires EXACT field order for signature — do NOT reorder!
        string[] signedFields =
        [
            "merchant",
            "operation",
            "payment_method",
            "order_amount",
            "currency",
            "order_invoice_number",
            "order_description",
            "customer_id",
            "success_url",
            "error_url",
            "cancel_url"
        ];

        var signedString = string.Join(
            ",",
            signedFields
                .Where(fields.ContainsKey)
                .Select(field => $"{field}={fields[field]}"));

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(signedString)));
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }

    private bool IsOwnerOrAdmin() => User.IsInRole("Owner") || User.IsInRole("Admin");
}
