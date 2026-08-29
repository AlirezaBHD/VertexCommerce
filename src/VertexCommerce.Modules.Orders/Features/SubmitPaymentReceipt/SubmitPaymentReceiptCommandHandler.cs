using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Shared.Services;

namespace VertexCommerce.Modules.Orders.Features.SubmitPaymentReceipt;

public sealed class SubmitPaymentReceiptCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    IMediaService mediaService)
    : ICommandHandler<SubmitPaymentReceiptCommand, PaymentReceiptResponse>
{
    public async Task<Result<PaymentReceiptResponse>> Handle(SubmitPaymentReceiptCommand command, CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(currentUser.UserId, ct);

        var order = await orderRepository.GetByIdAsync(command.OrderId, ct);

        if (order is null)
        {
            return Result.Failure<PaymentReceiptResponse>(
                Error.NotFound("Order", command.OrderId.ToString()));
        }

        if (order.CustomerId != customerId)
        {
            return Result.Failure<PaymentReceiptResponse>(
                Error.NotFound("Order for Customer", command.OrderId.ToString()));
        }

        if (order.ExpiresAt.HasValue && DateTime.UtcNow > order.ExpiresAt.Value)
        {
            return Result.Failure<PaymentReceiptResponse>(
                Error.Validation("Payment.Expired", "Payment time expired"));
        }

        var receiptImagePath = await mediaService.SaveFileAsync(fileStream: command.ReceiptFile,
            fileName: Guid.NewGuid().ToString(),
            folder: "receipts", ct);
        
        var paymentProcess = order.SubmitPaymentReceipt(receiptImagePath: receiptImagePath);

        if (paymentProcess.IsFailure)
        {
            return Result.Failure<PaymentReceiptResponse>(
                paymentProcess.Error);
        }

        var transactionReference = paymentProcess.Value;
        await unitOfWork.SaveChangesAsync(ct);

        var response = new PaymentReceiptResponse(transactionReference);
        return Result.Success(response);
    }
}
