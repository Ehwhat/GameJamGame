using UnityEngine;
using System.Collections;

public class EndLevel : MonoBehaviour
{

    int players = 0;

    void Update()
    {
        if (players == 1)
        {
            Application.Quit();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            players++;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            players--;
        }
    }
}
