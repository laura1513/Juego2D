using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DianaLauraPuertas : MonoBehaviour
{
    public GameObject diana;
    public GameObject flecha;
    public GameObject trigerPuerta;
    public GameObject puerta;
    public GameObject puertaAbierta;
    private void Start()
    {
        trigerPuerta.SetActive(false);
        puerta.SetActive(true);
        puertaAbierta.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject)
        {
            puerta.SetActive(false);
            trigerPuerta.SetActive(true);
            puertaAbierta.SetActive(true);
        }
    }

}
