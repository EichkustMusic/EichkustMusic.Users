using Duende.IdentityServer.Models;

namespace EichkustMusic.Users.Infrastructure.Identity
{
    public class IdentityConfiguration
    {
        private readonly string _secretKey;

        public IdentityConfiguration(string secretKey)
        {
            _secretKey = secretKey;   
        }

        public List<ApiScope> GetScopes() =>
        [
            new ApiScope("api_gateway.access_as_user"),
            new ApiScope("api_gateway.access_as_author")
        ];

        public List<ApiResource> GetApis() =>
        [
            new ApiResource("api_gateway")
            {
                ApiSecrets =
                {
                    new Secret(_secretKey.Sha256())
                },
                Scopes =
                {
                    "api_gateway.access_as_user", "api_gateway.access_as_author"
                }
            }
        ];

        public List<IdentityResource> GetIdentityResources() =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        ];

        public List<Client> GetClients() =>
        [
            new Client()
            {
                ClientId = "api_gateway",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes =
                {
                    "api_gateway.access_as_user", "apiGateway.access_as_author"
                },
                ClientSecrets =
                {
                    new Secret(_secretKey.Sha256())
                }
            }
        ];
    }
}
