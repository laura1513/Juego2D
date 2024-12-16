using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonedasLaura : MonoBehaviour
{
    public GameObject moneda;
    public GameObject monedaSiguiente;
    public GameObject flecha;
    public int contador;
    public GameObject trigerPuerta;
    public GameObject puerta;
    public GameObject puertaAbierta;
    private void Start()
    {
        trigerPuerta.SetActive(false);
        puerta.SetActive(true);
        puertaAbierta.SetActive(false);
        moneda.SetActive(true);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject)
        {
            moneda.SetActive(false);
            monedaSiguiente.SetActive(true);
            contador = contador -1;
            AbrirPuerta();
        }
        
    }
    private void AbrirPuerta() {
        if (contador == 0) {
            trigerPuerta.SetActive(true);
            puerta.SetActive(false);
            puertaAbierta.SetActive(true);
         }
    }
}