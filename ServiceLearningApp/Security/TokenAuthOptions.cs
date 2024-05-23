using Microsoft.IdentityModel.Tokens;

namespace ServiceLearningApp.Security
{
    public class TokenAuthOptions
    {
        public string? Audience { get; set; }
        public string? Issuer { get; set; }
        public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromDays(1);
        public static string? TokenType { get; } = "Bearer";
        public SigningCredentials? SigningCredentials { get; set; }

    }
}
