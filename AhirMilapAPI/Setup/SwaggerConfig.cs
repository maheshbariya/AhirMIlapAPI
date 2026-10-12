using Microsoft.OpenApi.Models;

namespace AhirMilapAPI.Setup
{
    public static class SwaggerConfig
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web Api", Version = "v1" });
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    //Type = SecuritySchemeType.ApiKey,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Name = "Bearer",
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        In = ParameterLocation.Header
                    }, new List<string>() }
                });

            });
        }


        public static void UseConfiguredSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Api");
               // c.DefaultModelsExpandDepth(-1);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });
        }
    }
}
