using MediatR;

namespace VertexCommerce.Shared.CQRS;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
