using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class win_game : MonoBehaviour
{

    private bool has_touched_flag = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (has_touched_flag == false)
        {
            Debug.Log("YOU WIN YAYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY");
            has_touched_flag = true;
        }
    }
}
