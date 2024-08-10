using Duende.IdentityServer.Models;

namespace EichkustMusic.Users.Infrastructure.Identity
{
    public class IdentityConfiguration
    {
        static string SecretKey { get; set; } = null!;

        public IdentityConfiguration(string secretKey)
        {
            SecretKey = secretKey;   
        }

        public List<ApiScope> Scopes { get; } = new List<ApiScope>
        {
            new ApiScope("accessAsUser", "Access as user"),
            new ApiScope("accessAsAuthor", "Access as author")
        };

        public List<ApiResource> Apis { get; } = new List<ApiResource>
        {
            new ApiResource("eichkustMusic.apiGateway")
            {
                ApiSecrets =
                {
                    new Secret(SecretKey.Sha256())
                }
            }
        };

        public List<IdentityResource> IdentityResources { get; } = new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public List<Client> Clients { get; } = new List<Client>
        {
            new Client()
            {
                ClientId = "eichkustMusic.apiGateway",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes =
                {
                    "accessAsUser", "accessAsAuthor"
                },
                ClientSecrets =
                {
                    new Secret(SecretKey.Sha256())
                }
            }
        };
    }
}
