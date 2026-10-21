namespace VertexCommerce.Shared.Contracts.Catalog;

public record StockDeductionRequest(Guid VariantId, int Quantity);