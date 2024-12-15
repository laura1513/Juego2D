using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IA_enemigoBoss: MonoBehaviour
{
    public enum EnemyState { Patrullando, Atacando, Escapando, Esperando, Muerto }
    private EnemyState currentState;

    [Header("Variables del Enemigo")]
    [SerializeField] private float distanciaVision = 10f;
    [SerializeField] private float distanciaEscape = 3f;
    [SerializeField] private float distanciaSegura = 7f;
    [SerializeField] private UnityEvent eventoAtaque;
    [SerializeField] private Transform player;

    [Header("Movimiento de Patrulla")]
    [SerializeField] private Transform puntoA; // Punto inicial de la patrulla
    [SerializeField] private Transform puntoB; // Punto final de la patrulla
    [SerializeField] private float velocidadPatrulla = 3f;

    [Header("Ataque con Proyectil")]
    [SerializeField] private GameObject proyectilPrefab;   // Prefab del proyectil
    [SerializeField] private Transform puntoDisparo;      // Posición desde donde se dispara
    [SerializeField] private float velocidadProyectil = 10f; // Velocidad del proyectil
    [SerializeField] private float intervaloDisparo = 1.5f;  // Tiempo entre disparos
    [SerializeField] private float tiempoEntreDisparosTrasRecibirDanho = 2f; // Tiempo de espera después de recibir daño

    [Header("Vida del Enemigo")]
    [SerializeField] private int puntosVida;

    [Header("Objeto Adicional a Eliminar")]
    [SerializeField] private GameObject objetoAEliminar; // Para eliminar la otra parte del boss al morir

    private NavMeshAgent agent;
    private float tiempoUltimoDisparo; // Control del intervalo de disparos
    private float tiempoUltimoDanho; // Controlar el cooldown después de recibir daño

    private Transform destinoActual; // Destino actual para patrullar

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Evitar que el enemigo rote automáticamente
        agent.updateUpAxis = false;

        currentState = EnemyState.Patrullando; // Estado inicial
        destinoActual = puntoA; // Empieza yendo hacia el punto A
    }

    void Update()
    {
        if (Time.time > tiempoUltimoDanho + tiempoEntreDisparosTrasRecibirDanho)
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

                case EnemyState.Esperando:
                    DetectarJugador();
                    break;
            }
        }
    }

    private void MirarAlJugador()
    {
        if (player != null)
        {
            // Rotar para mirar al jugador en base a su posición relativa
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

            // Cambiar al siguiente destino cuando se alcanza el actual
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
            agent.SetDestination(transform.position); // Detener el movimiento
            // Disparar proyectiles si se cumple el intervalo de disparos
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
            // Moverse en la dirección opuesta al jugador
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
        if (currentState != EnemyState.Muerto)
        {
            puntosVida--;

            if (puntosVida <= 0)
            {
                Matar();
            }
            else
            {
                tiempoUltimoDanho = Time.time; // Actualizar cooldown tras daño
            }
        }
    }

    private void Matar()
    {
        currentState = EnemyState.Muerto;

        // Destruir el enemigo después de un pequeño retraso
        Destroy(gameObject, 1f);

        // Destruir el objeto adicional si se ha asignado
        if (objetoAEliminar != null)
        {
            Destroy(objetoAEliminar,1f);
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

