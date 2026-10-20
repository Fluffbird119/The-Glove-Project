using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class play_movement : MonoBehaviour
{
    public get_num_from_ARD move_num;
    public get_2_num_from_ARD shoot_num;

    bool jumped;

    public string right_or_left_str = "";
    public string cur_move_dir = "";

    public int move_speed = 2;
    public Rigidbody2D rb;
    public Vector2 move_dir;

    public int move_dir_num;
    public int hand_rotate_num;

    int anx = -13;
    int any = 1;
    int anz = 7;

    private float x_speed;
    public int x_speed_int;
    public float previous_position;

    int jump_height = 3;

    public bool on_ground = false;

    bool can_use_normal_speed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        anx = move_num.nx;
        any = move_num.ny;
        anz = move_num.nz;
    }

    private void FixedUpdate()
    {
        check_move_dir(anx, any, anz);
        //check_hand_turn(gnx, gny, gnz);

        // finds x_speed to allow jumping and conserving momentum from jumps easier (see check_move_dir() for usage)
        x_speed = (gameObject.transform.position.x - previous_position);
        previous_position = gameObject.transform.position.x;

        // whenever working with speeds vals use "x_speed_int" not "x_speed"
        x_speed_int = (int)(x_speed * 100);

        if (x_speed_int <= -7)
        {
            cur_move_dir = "left";
        }
        else if (x_speed_int >= 7)
        {
            cur_move_dir = "right";
        }
    }

    void check_move_dir(int anx, int any, int anz)
    {
        // not moving perfectly flat
        if (anx >= -17 && anx <= -10 && any >= 5 && any <= 12 && anz >= 0 && anz <= 2)
        {
            move_dir = new Vector2(0, 0);

            jumped = false;

            right_or_left_str = "still";
        }

        // left 
        if (anx >= -23 && anx <= 3 && any >= -7 && any <= 4 && anz >= -22 && anz <= 0)
        {
            move_dir = new Vector2(-2, 0);
            right_or_left_str = "left";
            
            jumped = false;
        }

        // right
        if (anx >= -16 && anx <= -9 && any >= 13 && any <= 26 && anz >= -17 && anz <= 1)
        {
            move_dir = new Vector2(2, 0);
            right_or_left_str = "right";

            jumped = false;
        }

        // jump
        if (anx >= -28 && anx <= -17 && any >= 6 && any <= 9 && anz >= -12 && anz <= 0)
        {
            if (right_or_left_str == "right")
            {
                move_dir = new Vector2(2, jump_height);
            }
            else if (right_or_left_str == "left")
            {
                move_dir = new Vector2(-2, jump_height);
            }

            jumped = true;
            
        }

        // back
        /*
        if (anx >= -9 && anx <= 4 && any >= 6 && any <= 12 && anz >= -7 && anz <= 2)
        {
            // nothing currently no reason to make move down cuz gravity
        }
        */

        // keeps momenetum from grappling until you slow down 
        if (x_speed_int >= -7 && x_speed_int <= 7 || cur_move_dir != right_or_left_str)
        {
            //Debug.Log("using normal movement");
            can_use_normal_speed = true;
        }
        else
        {
            can_use_normal_speed = false;
        }

        // if you are on ground then the movement vectors ar applied
        if (on_ground == true && can_use_normal_speed == true)
        {
            rb.velocity = move_dir * move_speed;
        }
        else if (on_ground == true && can_use_normal_speed == false && jumped == true) // on ground moving faster than normal speed so you can conserve momentum by jumping
        {
            if (right_or_left_str == "right" && x_speed_int > 7)
            {
                move_dir = new Vector2(2, 2);
            }
            else if (right_or_left_str == "left" && x_speed_int < -7)
            {
                move_dir = new Vector2(-2, 2);
            }

            // bigger jump based on how fast you are moving on the ground
            rb.velocity = move_dir * x_speed_int / 4;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            on_ground = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            on_ground = false;
        }
    }

    public string right_or_left_str_check()
    {
        return right_or_left_str;
    }
}
