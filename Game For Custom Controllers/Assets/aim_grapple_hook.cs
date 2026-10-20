using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aim_grapple_hook : MonoBehaviour
{
    public play_movement target_script;

    public LineRenderer pre_shot_aim;

    private Vector2 launch_dir;

    string launch_dir_str = "";

    public GameObject aim_object;

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, launch_dir);

        launch_dir_str = target_script.right_or_left_str_check();

        if (launch_dir_str == "left")
        {
            launch_dir = new Vector2(-2, 1);
        }
        else if (launch_dir_str == "right")
        {
            launch_dir = new Vector2(2, 1);
        }

        if (hit.collider.tag == "Ground")
        {
            pre_shot_aim.SetPosition(0, hit.transform.position);
            pre_shot_aim.SetPosition(1, gameObject.transform.position);
            pre_shot_aim.enabled = true;
        }
        if (hit.collider.tag == "Boundary")
        {
            pre_shot_aim.enabled = false;
        }
    }
}
