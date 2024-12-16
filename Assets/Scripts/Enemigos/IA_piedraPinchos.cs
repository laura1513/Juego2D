using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IA_piedraPinchos: MonoBehaviour
{
    public enum EnemyState { Esperando, Moviendo }
    private EnemyState currentState;

    [Header("Puntos de Patrulla")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Variables del Enemigo")]
    [SerializeField] private float tiempoEspera = 1.5f;  // El tiempo de espera en cada punto
    private NavMeshAgent agent;
    private Transform currentPatrolPoint;
    private float esperaActual;  // Temporizador para contar el tiempo de espera

    private bool isMoving;  // Para asegurarse que las animaciones se actualicen correctamente si es necesario
    private Animator _animator;  // Para manejar animaciones si es necesario

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   //Estas dos lineas son para que el enemigo no rote
        agent.updateUpAxis = false;     //y se mueva hacia el objetivo de forma más fluida.

        currentState = EnemyState.Moviendo;  // Inicia en el estado Moviendo
        currentPatrolPoint = pointA;  // El enemigo comienza en el punto A

        esperaActual = tiempoEspera;  // Seteamos el tiempo de espera
        agent.SetDestination(currentPatrolPoint.position); // Movemos al enemigo al punto A

        // Animator
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Moviendo:
                Mover();
                break;
            case EnemyState.Esperando:
                Esperar();
                break;
        }
    }

    private void Mover()
    {
        // Mueve el agente hacia el punto actual
        agent.SetDestination(currentPatrolPoint.position);

        // Cuando el enemigo llega al destino
        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1.5f)
        {
            currentState = EnemyState.Esperando;  // Cambia a esperar cuando llega al punto
        }
    }

    private void Esperar()
    {
        // Reduce el tiempo de espera
        esperaActual -= Time.deltaTime;

        // Si ha pasado el tiempo de espera
        if (esperaActual <= 0f)
        {
            // Cambia al siguiente punto
            currentPatrolPoint = currentPatrolPoint == pointA ? pointB : pointA;

            // Restablece el tiempo de espera
            esperaActual = tiempoEspera;

            // Cambia el estado a "Moviendo" para que se mueva al siguiente punto
            currentState = EnemyState.Moviendo;
        }
    }

    // Visualización de los puntos de patrullaje en el editor con Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pointA.position, 0.5f);  // Visualiza el Punto A
        Gizmos.DrawWireSphere(pointB.position, 0.5f);  // Visualiza el Punto B
        Gizmos.DrawLine(pointA.position, pointB.position);  // Línea entre los puntos de patrullaje
    }
}