using AhirMilap.DAL.IServices;
using Microsoft.Extensions.Localization;
using System.Reflection;

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer _localizer;

    public LocalizationService(
        IStringLocalizerFactory factory)
    {
        _localizer = factory.Create(
            "Messages",
            Assembly.GetExecutingAssembly()
            .GetName().Name);
    }

    public string Get(string key)
    {
        return _localizer[key];
    }
}