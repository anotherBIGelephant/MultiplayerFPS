using UnityEngine;
using FishNet.Object;

public class PlayerHealth : NetworkBehaviour, ITakeDamage
{
    [SerializeField] int maxHealth = 100;
    int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public override void OnStartClient()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    public void TakeDamage(int amount)
    {
        ServerDamage(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    void ServerDamage(int amount)
    {
        Debug.Log(currentHealth);
        currentHealth -= amount;
        if(currentHealth <= 0)
            Die();
        
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    void Die()
    {
        
    }
}
