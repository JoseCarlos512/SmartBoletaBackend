using System;

namespace SmartBoleta.Domain.Abstractions.Security;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, Guid tenantId, string email, IEnumerable<string> roles, out DateTime expiresAt);
}
