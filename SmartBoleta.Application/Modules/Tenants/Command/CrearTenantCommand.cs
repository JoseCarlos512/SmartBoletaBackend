using FluentValidation;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Tenants.Command;

public record CrearTenantCommand(
    string NombreComercial,
    string Ruc,
    string LogoUrl,
    string ColorPrimario,
    string FaviconUrl
) : ICommand<Guid>;

internal sealed class CrearTenantCommandHandler : ICommandHandler<CrearTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepository;

    public CrearTenantCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Result<Guid>> Handle(CrearTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = Tenant.Create(
            request.NombreComercial,
            request.Ruc,
            request.FaviconUrl,
            request.LogoUrl,
            request.ColorPrimario
        );

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        return Result.Success(tenant.Id);
    }
}

internal sealed class CrearTenantCommandValidator : AbstractValidator<CrearTenantCommand>
{
    public CrearTenantCommandValidator()
    {
        RuleFor(x => x.NombreComercial).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Ruc).NotEmpty().MaximumLength(20);
        RuleFor(x => x.LogoUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ColorPrimario).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FaviconUrl).NotEmpty().MaximumLength(500);
    }
}
