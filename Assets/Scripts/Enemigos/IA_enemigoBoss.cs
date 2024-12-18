using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class IA_enemigoBoss : MonoBehaviour
{
    public enum EnemyState { Patrullando, Atacando, Escapando, Muerto }
    private EnemyState currentState;

    [Header("Variables del Enemigo")]
    [SerializeField] private float distanciaVision = 10f;
    [SerializeField] private float distanciaEscape = 3f;
    [SerializeField] private float distanciaSegura = 7f;
    [SerializeField] private UnityEvent eventoAtaque;
    [SerializeField] private Transform player;

    [Header("Movimiento de Patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float velocidadPatrulla = 3f;

    [Header("Ataque con Proyectil")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float velocidadProyectil = 10f;
    [SerializeField] private float intervaloDisparo = 1.5f;

    [Header("Vida del Enemigo")]
    [SerializeField] private int maxPuntosVida; // Vida máxima
    private int puntosVida;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float cooldownTime = 1f; // Cooldown de daño
    private float nextDamageTime; // Control del próximo daño

    [Header("Objeto Adicional a Eliminar")]
    [SerializeField] private GameObject objetoAEliminar;

    private NavMeshAgent agent;
    private float tiempoUltimoDisparo;
    private float tiempoUltimoDanho;

    private Transform destinoActual;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        puntosVida = maxPuntosVida;
        healthBar.SetMaxHealth(maxPuntosVida); // Configurar barra de vida
        currentState = EnemyState.Patrullando;
        destinoActual = puntoA;
    }

    void Update()
    {
        if (Time.time > tiempoUltimoDanho + intervaloDisparo)
        {
            MirarAlJugador();

            switch (currentState)
            {
                case EnemyState.Patrullando:
                    Patrullar();
                    DetectarJugador();
                    break;
                case EnemyState.Atacando:
                    Atacar();
                    break;
                case EnemyState.Escapando:
                    Escapar();
                    break;
            }
        }
    }

    private void MirarAlJugador()
    {
        if (player != null)
        {
            float dirX = player.position.x - transform.position.x;
            transform.rotation = Quaternion.Euler(0, dirX > 0 ? 180 : 0, 0);
        }
    }

    private void DetectarJugador()
    {
        float distanciaJugador = Vector3.Distance(transform.position, player.position);

        if (distanciaJugador <= distanciaEscape)
        {
            currentState = EnemyState.Escapando;
        }
        else if (distanciaJugador <= distanciaVision)
        {
            currentState = EnemyState.Atacando;
        }
    }

    private void Patrullar()
    {
        if (destinoActual != null)
        {
            agent.speed = velocidadPatrulla;
            agent.SetDestination(destinoActual.position);

            if (Vector3.Distance(transform.position, destinoActual.position) < 0.5f)
            {
                destinoActual = destinoActual == puntoA ? puntoB : puntoA;
            }
        }
    }

    private void Atacar()
    {
        float distanciaJugador = Vector3.Distance(transform.position, player.position);

        if (distanciaJugador > distanciaVision)
        {
            currentState = EnemyState.Patrullando;
        }
        else if (distanciaJugador <= distanciaEscape)
        {
            currentState = EnemyState.Escapando;
        }
        else
        {
            agent.SetDestination(transform.position);

            if (Time.time > tiempoUltimoDisparo + intervaloDisparo)
            {
                DispararProyectil();
                tiempoUltimoDisparo = Time.time;
            }
        }
    }

    private void Escapar()
    {
        float distanciaJugador = Vector3.Distance(transform.position, player.position);

        if (distanciaJugador > distanciaSegura)
        {
            currentState = EnemyState.Atacando;
        }
        else
        {
            Vector3 direccionEscape = (transform.position - player.position).normalized;
            Vector3 destinoEscape = transform.position + direccionEscape * 5f;
            agent.SetDestination(destinoEscape);
        }
    }

    private void DispararProyectil()
    {
        if (proyectilPrefab != null && puntoDisparo != null)
        {
            GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
            Vector2 direccion = (player.position - puntoDisparo.position).normalized;
            Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direccion * velocidadProyectil;
            }
        }
    }

    public void Golpear()
    {
        if (currentState != EnemyState.Muerto && Time.time >= nextDamageTime)
        {
            puntosVida -= 1;
            healthBar.SetHealth(puntosVida); // Actualizar barra de vida
            nextDamageTime = Time.time + cooldownTime; // Actualizar el tiempo para recibir daño de nuevo

            if (puntosVida <= 0)
            {
                Matar();
            }
        }
    }

    private void Matar()
    {
        currentState = EnemyState.Muerto;

        // Destruir la barra de vida
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject, 1f);
            SceneManager.LoadSceneAsync(9);
        }

        // Destruir el enemigo después de un pequeño retraso
        Destroy(gameObject, 1f);

        if (objetoAEliminar != null)
        {
            Destroy(objetoAEliminar, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaEscape);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaSegura);
    }
}