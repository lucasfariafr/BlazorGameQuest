using BlazorGame.GameService.Data;
using BlazorGame.SharedModels.Constants;
using BlazorGame.SharedModels.Enums.Utils;
using BlazorGame.SharedModels.Models.Entities;
using BlazorGame.SharedModels.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour gérer les joueurs.
/// </summary>
public class PlayerService
{
    private readonly GameDatabaseContext _context;

    /// <summary>
    /// Initialise le service avec le contexte de la base de données.
    /// </summary>
    /// <param name="context">Le contexte de la base de données du jeu.</param>
    public PlayerService(GameDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un joueur par son identifiant.
    /// </summary>
    /// <param name="playerId">Identifiant du monstre.</param>
    /// <returns>Le monstre ou null s'il n'existe pas.</returns>
    public async Task<IReadOnlyList<Player>> GetAllPlayersAsync()
    {
        return await _context.Player.ToListAsync();
    }

    /// <summary>
    /// Récupère un joueur par son identifiant.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <returns>Le joueur ou null s'il n'existe pas.</returns>
    public async Task<Player?> GetPlayerByIdAsync(int playerId)
    {
        return await _context.Player.FindAsync(playerId);
    }

    /// <summary>
    /// Récupère un joueur par l'identifiant de son utilisateur Keycloak.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur Keycloak.</param>
    /// <returns>Le joueur ou null s'il n'existe pas.</returns>
    public async Task<Player?> GetPlayerByUserIdAsync(string userId)
    {
        return await _context.Player.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    /// <summary>
    /// Crée ou récupère un joueur pour un utilisateur Keycloak.
    /// Garantit qu'un utilisateur ne peut avoir qu'un seul joueur.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur Keycloak.</param>
    /// <returns>Le joueur associé à cet utilisateur.</returns>
    public async Task<Player> GetOrCreatePlayerForUserAsync(string userId)
    {
        var existingPlayer = await GetPlayerByUserIdAsync(userId);
        if (existingPlayer != null)
        {
            return existingPlayer;
        }

        var defaultWeapon = new Weapon
        {
            WeaponId = 0,
            Type = WeaponType.Sword
        };

        var newPlayer = new Player
        {
            CharacterId = 0,
            UserId = userId,
            Health = GameConstants.MaxHealth,
            Strength = 10,
            Armor = 5,
            HeartNumber = GameConstants.MaxHearts,
            Weapon = defaultWeapon,
            Potions = new List<Potion>(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Player.Add(newPlayer);
        await _context.SaveChangesAsync();

        return newPlayer;
    }

    /// <summary>
    /// Crée un nouveau personnage pour un utilisateur (pour une nouvelle partie).
    /// Chaque partie a son propre personnage avec des stats neuves.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur Keycloak.</param>
    /// <returns>Le nouveau joueur créé.</returns>
    public async Task<Player> CreateNewPlayerForUserAsync(string userId)
    {
        var defaultWeapon = new Weapon
        {
            WeaponId = 0,
            Type = WeaponType.Sword
        };

        var newPlayer = new Player
        {
            CharacterId = 0,
            UserId = userId,
            Health = GameConstants.MaxHealth,
            Strength = 10,
            Armor = 5,
            HeartNumber = GameConstants.MaxHearts,
            Weapon = defaultWeapon,
            Potions = new List<Potion>(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Player.Add(newPlayer);
        await _context.SaveChangesAsync();

        return newPlayer;
    }

    /// <summary>
    /// Utilise une potion pour un joueur.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="potionId">Identifiant de la potion.</param>
    /// <returns>Le joueur mis à jour.</returns>
    /// <exception cref="KeyNotFoundException">Si le joueur ou la potion n'existe pas.</exception>
    public async Task<Player> UsePotionAsync(int playerId, int potionId)
    {
        var player = await GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            throw new KeyNotFoundException($"Le joueur {playerId} n'existe pas.");
        }

        var potion = player.Potions?.FirstOrDefault(p => p.PotionId == potionId);
        if (potion == null)
        {
            throw new KeyNotFoundException($"La potion {potionId} n'existe pas pour ce joueur.");
        }

        ApplyPotionEffect(player, potion);

        player.Potions?.Remove(potion);
        await _context.SaveChangesAsync();

        return player;
    }

    /// <summary>
    /// Applique l'effet d'une potion sur un joueur.
    /// </summary>
    /// <param name="player">Le joueur.</param>
    /// <param name="potion">La potion à appliquer.</param>
    private void ApplyPotionEffect(Player player, Potion potion)
    {
        switch (potion.Type)
        {
            case PotionType.Health:
                player.Health = Math.Min(player.Health + potion.Effect, GameConstants.MaxHealth);
                break;

            case PotionType.Strength:
                player.Strength = Math.Max(player.Strength + potion.Effect, 0);
                break;

            default:
                throw new InvalidOperationException($"Type de potion non géré: {potion.Type}");
        }
    }

    /// <summary>
    /// Met à jour la santé d'un joueur.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="newHealth">Nouvelle valeur de santé.</param>
    /// <returns>True si le joueur existe et a été mis à jour, sinon false.</returns>
    public async Task<bool> UpdatePlayerHealthAsync(int playerId, double newHealth)
    {
        var player = await _context.Player.FindAsync(playerId);
        if (player == null)
        {
            return false;
        }

        player.Health = Math.Clamp(newHealth, 0, GameConstants.MaxHealth);

        if (player.Health <= 0 && player.HeartNumber > 0)
        {
            player.HeartNumber--;
            if (player.HeartNumber > 0)
            {
                player.Health = GameConstants.MaxHealth;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Vérifie si un joueur est en vie.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <returns>True si le joueur est vivant, sinon false.</returns>
    public async Task<bool> IsPlayerAliveAsync(int playerId)
    {
        var player = await _context.Player
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CharacterId == playerId);

        return player != null && (player.Health > 0 || player.HeartNumber > 0);
    }

    /// <summary>
    /// Ajoute une potion à un joueur.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="potion">Potion à ajouter.</param>
    /// <returns>True si ajout réussi, sinon false.</returns>
    public async Task<bool> AddPotionAsync(int playerId, Potion potion)
    {
        var player = await GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            return false;
        }

        player.Potions ??= new List<Potion>();

        // Ajouter la potion au contexte si elle n'est pas déjà trackée
        var existingPotion = await _context.Potions.FindAsync(potion.PotionId);
        if (existingPotion == null)
        {
            _context.Potions.Add(potion);
        }

        player.Potions.Add(potion);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Équipe une arme pour un joueur.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="weaponId">Identifiant de l'arme.</param>
    /// <returns>True si l'arme est équipée, sinon false.</returns>
    public async Task<bool> EquipWeaponAsync(int playerId, int weaponId)
    {
        var player = await _context.Player.FindAsync(playerId);
        if (player == null)
        {
            return false;
        }

        var weapon = await _context.Weapons.FindAsync(weaponId);
        if (weapon == null)
        {
            return false;
        }

        player.Weapon = weapon;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Active ou désactive un joueur.
    /// </summary>
    /// <param name="playerId">Identifiant du joueur.</param>
    /// <param name="isActive">True pour activer, false pour désactiver.</param>
    /// <returns>True si succès, sinon false.</returns>
    public async Task<bool> SetPlayerActiveStatusAsync(int playerId, bool isActive)
    {
        var player = await _context.Player.FindAsync(playerId);
        if (player == null)
        {
            return false;
        }

        player.IsActive = isActive;
        await _context.SaveChangesAsync();

        return true;
    }
}
