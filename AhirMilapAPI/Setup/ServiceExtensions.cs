using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

namespace AhirMilapAPI.Setup
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services, string origin = null)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(_ => true)
                            .AllowCredentials();
                    });
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder =>
            //        {
            //            builder.AllowAnyMethod()
            //                .AllowAnyHeader()
            //                .SetIsOriginAllowed(_ => true)
            //                .AllowCredentials();

            //            if (string.IsNullOrEmpty(origin))
            //            {
            //                builder.AllowAnyOrigin();
            //            }
            //            else
            //            {
            //                builder.WithOrigins("https://transpotereapp.com", "http://transpotereapp.com", "http://transporters033-001-site2.anytempurl.com", "https://transporters033-001-site2.anytempurl.com", "https://transporters033-001-site3.anytempurl.com", origin); //http://localhost fix for the local iss deploy
            //            }

            //        });
            //});
        }

        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Insert(0, new CentralizedPrefixConvention(routeAttribute));
        }
    }
}
