using Microsoft.Extensions.Configuration;

namespace KooliProjekt.Application.Authentication.Jwt
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireMinutes { get; set; }

        public JwtSettings(IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");

            Key = jwtSection["Key"];
            Issuer = jwtSection["Issuer"];
            Audience = jwtSection["Audience"];
            ExpireMinutes = int.Parse(jwtSection["ExpireMinutes"] ?? "60");
        }
    }
}
