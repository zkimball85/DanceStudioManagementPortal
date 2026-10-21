namespace EatonvilleDanceStudio.Settings;

/// <summary>
/// Represents the configuration settings required to integrate with the Stripe payment platform.
/// These settings include the API keys needed for authentication and communication with Stripe services.
/// 
/// Typically, the settings values are loaded from a configuration provider, such as appsettings.json,
/// using dependency injection and the options pattern.
/// </summary>

public class StripeSettings
{
    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
}