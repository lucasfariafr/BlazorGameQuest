using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.GameService.Helpers;

/// <summary>
/// Classe helper pour récupérer les informations de l'utilisateur connecté.
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// Récupère l'ID de l'utilisateur connecté à partir du ClaimsPrincipal.
    /// </summary>
    /// <param name="user">Le ClaimsPrincipal de l'utilisateur.</param>
    /// <returns>L'ID de l'utilisateur ou null si non trouvé.</returns>
    public static string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? user.FindFirst("sub")?.Value;
    }
}
