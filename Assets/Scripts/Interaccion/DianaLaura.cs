using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DianaLaura : MonoBehaviour
{
    public GameObject diana;
    public GameObject flecha;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject)
        {
            //Cambiar de nivel
            Debug.Log("Diana");
            SceneManager.LoadScene("LauraSambori");
        }
    }

}
