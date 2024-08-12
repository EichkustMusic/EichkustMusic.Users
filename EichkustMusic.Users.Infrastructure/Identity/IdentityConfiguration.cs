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

        public List<ApiScope> Scopes { get; } =
        [
            new ApiScope("access_as_user", "Access as user"),
            new ApiScope("access_as_author", "Access as author")
        ];

        public List<ApiResource> Apis { get; } =
        [
            new ApiResource("eichkustMusic.apiGateway")
            {
                ApiSecrets =
                {
                    new Secret(SecretKey.Sha256())
                }
            }
        ];

        public List<IdentityResource> IdentityResources { get; } =
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        ];

        public List<Client> Clients { get; } =
        [
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
        ];
    }
}
