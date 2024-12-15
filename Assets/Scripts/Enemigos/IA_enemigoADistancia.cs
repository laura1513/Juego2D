using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IA_enemigoADistancia : MonoBehaviour
{
    public enum EnemyState { Atacando, Esperando, Muerto }
    private EnemyState currentState;

    [Header("Variables del Enemigo")]
    [SerializeField] private float distanciaVision = 10f;
    [SerializeField] private float tiempoEspera = 2f;
    [SerializeField] private UnityEvent eventoAtaque;
    [SerializeField] private Transform player;

    [Header("Ataque con Proyectil")]
    [SerializeField] private GameObject proyectilPrefab;   // Prefab del proyectil
    [SerializeField] private Transform puntoDisparo;      // Posici�n desde donde se dispara
    [SerializeField] private float velocidadProyectil = 10f; // Velocidad del proyectil
    [SerializeField] private float intervaloDisparo = 1.5f;  // Tiempo entre disparos
    [SerializeField] private float tiempoEntreDisparosTrasRecibirDanho = 2f; // Tiempo de espera despu�s de recibir da�o

    [Header("Vida del Enemigo")]
    [SerializeField] private int puntosVida;

    private NavMeshAgent agent;
    private float esperaActual;
    private float tiempoUltimoDisparo; // Control del intervalo de disparos
    private float tiempoUltimoDanho; // Controlar el cooldown despu�s de recibir da�o

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   //Estas dos lineas son 
        agent.updateUpAxis = false;     //para que el enemigo no rote y deje de verse
        currentState = EnemyState.Esperando;

    }

    void Update()
    {
        if (Time.time > tiempoUltimoDanho + tiempoEntreDisparosTrasRecibirDanho)
        {
            MirarAlJugador();

            switch (currentState)
            {
                case EnemyState.Atacando:
                    Atacar();
                    break;

                case EnemyState.Esperando:
                    DetectarJugador();
                    break;

                case EnemyState.Muerto:
                    // L�gica opcional para el estado muerto
                    break;
            }
        }
    }

    private void MirarAlJugador()
    {
        // Detecta la posici�n del jugador relativa al enemigo
        if (player != null)
        {
            // Verificar si el jugador est� a la derecha o izquierda del enemigo
            if (player.position.x > transform.position.x)
            {
                // El jugador est� a la derecha, rotamos el enemigo 180 grados (mirando hacia la derecha)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (player.position.x < transform.position.x)
            {
                // El jugador est� a la izquierda, rotamos el enemigo 0 grados (mirando hacia la izquierda)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void DetectarJugador()
    {
        // Detecta si el jugador est� dentro del rango de visi�n
        if (Vector3.Distance(transform.position, player.position) <= distanciaVision)
        {
            currentState = EnemyState.Atacando;
        }
    }

    private void Atacar()
    {
        // Si el jugador est� fuera de la zona de visi�n, espera
        if (Vector3.Distance(transform.position, player.position) > distanciaVision)
        {
            currentState = EnemyState.Esperando;
            return;
        }

        // Disparar si el tiempo desde el �ltimo disparo ha pasado
        if (Time.time > tiempoUltimoDisparo + intervaloDisparo)
        {
            DispararProyectil();
            tiempoUltimoDisparo = Time.time;
        }
    }

    private void DispararProyectil()
    {
        if (proyectilPrefab != null && puntoDisparo != null)
        {
            // Crear el proyectil
            GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);

            // Direccionar el proyectil hacia el jugador
            Vector2 direccion = (player.position - puntoDisparo.position).normalized;

            // Aplicar velocidad al proyectil
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
                currentState = EnemyState.Muerto;
                Destroy(this.gameObject, 1f); // Destruir tras 1 segundo
            }

            // Activar cooldown tras recibir da�o
            tiempoUltimoDanho = Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar zona de visi�n
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);
    }
}
