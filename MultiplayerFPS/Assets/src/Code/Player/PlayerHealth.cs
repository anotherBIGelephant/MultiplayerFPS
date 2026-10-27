using UnityEngine;
using FishNet.Object;

public class PlayerHealth : NetworkBehaviour, ITakeDamage
{
    [SerializeField] int maxHealth = 100;
    int currentHealth;

    public bool Dead {get; private set;}

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
        ServerToggleDisable(false);
    }

    [ServerRpc(RequireOwnership = false)]
    void ServerToggleDisable(bool toggle)
    {
        ObserverToggleDisable(toggle);
    }

    [ObserversRpc]
    void ObserverToggleDisable(bool toggle)
    {
        Dead = !toggle;
        GetComponent<CapsuleCollider>().enabled = toggle;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in renderers)
        {
            r.enabled = toggle;
        }
        
    }
}
