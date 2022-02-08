using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Play.Common.Identity
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        
        private readonly IConfiguration configuration;

        public ConfigureJwtBearerOptions(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            //Check if the name is Bearer
            if(name == JwtBearerDefaults.AuthenticationScheme)
            {
                //Get the configured settings of this microservice to set the authority and name
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                options.Authority = serviceSettings.Authority;
                options.Audience = serviceSettings.ServiceName;

                //Enforce standard names, aspnet core renames these which can cause issues
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters() {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            }
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(Options.DefaultName, options);
        }
    }
}