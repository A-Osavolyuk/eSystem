namespace eSecurity.Client.Common.JS.Localization;

public interface ILocalizationManager
{
    public ValueTask<string> GetLocaleAsync();
    public ValueTask<string> GetTimeZoneAsync();
}