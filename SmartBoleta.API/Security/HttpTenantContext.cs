using System.Security.Claims;

namespace SmartBoleta.API.Security;

public class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetTenantId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        var claimValue = context.User.FindFirstValue("tenantId");
        if (!string.IsNullOrWhiteSpace(claimValue) && Guid.TryParse(claimValue, out var tenantIdFromClaim))
        {
            return tenantIdFromClaim;
        }

        var tenantHeader = context.Request.Headers["TenantID"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(tenantHeader) && Guid.TryParse(tenantHeader, out var tenantIdFromHeader))
        {
            return tenantIdFromHeader;
        }

        return null;
    }
}
