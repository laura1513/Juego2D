using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DianaLaura : MonoBehaviour
{
    public GameObject diana;
    public GameObject flecha;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Flecha"))
        {
            //Cambiar de nivel
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log(SceneManager.GetActiveScene().buildIndex+1);

        }
    }

}
