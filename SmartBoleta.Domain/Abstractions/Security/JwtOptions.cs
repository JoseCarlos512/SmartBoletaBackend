using System;

namespace SmartBoleta.Domain.Abstractions.Security;

public class JwtOptions
{
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Secret { get; set; } = ""; // para HS256; mejor usar un certificado para RS256
    public int TokenLifetimeMinutes { get; set; } = 60;
}
