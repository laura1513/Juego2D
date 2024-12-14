using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class IA_enemigoADistancia : MonoBehaviour
{
    public enum EnemyState { Atacando, Esperando, Muerto }
    private EnemyState currentState;

    [Header("Variables del Enemigo")]
    [SerializeField] private float distanciaVision = 10f;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float tiempoEspera = 2f;
    [SerializeField] private UnityEvent eventoAtaque;
    [SerializeField] private Transform player;

    [Header("Ataque con Proyectil")]
    [SerializeField] private GameObject proyectilPrefab;   // Prefab del proyectil
    [SerializeField] private Transform puntoDisparo;      // Posición desde donde se dispara
    [SerializeField] private float velocidadProyectil = 10f; // Velocidad del proyectil
    [SerializeField] private float intervaloDisparo = 1.5f;  // Tiempo entre disparos

    [Header("Vida del Enemigo")]
    [SerializeField] private int puntosVida;

    private float esperaActual;
    private float tiempoUltimoDisparo; // Control del intervalo de disparos

    void Start()
    {
        currentState = EnemyState.Esperando;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Atacando:
                Atacar();
                break;

            case EnemyState.Esperando:
                DetectarJugador();
                break;

            case EnemyState.Muerto:
                // Lógica opcional para el estado muerto
                break;
        }
    }

    private void DetectarJugador()
    {
        // Detecta si el jugador está dentro del rango de visión
        if (Vector3.Distance(transform.position, player.position) <= distanciaVision)
        {
            currentState = EnemyState.Atacando;
        }
    }

    private void Atacar()
    {
        // Si el jugador está fuera de la zona de visión, espera
        if (Vector3.Distance(transform.position, player.position) > distanciaVision)
        {
            currentState = EnemyState.Esperando;
            return;
        }

        // Disparar si el tiempo desde el último disparo ha pasado
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
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar zona de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);

        // Visualizar zona de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}
