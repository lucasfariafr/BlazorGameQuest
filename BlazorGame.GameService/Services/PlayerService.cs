namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour la gestion du joueur.
/// </summary>
public class PlayerService
{
    private readonly GameDatabaseContext _context;

    public PlayerService(GameDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un joueur par son ID avec toutes ses relations.
    /// </summary>
    public async Task<Player?> GetPlayerByIdAsync(int playerId)
    {
        return await _context.Player
            .Include(p => p.Weapon)
            .Include(p => p.Potions)
            .FirstOrDefaultAsync(p => p.CharacterId == playerId);
    }

    /// <summary>
    /// Utilise une potion sur le joueur.
    /// </summary>
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
    /// Applique l'effet d'une potion au joueur.
    /// </summary>
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
    /// Met à jour la santé du joueur.
    /// </summary>
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
    /// Vérifie si le joueur existe.
    /// </summary>
    public async Task<bool> PlayerExistsAsync(int playerId)
    {
        return await _context.Player.AnyAsync(p => p.CharacterId == playerId);
    }

    /// <summary>
    /// Vérifie si le joueur est vivant (a des PV ou des cœurs).
    /// </summary>
    public async Task<bool> IsPlayerAliveAsync(int playerId)
    {
        var player = await _context.Player
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CharacterId == playerId);

        return player != null && (player.Health > 0 || player.HeartNumber > 0);
    }

    /// <summary>
    /// Ajoute une potion à l'inventaire du joueur.
    /// </summary>
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
    /// Équipe une arme au joueur.
    /// </summary>
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
