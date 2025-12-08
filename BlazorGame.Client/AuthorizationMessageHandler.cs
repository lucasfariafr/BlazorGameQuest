using System.Net.Http.Headers;

namespace BlazorGame.Client;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthorizationMessageHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Récupérer le token
        var token = await _tokenService.GetTokenAsync();

        // Ajouter le token au header Authorization si disponible
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
