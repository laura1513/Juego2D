using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IA_enemigoBossTp : MonoBehaviour
{
    public enum EnemyState { Patrullando, DetectarJugador, Teletrasportando, Persiguiendo, Atacando, RecibiendoDa�o, Muerto }
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
    [SerializeField] private float radioTeletransporteMinimo = 2f; // Distancia m�nima
    [SerializeField] private float radioTeletransporteMaximo = 5f; // Distancia m�xima

    [Header("Vida del Enemigo")]
    [SerializeField] private int maxPuntosVida; // Vida m�xima
    private int puntosVida;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float cooldownTime = 1f; // Cooldown de da�o
    private float nextDamageTime; // Control del pr�ximo da�o

    [Header("Aturdimiento")]
    [SerializeField] private float duracionAturdimiento = 1.5f; // Tiempo que se queda quieto tras recibir da�o
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

            case EnemyState.RecibiendoDa�o:
                Aturdido();
                break;

            case EnemyState.Muerto:
                // Ya gestionado por el m�todo Matar
                break;
        }
    }

    private void MirarJugador()
    {
        Vector3 direccion = player.position - transform.position;

        // Si el jugador est� a la derecha, voltear el sprite
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

        // Intentar generar una posici�n v�lida dentro del rango especificado
        for (int i = 0; i < 10; i++) // M�ximo 10 intentos
        {
            // Generar una direcci�n aleatoria y calcular una distancia dentro del rango permitido
            Vector3 direccionAleatoria = Random.insideUnitSphere.normalized;
            float distanciaAleatoria = Random.Range(radioTeletransporteMinimo, radioTeletransporteMaximo);

            // Calcular la nueva posici�n basada en la direcci�n y la distancia
            nuevaPosicion = player.position + direccionAleatoria * distanciaAleatoria;
            nuevaPosicion.y = transform.position.y; // Mantener la posici�n Y del enemigo

            // Verificar si la nueva posici�n cumple con las restricciones
            float distanciaJugador = Vector3.Distance(nuevaPosicion, player.position);

            if (distanciaJugador >= radioTeletransporteMinimo && distanciaJugador <= radioTeletransporteMaximo)
            {
                // Realizar un chequeo de colisi�n usando un Raycast
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
            Debug.LogWarning("No se encontr� una posici�n v�lida para teletransportar al enemigo.");
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

        // Aqu� puedes a�adir l�gica para realizar un ataque (por ejemplo, da�o al jugador)
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
                CambiarEstado(EnemyState.RecibiendoDa�o);
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
        // Visualizaci�n del rango de visi�n y ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // Visualizaci�n de los radios de teletransporte
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Verde con transparencia
        Gizmos.DrawWireSphere(player.position, radioTeletransporteMaximo);
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Rojo con transparencia
        Gizmos.DrawWireSphere(player.position, radioTeletransporteMinimo);
    }
}
