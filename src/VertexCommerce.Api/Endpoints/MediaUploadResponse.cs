namespace VertexCommerce.Api.Endpoints;

internal sealed record MediaUploadResponse(
    Guid MediaFileId,
    string Path,
    string FileName,
    string ContentType,
    long Size
);
