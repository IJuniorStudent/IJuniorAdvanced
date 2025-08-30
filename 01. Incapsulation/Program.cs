namespace Incapsulation;

class Program
{
    private static void Main(string[] args)
    {
        int playerHealth = 100;
        int weaponDamage = 5;
        int weaponBullets = 10;
        
        Player player = new Player(playerHealth);
        player.HealthChanged += OnPlayerHealthChanged;
        
        Weapon weapon = new Weapon(weaponDamage, weaponBullets);
        Bot bot = new Bot(weapon);
        
        bot.OnSeePlayer(player);
        
        player.HealthChanged -= OnPlayerHealthChanged;
    }
    
    private static void OnPlayerHealthChanged(int oldHealth, int newHealth)
    {
        Console.WriteLine($"Player health changed {oldHealth} -> {newHealth}");
    }
}