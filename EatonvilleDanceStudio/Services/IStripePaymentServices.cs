using Stripe.Checkout;

namespace EatonvilleDanceStudio.Services;

/// <summary>
/// Defines methods for handling Stripe payment sessions, including creating and retrieving sessions.
/// </summary>
public interface IStripePaymentService
{
    Task<Session> CreateSessionAsync(SessionCreateOptions options, Stripe.RequestOptions requestOptions);
    Task<Session> GetSessionAsync(string sessionId);
}