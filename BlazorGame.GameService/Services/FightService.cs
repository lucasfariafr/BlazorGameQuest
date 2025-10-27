namespace BlazorGame.GameService.Services;

/// <summary>
/// Service pour la gestion des combats.
/// </summary>
public class FightService
{
    private readonly PlayerService _playerService;
    private readonly MonstersService _monsterService;
    private readonly GameDatabaseContext _context;

    public FightService(PlayerService playerService, MonstersService monsterService, GameDatabaseContext context)
    {
        _playerService = playerService;
        _monsterService = monsterService;
        _context = context;
    }

    /// <summary>
    /// Lance un combat entre un joueur et un monstre.
    /// </summary>
    public async Task<FightResultDto> ExecuteFightAsync(int playerId, int monsterId)
    {

            var player = await _playerService.GetPlayerByIdAsync(playerId);
            if (player == null)
            {
                throw new KeyNotFoundException($"Le joueur {playerId} n'existe pas.");
            }

            var monster = await _monsterService.GetMonsterByIdAsync(monsterId);
            if (monster == null)
            {
                throw new KeyNotFoundException($"Le monstre {monsterId} n'existe pas.");
            }

            var result = SimulateFight(player, monster);
            
            await SaveFightResultAsync(player, monster);

            return result;
    }

    /// <summary>
    /// Simule le combat tour par tour.
    /// </summary>
    private FightResultDto SimulateFight(Player player, Monster monster)
    {
        var turns = new List<FightTurnDto>();
        int turnNumber = 0;

        while (player.Health > 0 && monster.Health > 0)
        {
            turnNumber++;
            
            var (damageToMonster, damageToPlayer) = CalculateDamages(player, monster);

            monster.Health = Math.Max(monster.Health - damageToMonster, 0);
            player.Health = Math.Max(player.Health - damageToPlayer, 0);

            var turn = new FightTurnDto
            {
                TurnNumber = turnNumber,
                PlayerHealth = Math.Round(player.Health, 2),
                MonsterHealth = Math.Round(monster.Health, 2),
                PlayerDamageDealt = Math.Round(damageToMonster, 2),
                MonsterDamageDealt = Math.Round(damageToPlayer, 2)
            };

            turns.Add(turn);
        }

        return new FightResultDto
        {
            Turns = turns,
            PlayerSnapshot = CreatePlayerSnapshot(player),
            MonsterSnapshot = CreateMonsterSnapshot(monster),
            Result = DetermineFightOutcome(player, monster),
            TotalTurns = turnNumber
        };
    }

    /// <summary>
    /// Calcule les dégâts infligés par chaque combattant.
    /// </summary>
    private (double damageToMonster, double damageToPlayer) CalculateDamages(Player player, Monster monster)
    {
        double damageToMonster = Math.Max(player.Damage - monster.Armor, 0);
        double damageToPlayer = Math.Max(monster.Damage - player.Armor, 0);

        return (damageToMonster, damageToPlayer);
    }

    /// <summary>
    /// Détermine le résultat du combat.
    /// </summary>
    private string DetermineFightOutcome(Player player, Monster monster)
    {
        if (player.Health <= 0 && monster.Health <= 0)
        {
            return "Égalité : les deux combattants sont morts !";
        }
        
        if (player.Health <= 0)
        {
            return $"Défaite : le joueur a été vaincu par le {monster.Type} !";
        }
        
        return $"Victoire : le {monster.Type} a été vaincu !";
    }

    /// <summary>
    /// Sauvegarde les résultats du combat en base.
    /// </summary>
    private async Task SaveFightResultAsync(Player player, Monster monster)
    {
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Crée un snapshot du joueur pour le résultat.
    /// </summary>
    private PlayerSnapshotDto CreatePlayerSnapshot(Player player)
    {
        return new PlayerSnapshotDto
        {
            CharacterId = player.CharacterId,
            Health = Math.Round(player.Health, 2),
            Strength = player.Strength,
            Armor = player.Armor,
            Damage = Math.Round(player.Damage, 2),
            HeartNumber = player.HeartNumber,
            WeaponType = player.Weapon?.Type.ToString()
        };
    }

    /// <summary>
    /// Crée un snapshot du monstre pour le résultat.
    /// </summary>
    private MonsterSnapshotDto CreateMonsterSnapshot(Monster monster)
    {
        return new MonsterSnapshotDto
        {
            CharacterId = monster.CharacterId,
            Type = monster.Type.ToString(),
            Health = Math.Round(monster.Health, 2),
            Strength = monster.Strength,
            Armor = monster.Armor,
            Damage = Math.Round(monster.Damage, 2),
            WeaponType = monster.Weapon?.Type.ToString()
        };
    }
}
