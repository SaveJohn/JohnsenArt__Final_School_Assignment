namespace IntegrationTests.Features.ArtworkTests.Interfaces;

public interface IAuthenticationHandlerTesting
{
    Task Authenticate_WithCorrectCredentials();

    Task Authenticate_WithIncorrectCredentials();
}