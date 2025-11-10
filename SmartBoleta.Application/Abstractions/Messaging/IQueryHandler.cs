using SmartBoleta.Domain.Abstractions;
using MediatR;

namespace SmartBoleta.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery,TResponse> 
: IRequestHandler<TQuery,Result<TResponse>>
where TQuery : IQuery<TResponse>
{
    
}