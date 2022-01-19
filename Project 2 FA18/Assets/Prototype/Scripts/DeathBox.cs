using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathBox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Destroy(c.gameObject);
        }
    }
}
