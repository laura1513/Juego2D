using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Transform destination; // Punto de teletransportaci�n asignado desde el Inspector.

    [SerializeField]
    private float distanceThreshold = 0.2f; // Umbral m�nimo de distancia para evitar bucles.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica que el destino existe antes de intentar teletransportar.
        if (destination != null && Vector2.Distance(transform.position, collision.transform.position) > distanceThreshold)
        {
            collision.transform.position = destination.position;
        }
        else if (destination == null)
        {
            Debug.LogError("El destino no ha sido asignado en el inspector.");
        }
    }
}


