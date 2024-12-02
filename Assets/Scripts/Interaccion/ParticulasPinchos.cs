using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticulasPinchos : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem; // Sistema de partículas a activar
    private bool activo = false;
    private Animator anim;
    [SerializeField] float tiempoActivo = 1f;
    private float cuentaAtras;
    private void Start()
    {
        anim = GetComponent<Animator>();
        cuentaAtras = tiempoActivo;
    }
    private void Update()
    {
        cuentaAtras -= Time.deltaTime;
        if (cuentaAtras <= 0)
        {
            activo = !activo;
            anim.SetTrigger("activo");
            cuentaAtras = tiempoActivo;
            if (activo)
            {
                GetComponent<BoxCollider2D>().enabled = true;
                Debug.Log("Activado");
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = false;
                Debug.Log("Desactivado");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && activo)
        {
            Debug.Log("Golpeado");
            Instantiate(particleSystem, collision.transform.position, collision.transform.rotation); //genera particulas
            // hit.collider.gameObject.GetComponent<IA_Enemigo>().Golpear(); tiene el quitar vida en el enemigo
        }
    }
 
}
