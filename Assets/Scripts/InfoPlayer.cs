using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPlayer : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    [SerializeField] private float cooldownTime; 
    [SerializeField] private float nextDamageTime;
    [SerializeField] ParticleSystem particulasDMG;
    [SerializeField] ParticleSystem particulasMuerte;
    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    

    public void OnTriggerStay2D(Collider2D colision)
    {
        if (colision.CompareTag("Enemy"))
        {
            if (Time.time > nextDamageTime)
            { 
                TakeDamage(1);
                Instantiate(particulasDMG,this.transform);
                nextDamageTime = Time.time + cooldownTime;
            }
        }
    }

    void TakeDamage (int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Instantiate(particulasMuerte, transform.position, transform.rotation);
            Destroy(gameObject,1);
        }         
    }
}
