using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Payments;

public record GetPaymentsByBookingQuery(Guid BookingId, Guid UserId, bool IsOwnerOrAdmin) : IRequest<IEnumerable<PaymentDto>>;
public record GetPaymentByIdQuery(Guid PaymentId, Guid UserId, bool IsOwnerOrAdmin) : IRequest<PaymentDto>;
public record GetPaymentHistoryQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<IEnumerable<PaymentDto>>;
public record ProcessDepositPaymentCommand(Guid UserId, ProcessPaymentRequestDto Request) : IRequest<PaymentDto>;
public record ProcessFullPaymentCommand(Guid UserId, ProcessPaymentRequestDto Request) : IRequest<PaymentDto>;
public record RefundPaymentCommand(Guid PaymentId, Guid UserId, bool IsOwnerOrAdmin) : IRequest<PaymentDto>;

public class GetPaymentsByBookingQueryHandler : IRequestHandler<GetPaymentsByBookingQuery, IEnumerable<PaymentDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByBookingQueryHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsByBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);
        if (!request.IsOwnerOrAdmin && booking.UserId != request.UserId)
            throw new ValidationException("You are not allowed to view payments for this booking.");

        var payments = await _paymentRepository.GetPaymentsByBookingIdAsync(request.BookingId, cancellationToken);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), request.PaymentId);
        if (!request.IsOwnerOrAdmin && payment.Booking?.UserId != request.UserId)
            throw new ValidationException("You are not allowed to view this payment.");

        return _mapper.Map<PaymentDto>(payment);
    }
}

public class GetPaymentHistoryQueryHandler : IRequestHandler<GetPaymentHistoryQuery, IEnumerable<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentHistoryQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetPaymentHistoryForUserAsync(request.UserId, request.PageNumber, request.PageSize, cancellationToken);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }
}

public class ProcessDepositPaymentCommandHandler : IRequestHandler<ProcessDepositPaymentCommand, PaymentDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public ProcessDepositPaymentCommandHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(ProcessDepositPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.Request.BookingId);
        if (booking.UserId != request.UserId)
            throw new ValidationException("Only the booking customer can pay the deposit.");
        if (booking.BookingStatus != BookingStatus.Accepted)
            throw new ValidationException("Deposit can only be paid after the owner accepts the booking.");
        if (booking.Payments.Any(p => p.PaymentType == PaymentType.Deposit && p.PaymentStatus == PaymentStatus.Success))
            throw new ValidationException("Deposit has already been paid.");

        var payment = CreatePayment(booking.Id, booking.DepositAmount > 0 ? booking.DepositAmount : Math.Round(booking.TotalPrice * 0.5m, 2), PaymentType.Deposit, request.Request);
        booking.BookingStatus = BookingStatus.Deposited;
        booking.UpdatedAt = DateTime.UtcNow;
        foreach (var item in booking.BookingItems)
        {
            if (item.Slot != null)
            {
                item.Slot.SlotStatus = SlotStatus.Booked;
                item.Slot.LockedUntil = null;
                item.Slot.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PaymentDto>(payment);
    }

    public static Payment CreatePayment(Guid bookingId, decimal amount, PaymentType type, ProcessPaymentRequestDto request)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Amount = amount,
            PaymentType = type,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatus.Success,
            TransactionCode = string.IsNullOrWhiteSpace(request.TransactionCode)
                ? $"{type.ToString().ToUpperInvariant()}-{Guid.NewGuid():N}"
                : request.TransactionCode.Trim(),
            PaidAt = DateTime.UtcNow
        };
    }
}

public class ProcessFullPaymentCommandHandler : IRequestHandler<ProcessFullPaymentCommand, PaymentDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ProcessFullPaymentCommandHandler(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IUserRepository userRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(ProcessFullPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.Request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.Request.BookingId);
        if (booking.UserId != request.UserId)
            throw new ValidationException("Only the booking customer can pay the remaining amount.");
        if (booking.BookingStatus != BookingStatus.Deposited)
            throw new ValidationException("Remaining amount can only be paid after deposit.");
        if (booking.Payments.Any(p => p.PaymentType == PaymentType.Final && p.PaymentStatus == PaymentStatus.Success))
            throw new ValidationException("Remaining amount has already been paid.");

        var paidDeposit = booking.Payments
            .Where(p => p.PaymentType == PaymentType.Deposit && p.PaymentStatus == PaymentStatus.Success)
            .Sum(p => p.Amount);
        var remainingAmount = booking.TotalPrice - paidDeposit;
        if (remainingAmount <= 0)
            throw new ValidationException("No remaining amount is due.");

        var payment = ProcessDepositPaymentCommandHandler.CreatePayment(booking.Id, remainingAmount, PaymentType.Final, request.Request);
        booking.BookingStatus = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;

        var user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);
        if (user != null)
        {
            user.LoyaltyPoints += (int)Math.Floor(booking.TotalPrice / 10000m);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PaymentDto>(payment);
    }
}

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public RefundPaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), request.PaymentId);
        if (!request.IsOwnerOrAdmin && payment.Booking?.UserId != request.UserId)
            throw new ValidationException("You are not allowed to refund this payment.");
        if (payment.PaymentStatus != PaymentStatus.Success)
            throw new ValidationException("Only successful payments can be refunded.");

        payment.PaymentStatus = PaymentStatus.Refunded;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PaymentDto>(payment);
    }
}
