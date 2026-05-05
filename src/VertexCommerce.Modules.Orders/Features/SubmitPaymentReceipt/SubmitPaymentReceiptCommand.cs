using Microsoft.AspNetCore.Http;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.SubmitPaymentReceipt;

public sealed record SubmitPaymentReceiptCommand(Guid OrderId, Stream ReceiptFile) : ICommand<PaymentReceiptResponse>;

public sealed record PaymentReceiptResponse(
    string OrderId
);
