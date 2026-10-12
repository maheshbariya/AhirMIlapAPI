using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using AhirMilapAPI.Data;
using AhirMilapAPI.Setup;
using AhirMilap.DAL.CommonService;
using AhirMilapAPI.CommonService;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using AhirMilap.DAL.IServices;

namespace AhirMilapAPI
{
    public class Startup
    {
        private IWebHostEnvironment _hostEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        protected void ConfigureDependencies(IServiceCollection services)
        {
            DependenciesConfig.ConfigureDependencies(services);
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("localDb")));

            services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();

            // Other service configurations...

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            services.ConfigureAuth(Configuration);
            ConfigureDependencies(services);
            services.ConfigureCors("http://localhost:4200"); // Add your Angular app's address here 
            services.ConfigureSwagger();
            services.AddAuthorization();
            services.AddScoped<GenerateDomainURL>();

            services.AddSignalR();
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services
                 .AddControllers(opt =>
                 {
                     opt.UseCentralRoutePrefix(new RouteAttribute("api"));
                     opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                 })
                 .AddNewtonsoftJson(options =>
                 {
                     options.SerializerSettings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
                 });
            services.AddScoped<ILocalizationService, LocalizationService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Other app configurations...
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(_hostEnvironment.ContentRootPath)
            });
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("gu")
            };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseConfiguredSwagger();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
