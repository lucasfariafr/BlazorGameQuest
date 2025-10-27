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
}
