using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappling_hook : MonoBehaviour
{
    public play_movement player_movement;
    public get_2_num_from_ARD shoot_num;

    public LineRenderer actual_hook_visual;
    public DistanceJoint2D actual_hook_joint;

    public LineRenderer pre_shot_aim;

    private Vector2 launch_dir;

    string launch_dir_str = "";

    bool has_shot;
    bool has_retrieved;

    int gnx;
    int gny;
    int gnz;

    int iteration_counter;

    public GameObject player;
    public GameObject aim_object;
    void Start()
    {
        actual_hook_joint.enabled = false;
    }

    void Update()
    {

        //****************** this is the auto aim script check other grappling hook script for joystick aim **********************//
        
        // every g num other than gx is useless lol
        gnx = shoot_num.gnx;
        gny = shoot_num.gny;
        gnz = shoot_num.gnz;

        check_hand_turn(gnx, gny, gnz);

        /*
        (int, int, int) ard_ints = (gnx, gny, gnz);
        Debug.Log("right hand: " + ard_ints + " " + iteration_counter);
        */
        RaycastHit2D hit_aim = Physics2D.Raycast(gameObject.transform.position, launch_dir);

        RaycastHit2D hit_actual = Physics2D.Raycast(gameObject.transform.position, launch_dir);

        launch_dir_str = player_movement.right_or_left_str_check();

        
        if (launch_dir_str == "left")
        {
            launch_dir = new Vector2(-2, 1);
        }
        else if (launch_dir_str == "right")
        {
            launch_dir = new Vector2(2, 1);
        }
        else if (launch_dir_str == "still")
        {
            launch_dir = new Vector2(0, 0);
        }
        
        if (hit_aim.collider.tag == "Ground")
        {
            pre_shot_aim.SetPosition(0, hit_aim.transform.position);
            pre_shot_aim.SetPosition(1, gameObject.transform.position);
            pre_shot_aim.enabled = true;
        }
        if (hit_aim.collider.tag == "Boundary")
        {
            pre_shot_aim.enabled = false;
        }

        if (has_shot == true)
        {
            Debug.Log("shot");
            if (hit_actual.collider.tag == "Ground" && actual_hook_joint.enabled == false)
            {
                actual_hook_visual.SetPosition(0, hit_actual.transform.position);
                actual_hook_visual.SetPosition(1, gameObject.transform.position);
                actual_hook_joint.connectedAnchor = hit_actual.transform.position;
                actual_hook_joint.enabled = true;
                actual_hook_visual.enabled = true;
                pre_shot_aim.enabled = false;
            }
        }
        else if (has_shot == false)
        {
            Debug.Log("un-shot");
            actual_hook_visual.enabled = false;
            actual_hook_joint.enabled = false;
        }

        if (actual_hook_joint.enabled == true)
        {
            actual_hook_visual.SetPosition(1, gameObject.transform.position);
        }

        // if both pre-aim and actual grapple are on the same spot it turns off pre-aim
        if (hit_aim.transform.position.x == hit_actual.transform.position.x && hit_aim.transform.position.y == hit_actual.transform.position.y && actual_hook_visual.enabled == true)
        {
            pre_shot_aim.enabled = false;
        }
    }

    void check_hand_turn(int gnx, int gny, int gnz)
    {
        if (gnx == -2 && has_shot == false)
        {
            has_shot = true;
        }

        if (gnx == 2 && has_shot == true)
        {
            has_shot = false;
        }
    }
}
