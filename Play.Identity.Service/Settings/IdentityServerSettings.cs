using IdentityServer4.Models;

namespace Play.Identity.Service.Settings
{
    public class IdentityServerSettings
    {
        public IReadOnlyCollection<ApiScope> ApiScopes { get; set; } = Array.Empty<ApiScope>();
        public IReadOnlyCollection<Client> Clients { get; init; } 

        public IReadOnlyCollection<IdentityResource> Resources => new IdentityResource[] {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };
    }
}