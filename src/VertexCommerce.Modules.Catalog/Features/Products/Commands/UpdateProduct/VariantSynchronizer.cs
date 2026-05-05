// using VertexCommerce.Modules.Catalog.Domain.Products;
// using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
//
// namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;
//
// internal sealed class VariantSynchronizer(Product product, IProductRepository repository)
// {
//     public async Task SyncAsync(List<UpdateVariantDto> variants, CancellationToken ct)
//     {
//         RemoveDeletedVariants(variants);
//         foreach (var dto in variants)
//         {
//             if (dto.Id.HasValue)
//                 UpdateExistingVariant(dto);
//             else
//                 await AddNewVariantAsync(dto, ct);
//         }
//     }
//
//     private void RemoveDeletedVariants(List<UpdateVariantDto> variants)
//     {
//         var existingIds = variants
//             .Where(v => v.Id.HasValue)
//             .Select(v => v.Id!.Value)
//             .ToHashSet();
//
//         var toRemove = product.Variants
//             .Where(v => !existingIds.Contains(v.Id))
//             .Select(v => v.Id)
//             .ToList();
//
//         foreach (var id in toRemove)
//             product.RemoveVariant(id);
//     }
//
//     private void UpdateExistingVariant(UpdateVariantDto dto)
//     {
//         var variant = product.Variants.FirstOrDefault(v => v.Id == dto.Id!.Value);
//         if (variant is null) return;
//
//         var sku = Sku.Create(dto.Sku ?? variant.Sku.Value);
//         var options = dto.Options.Select(o => VariantOption.Create(o.Name, o.Value)).ToList();
//         var price = Money.Create(dto.Price, dto.Currency ?? "USD");
//         var medias = dto.Medias.Select(m => ProductMedia.Create(m.Path, MediaType.Image, m.Order)).ToList();
//
//         variant.Update(sku, options, dto.StockQuantity, dto.Order, price);
//         variant.ReplaceMedia(medias);
//     }
//
//     private async Task AddNewVariantAsync(UpdateVariantDto dto, CancellationToken ct)
//     {
//         var options = dto.Options.Select(o => VariantOption.Create(o.Name, o.Value)).ToList();
//         var price = Money.Create(dto.Price, dto.Currency ?? "USD");
//         var sku = Sku.Generate();
//
//         var variant = ProductVariant.Create(
//             product.Id,
//             sku,
//             options,
//             dto.StockQuantity,
//             dto.Order,
//             price);
//
//         var medias = dto.Medias.Select(m => ProductMedia.Create(m.Path, MediaType.Image, m.Order)).ToList();
//         variant.SetMedia(medias);
//
//         product.AddVariant(variant);
//         await repository.AddVariantAsync(variant, ct);
//     }
// }
