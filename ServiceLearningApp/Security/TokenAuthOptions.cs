using Microsoft.IdentityModel.Tokens;

namespace ServiceLearningApp.Security
{
    public class TokenAuthOptions
    {
        public string Audience { get; set; } = "LearningAppAudience";
        public string Issuer { get; set; } = "LearningAppIssuer";
        public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromDays(1);
        public static string? TokenType { get; } = "Bearer";
        public SigningCredentials? SigningCredentials { get; set; }

    }
}
