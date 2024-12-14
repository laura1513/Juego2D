using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class Torreta : MonoBehaviour
{
    public enum EnemyState { NoAtacando,Atacando, Esperando }
    private EnemyState currentState;
    [SerializeField] private GameObject balaPref;
    [SerializeField] private float timeCooldown;

    //Ataque
    [SerializeField] private float distanciaVision = 10f;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float tiempoEspera = 3f;
    [SerializeField] private Transform player;
    private float esperaActual;


    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        
    }


    IEnumerator Shoot()
    {
        while (true) 
        { 
        yield return new WaitForSeconds(timeCooldown);
        Instantiate(balaPref, transform.position, Quaternion.identity);
        }

    }
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.NoAtacando:
                NoAtacando();
                break;
            case EnemyState.Atacando:
                Atacar();
                break;
            case EnemyState.Esperando:
                Esperar();
                break;
        }
    }

    private void NoAtacando()
    {
        if (Vector3.Distance(transform.position, player.position) <= distanciaAtaque)
        {
            currentState = EnemyState.Atacando;
        }
        else if (Vector3.Distance(transform.position, player.position) > distanciaVision)
        {
            StopAllCoroutines();
            currentState =EnemyState.Esperando;
        }
    }
    private void Atacar()
    { 
        StartCoroutine(Shoot());
        currentState = EnemyState.Esperando;
        esperaActual = tiempoEspera;
    }

    private void Esperar()
    {
        esperaActual -= Time.deltaTime;

        if (esperaActual <= 0)
        {
            currentState = EnemyState.NoAtacando;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque); // Zona de ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVision); // Zona de visión
    }
}
