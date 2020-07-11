public interface IHealth
{
    float startingHealth { get; set; }
    float currentHealth { get; set; }

    void SetupHealth(float health);
    
    void ChangeHealth(float amount);
}
