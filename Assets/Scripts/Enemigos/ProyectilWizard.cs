using UnityEngine;

public class Proyectil : MonoBehaviour
{
    // Tiempo de vida del proyectil opcional
    public float tiempoDeVida = 5f;

    private void Start()
    {
        // Destruir después de un tiempo si no impacta con nada
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Impacto con: {collision.name} (Tag: {collision.tag})");

        if (collision.CompareTag("Wizard") || collision.CompareTag("WizardEnemy"))
        {
            // No destruimos si colisionamos con magos
            return;
        }

        // Destruir el proyectil al impactar con otro objeto
        Destroy(gameObject, 0.1f);
    }


}

