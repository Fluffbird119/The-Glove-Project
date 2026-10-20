using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jst_grappling_hook : MonoBehaviour
{
    public get_2_num_from_ARD get_ard_2; // i change my naming conventions everytime lol
    public grapple_aim_mover aim_mover_script;

    public GameObject player;
    public GameObject aim_object;

    public LineRenderer pre_shot_aim;

    public LineRenderer hook_visual;
    public DistanceJoint2D hook_joint;


    bool can_shoot;
    public bool has_shot;
    bool hand_turn_swap;
    public bool swapped_sw = false;

    int gnx = 0;
    int pre_gnx;
    int pre_sw;
    
    private void Update()
    {
        pre_gnx = gnx;
        gnx = get_ard_2.gnx;
        pre_sw = get_ard_2.sw;

        Physics2D.Linecast(player.transform.position, aim_object.transform.position);

        can_shoot = aim_mover_script.can_shoot;

        check_sw(pre_sw);

        if (can_shoot == true)
        {
            // visual turns on when aim object in a ground object
            pre_shot_aim.SetPosition(0, aim_object.transform.position);
            pre_shot_aim.SetPosition(1, player.transform.position);
            pre_shot_aim.enabled = true;

            // when you are aimed at an object it starts sending constant checks to see if youve fired yet
            check_hand_turn(gnx);

            // once youve fired
            if (has_shot == true)
            {
                //Debug.Log("shot");

                pre_shot_aim.enabled = false;

                // visual for real hook turns on
                hook_visual.SetPosition(0, aim_object.transform.position);
                hook_visual.SetPosition(1, player.transform.position);
                hook_visual.enabled = true;

                // actual join turns on
                hook_joint.connectedAnchor = aim_object.transform.position;
                hook_joint.enabled = true;
            }
            else if (has_shot == false)
            {
                // visual off
                hook_visual.enabled = false;

                // joint off
                hook_joint.enabled = false;
            }
        }
        else if (can_shoot == false)
        {
            // visual turns off
            pre_shot_aim.enabled = false;

            // also turn off real hook
            hook_visual.enabled = false;
            hook_joint.enabled = false;
        }
    }

    void check_hand_turn(int gnx)
    {
        // when hand_turn_swap is false you turn glove left to shoot grapple
        if (hand_turn_swap == false)
        {
            // -gnx is when moving clockwise when viewing the glove from where you hand goes it the fingers of the glove pointing straight away from you
            if (gnx == -2 && has_shot == true)
            {
                has_shot = false;
            }

            // +gnx is whe you move the glove counter clockwise following the same perameters
            if (gnx == 2 && has_shot == false)
            {
                has_shot = true;
            }
        }
        // opposite for hand turn swap being true
        else if (hand_turn_swap == true)
        {
            if (gnx == 2 && has_shot == true)
            {
                has_shot = false;
            }

            if (gnx == -2 && has_shot == false)
            {
                has_shot = true;
            }
        }


        // previous tries both didnt work well
        /*
        if (has_shot == true)
        {
            Debug.Log("shoot");
            has_shot = false;
        }
        else if (has_shot == false)
        {
            Debug.Log("unshoot");
            has_shot = true;
        }

        if (az > 0)
        {
            has_shot = false;
        }
        else if (az < -1)
        {
            has_shot = true;
        }
        */
    }
    void check_sw(int pre_sw)
    {
        // if you press down joystick it swaps what direction you turn your hand to use to shoot grappling hook
        if (pre_sw == 1 && swapped_sw == false)
        {
            swapped_sw = true;

            // changes hand_turn_swap to the opposite of what it currently is
            if (hand_turn_swap == false)
            {
                // since you start by turning left to shoot this will make it right
                hand_turn_swap = true;
                //Debug.Log(hand_turn_swap);
            }

            else if (hand_turn_swap == true)
            {
                // this will turn it back to left turn
                hand_turn_swap = false;
                //Debug.Log(hand_turn_swap);
            }
        }

        // once you stop pressing the joystick down you can swap it back again
        else if (pre_sw == 0)
        {
            swapped_sw = false;
        }
    }
}
