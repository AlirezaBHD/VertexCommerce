using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.SendOpt;

public sealed record SendOptCommand(string PhoneNumber) 
    : ICommand<RegistrationTokenResponse>;