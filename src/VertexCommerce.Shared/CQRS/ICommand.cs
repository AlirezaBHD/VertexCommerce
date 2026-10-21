using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace VertexCommerce.Shared.CQRS;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces",
    Justification = "Marker interfaces are required for CQRS type constraints and MediatR pipeline behaviors.")]
public interface ICommand : IRequest<Result>;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces",
    Justification = "Marker interfaces are required for CQRS type constraints and MediatR pipeline behaviors.")]
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;