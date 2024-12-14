using BankingApp.Services.Common.Response;

namespace BankingApp.Services.Authentication.Utils
{
    public static class AuthenticationServiceErrors
    {
        public static Error InvalidEmailOrPassword =
            new($"Auth.Authenticate", "Invalid email or password.");
        
        public static Error TokenGeneration =
            new("Auth.Authenticate", "Failed to generate token.");
    }
}
