namespace VertexCommerce.Modules.Identity.Features.Commands.ChangePassword;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
