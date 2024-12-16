using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IA_enemigoBossTp : MonoBehaviour
{
    public enum EnemyState { Patrullando, DetectarJugador, Teletrasportando, Persiguiendo, Atacando, RecibiendoDaño, Muerto }
    private EnemyState currentState;

    [Header("Variables del Enemigo")]
    [SerializeField] private float distanciaVision = 10f;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private Transform player;

    [Header("Movimiento de Patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float velocidadPatrulla = 3f;

    [Header("Teletransporte")]
    [SerializeField] private float radioTeletransporte = 5f;
    [SerializeField] private float tiempoPersecucion = 3f; // Tiempo que persigue antes de teletransportarse
    [SerializeField] private float radioTeletransporteMinimo = 2f; // Distancia mínima
    [SerializeField] private float radioTeletransporteMaximo = 5f; // Distancia máxima

    [Header("Vida del Enemigo")]
    [SerializeField] private int maxPuntosVida; // Vida máxima
    private int puntosVida;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float cooldownTime = 1f; // Cooldown de daño
    private float nextDamageTime; // Control del próximo daño

    [Header("Aturdimiento")]
    [SerializeField] private float duracionAturdimiento = 1.5f; // Tiempo que se queda quieto tras recibir daño
    private float tiempoFinAturdimiento;

    private NavMeshAgent agent;
    private Transform destinoActual;
    private float tiempoInicioPersecucion;
    [SerializeField] private GameObject puertaBoss;

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

        // Mirar siempre al jugador
        MirarJugador();

        switch (currentState)
        {
            case EnemyState.Patrullando:
                Patrullar();
                DetectarJugador();
                break;

            case EnemyState.DetectarJugador:
                CambiarEstado(EnemyState.Teletrasportando);
                break;

            case EnemyState.Teletrasportando:
                Teletransportarse();
                CambiarEstado(EnemyState.Persiguiendo);
                break;

            case EnemyState.Persiguiendo:
                PerseguirJugador();
                break;

            case EnemyState.Atacando:
                Atacar();
                break;

            case EnemyState.RecibiendoDaño:
                Aturdido();
                break;

            case EnemyState.Muerto:
                // Ya gestionado por el método Matar
                break;
        }
    }

    private void MirarJugador()
    {
        Vector3 direccion = player.position - transform.position;

        // Si el jugador está a la derecha, voltear el sprite
        if (direccion.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // Mirar hacia la derecha
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Mirar hacia la izquierda
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

    private void DetectarJugador()
    {
        float distanciaJugador = Vector3.Distance(transform.position, player.position);

        if (distanciaJugador <= distanciaVision)
        {
            CambiarEstado(EnemyState.Teletrasportando);
        }
    }

    private void Teletransportarse()
    {
        Vector3 nuevaPosicion;
        bool posicionValida = false;

        // Intentar generar una posición válida dentro del rango especificado
        for (int i = 0; i < 10; i++) // Máximo 10 intentos
        {
            // Generar una dirección aleatoria y calcular una distancia dentro del rango permitido
            Vector3 direccionAleatoria = Random.insideUnitSphere.normalized;
            float distanciaAleatoria = Random.Range(radioTeletransporteMinimo, radioTeletransporteMaximo);

            // Calcular la nueva posición basada en la dirección y la distancia
            nuevaPosicion = player.position + direccionAleatoria * distanciaAleatoria;
            nuevaPosicion.y = transform.position.y; // Mantener la posición Y del enemigo

            // Verificar si la nueva posición cumple con las restricciones
            float distanciaJugador = Vector3.Distance(nuevaPosicion, player.position);

            if (distanciaJugador >= radioTeletransporteMinimo && distanciaJugador <= radioTeletransporteMaximo)
            {
                // Realizar un chequeo de colisión usando un Raycast
                if (!Physics.CheckSphere(nuevaPosicion, 0.5f, LayerMask.GetMask("player")))
                {
                    posicionValida = true;
                    agent.Warp(nuevaPosicion);
                    break;
                }
            }
        }

        if (!posicionValida)
        {
            Debug.LogWarning("No se encontró una posición válida para teletransportar al enemigo.");
        }
    }



    private void PerseguirJugador()
    {
        agent.SetDestination(player.position);

        if (Time.time >= tiempoInicioPersecucion + tiempoPersecucion)
        {
            CambiarEstado(EnemyState.Teletrasportando);
        }
        else if (Vector3.Distance(transform.position, player.position) <= distanciaAtaque)
        {
            CambiarEstado(EnemyState.Atacando);
        }
    }

    private void Atacar()
    {
        agent.SetDestination(transform.position); // Detener el movimiento mientras ataca

        if (Vector3.Distance(transform.position, player.position) > distanciaAtaque)
        {
            CambiarEstado(EnemyState.Persiguiendo);
        }

        // Aquí puedes añadir lógica para realizar un ataque (por ejemplo, daño al jugador)
        // eventoAtaque.Invoke();
    }

    public void Golpear()
    {
        if (currentState != EnemyState.Muerto && Time.time >= nextDamageTime)
        {
            puntosVida -= 1;
            healthBar.SetHealth(puntosVida);
            nextDamageTime = Time.time + cooldownTime;

            if (puntosVida <= 0)
            {
                Matar();
            }
            else
            {
                CambiarEstado(EnemyState.RecibiendoDaño);
                tiempoFinAturdimiento = Time.time + duracionAturdimiento;
            }
        }
    }

    private void Matar()
    {
        currentState = EnemyState.Muerto;

        if (healthBar != null)
        {
            Destroy(healthBar.gameObject,1f);
            Destroy(puertaBoss,3f);
        }

        Destroy(gameObject, 1f);
    }

    private void CambiarEstado(EnemyState nuevoEstado)
    {
        currentState = nuevoEstado;

        if (nuevoEstado == EnemyState.Persiguiendo)
        {
            tiempoInicioPersecucion = Time.time;
        }
    }
    private void Aturdido()
    {
        agent.SetDestination(transform.position); // Detener el movimiento

        if (Time.time >= tiempoFinAturdimiento)
        {
            // Volver al estado anterior o elegir un estado adecuado
            CambiarEstado(EnemyState.Patrullando);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualización del rango de visión y ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // Visualización de los radios de teletransporte
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Verde con transparencia
        Gizmos.DrawWireSphere(player.position, radioTeletransporteMaximo);
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Rojo con transparencia
        Gizmos.DrawWireSphere(player.position, radioTeletransporteMinimo);
    }
}
