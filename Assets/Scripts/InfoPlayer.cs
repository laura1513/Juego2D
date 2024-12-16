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
        // Verificamos si colisiona con un objeto que tenga el tag "Enemy" o "Wizard o WizardEnemy" o con cualquier "Boss"
        if (colision.CompareTag("Enemy") || colision.CompareTag("Wizard")  || colision.CompareTag("WizardEnemy") || colision.CompareTag("Boss") || colision.CompareTag("BossTp")) 
        {
            if (Time.time > nextDamageTime)
            {
                TakeDamage(1); // Aplicar daño
                Instantiate(particulasDMG, this.transform); // Instanciar partículas
                nextDamageTime = Time.time + cooldownTime; // Actualizar tiempo de espera para recibir daño
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
