using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    private Rigidbody2D _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, 5);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Taca");
        this.transform.parent = collision.gameObject.transform; //Linea m�gica para que las flechas se claven en el enemigo
        _rb.angularVelocity = 0;
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;

        //Si golpeamos a un enemigo le quitamos vida y destruimos la flecha
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IA_enemigoCaC>().Golpear();
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("WizardEnemy"))
        {
            collision.gameObject.GetComponent<IA_enemigoADistancia>().Golpear();
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<IA_enemigoBoss>().Golpear();
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("BossTp"))
        {
            collision.gameObject.GetComponent<IA_enemigoBossTp>().Golpear();
            Destroy(this.gameObject);
        }
    }
}
