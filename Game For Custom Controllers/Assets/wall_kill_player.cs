using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall_kill_player : MonoBehaviour
{
    //public GameObject player;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Destroy(other.gameObject);
        }
    }
}
