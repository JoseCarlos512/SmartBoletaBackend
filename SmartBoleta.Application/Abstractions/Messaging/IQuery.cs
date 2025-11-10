using SmartBoleta.Domain.Abstractions;
using MediatR;

namespace SmartBoleta.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
    
}
