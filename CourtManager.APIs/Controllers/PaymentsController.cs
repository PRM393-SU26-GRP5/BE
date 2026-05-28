using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Payments;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    private readonly IPaymentRepository _paymentRepository;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger, IPaymentRepository paymentRepository)
    {
        _mediator = mediator;
        _logger = logger;
        _paymentRepository = paymentRepository;
    }

    /// <summary>
    /// Gets all payments for a specific booking.
    /// </summary>
    /// <param name="bookingId">The booking ID</param>
    /// <returns>List of payments</returns>
    [NonAction]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByBooking(Guid bookingId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payments for booking {BookingId}", bookingId);
        var result = await _mediator.Send(new GetPaymentsByBookingQuery(bookingId, GetCurrentUserId(), IsOwnerOrAdmin()), cancellationToken);
        return Ok(result);
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
        var result = await _mediator.Send(new GetPaymentByIdQuery(id, GetCurrentUserId(), IsOwnerOrAdmin()), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets payment history for the current user.
    /// </summary>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <returns>Paginated payment history</returns>
    [NonAction]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Fetching payment history for user {UserId}", userId);
        var result = await _mediator.Send(new GetPaymentHistoryQuery(GetCurrentUserId(), pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Processes a payment for a booking.
    /// </summary>
    /// <param name="payment">The payment creation data</param>
    /// <returns>Created payment</returns>
    [NonAction]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaymentDto>> ProcessPayment([FromBody] PaymentDto payment, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Processing payment for booking {BookingId} by user {UserId}", payment.BookingId, userId);
        var request = new ProcessPaymentRequestDto
        {
            BookingId = payment.BookingId,
            PaymentMethod = payment.PaymentMethod,
            TransactionCode = payment.TransactionCode
        };
        var result = string.Equals(payment.PaymentType, "Final", StringComparison.OrdinalIgnoreCase)
            ? await _mediator.Send(new ProcessFullPaymentCommand(GetCurrentUserId(), request), cancellationToken)
            : await _mediator.Send(new ProcessDepositPaymentCommand(GetCurrentUserId(), request), cancellationToken);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
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
        var result = await _mediator.Send(new ProcessFullPaymentCommand(GetCurrentUserId(), request), cancellationToken);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
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
        if (string.IsNullOrWhiteSpace(callback.TransactionCode))
        {
            return BadRequest(new { success = false, message = "transactionCode is required" });
        }

        var payment = await _paymentRepository.GetByTransactionIdAsync(callback.TransactionCode, cancellationToken);
        if (payment == null)
        {
            return NotFound(new { success = false, message = "Payment transaction was not found" });
        }

        payment.PaymentStatus = callback.Success ? PaymentStatus.Success : PaymentStatus.Failed;
        payment.PaidAt = callback.Success ? DateTime.UtcNow : payment.PaidAt;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Gateway} callback updated payment {PaymentId} to {Status}", gateway, payment.Id, payment.PaymentStatus);
        return Ok(new { success = true, paymentId = payment.Id, paymentStatus = payment.PaymentStatus.ToString() });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }

    private bool IsOwnerOrAdmin() => User.IsInRole("Manager") || User.IsInRole("Admin");
}

public class PaymentGatewayCallbackDto
{
    public string TransactionCode { get; set; } = string.Empty;
    public bool Success { get; set; }
}
