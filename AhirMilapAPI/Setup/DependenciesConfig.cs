using AhirMilap.DAL.CommonService;
using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DAL.Repository;
using AhirMilap.DAL.Services;

namespace AhirMilapAPI.Setup
{
    public class DependenciesConfig
    {
        public static void ConfigureDependencies(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<JwtManager>();
            services.AddTransient<EmailHelper>(); 
            services.AddTransient<FirebaseNotificationService>(); 

            services.AddTransient<IAuthService, AuthService>(); 
            services.AddTransient<IFavoriteService, FavoriteService>(); 
            services.AddTransient<IDashboardService, DashboardService>();  
            services.AddTransient<IProfileBlockService, ProfileBlockService>();  
            services.AddTransient<IProfileHideService, ProfileHideService>();  
            services.AddTransient<IProfileReportService, ProfileReportService>();  
            services.AddTransient<IProfileLikeService, ProfileLikeService>();  
            services.AddTransient<IFirebaseService, FirebaseService>();
            services.AddTransient<IConnectionService, ConnectionService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IGooglePlayService, GooglePlayService>();

            services.AddTransient<IAuthRepository, AuthRepository>(); 
            services.AddTransient<IDashboardRepository, DashboardRepository>(); 
            services.AddTransient<IFavoriteRepository, FavoriteRepository>();  
            services.AddTransient<IProfileBlockRepository, ProfileBlockRepository>();  
            services.AddTransient<IProfileHideRepository, ProfileHideRepository>();  
            services.AddTransient<IProfileReportRepository, ProfileReportRepository>();  
            services.AddTransient<IProfileLikeRepository, ProfileLikeRepository>();  
            services.AddTransient<IFirebaseRepository, FirebaseRepository>();  
            services.AddTransient<IConnectionRepository, ConnectionRepository>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
        }
    }
}