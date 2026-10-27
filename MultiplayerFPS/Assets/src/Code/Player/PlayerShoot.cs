using UnityEngine;
using FishNet.Object;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputReader inputReader;

    [SerializeField] int damage = 10;
    [SerializeField] private float fireRate = 0.2f;
    private float timeTillShot = 0;
    [SerializeField] float range = 20f;
    [SerializeField] LayerMask hitMask;
    [SerializeField] ParticleSystem muzzleFlash, hitParticles, bloodParticles;

    PlayerHealth health;

    public override void OnStartClient()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    void Awake()
    {
        health = inputReader.GetComponent<PlayerHealth>();
    }

    public void Update()
    {
        timeTillShot -= Time.deltaTime;
        if (inputReader.FirePressed)
        {
            if(timeTillShot <= 0 && !health.Dead)
            {
                Shoot();
            }

        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, range, hitMask))
        {
            if(hit.collider.TryGetComponent<ITakeDamage>(out ITakeDamage dmgObj))
            {
                dmgObj.TakeDamage(damage);
                Instantiate(bloodParticles, hit.point, Quaternion.Euler(hit.normal));
            }
            else
            {
                Instantiate(hitParticles, hit.point, Quaternion.Euler(hit.normal));
            }
        }
        AnimateWeapon();
        timeTillShot = fireRate;
    }

    [ServerRpc]
    void AnimateWeapon()
    {
        PlayAnimation();
    }

    [ObserversRpc]
    void PlayAnimation()
    {
        animator.SetTrigger("Fire");
        muzzleFlash.Play();
    }
}
