namespace BlazorGame.AuthenticationServices.Models;

/// <summary>
/// Représente les deux rôles d'accès pour l'api.
/// </summary>
public enum UserRole
{

    /// <summary>
    /// Administrateur du jeu.
    /// </summary>
    Admin,

    /// <summary>
    /// Joueur de la session en cours.
    /// </summary>
    Player

}
