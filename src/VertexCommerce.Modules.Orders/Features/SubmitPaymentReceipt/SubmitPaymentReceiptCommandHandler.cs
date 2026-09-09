using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Orders.Domain.Errors;
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
                OrderErrors.NotFound(command.OrderId));
        }

        if (order.CustomerId != customerId)
        {
            return Result.Failure<PaymentReceiptResponse>(
                OrderErrors.NotFoundForCustomer(command.OrderId));
        }

        if (order.ExpiresAt.HasValue && DateTime.UtcNow > order.ExpiresAt.Value)
        {
            return Result.Failure<PaymentReceiptResponse>(
                OrderErrors.PaymentExpired);
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
