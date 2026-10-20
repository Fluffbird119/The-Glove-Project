using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grapple_aim_mover : MonoBehaviour
{
    public get_2_num_from_ARD num_2_ard;
    public jst_grappling_hook jst_grapple;

    public Rigidbody2D rb;
    float sensitivity = 0.01f;

    public GameObject aim_object;
    public GameObject player;

    public Vector2 move_dir;
    int renormalize_val = 5;
    int move_speed = 5;

    public bool can_shoot;

    float new_jx;
    float new_jy;

    // might want to swap to float cuz efficency
    double acutal_diagonal_move;

    void FixedUpdate()
    {
        if (jst_grapple.has_shot == false)
        { 
            rb.velocity = move_dir * move_speed;
        }
    }

    void Update()
    {
        float jx = num_2_ard.jx * sensitivity;
        float jy = num_2_ard.jy * sensitivity;

        //Debug.Log("happens");
        // x force being added
        move_dir_normalize(jx, jy);
        move_dir = new Vector2(new_jx, new_jy);
        // change .Normalize to a function that checks if the sqrt(jx^2 + jy^2) is greater than whatever the max speed you can get in either x or y and if so makes it go down
        // ^ possibly could put this in FixedUpdate() because we dont want the velocity getting multipled while the vecotr is being normalized
        //move_dir.Normalize();

        // rotation??????
        // no wiring issue :(
        // it was code issue so i could make it happen... nah i dont care that much

        /*
        if (aim_object.transform.position.x > 28 + player.transform.position.x )
        {
            Debug.Log("x value more than 28");
            //aim_object.transform.position = new Vector2(0, 10);
        }*/

        if (jst_grapple.has_shot == false)
        {
            // aim object is outside of camera on the right (the 28 and 13.2 are the bounds of the camera)
            if (aim_object.transform.position.x > 0 && aim_object.transform.position.x > 28 + player.transform.position.x)
            {
                // all the vectors that the dot is transformed to is based on play position
                aim_object.transform.position = new Vector2(player.transform.position.x + 10, player.transform.position.y + 5);
            }

            // aim object is outside of camera on the left
            else if (aim_object.transform.position.x < 0 && aim_object.transform.position.x < player.transform.position.x - 28)
            {
                aim_object.transform.position = new Vector2(player.transform.position.x + 10, player.transform.position.y + 5);
            }

            // aim object is above camera
            else if (aim_object.transform.position.y > 0 && aim_object.transform.position.y > 16.2 + player.transform.position.y)
            {
                aim_object.transform.position = new Vector2(player.transform.position.x + 10, player.transform.position.y + 5);
            }

            // for some reason the top goes up 13.2 and goes down 14.2 dunno why, prob cuz of camera position
            else if (aim_object.transform.position.y < 0 && aim_object.transform.position.y < player.transform.position.y - 14.2)
            {
                aim_object.transform.position = new Vector2(player.transform.position.x + 10, player.transform.position.y + 5);
            }
        }
    }

    // custom normalize to keep the dynamic movement allowed by a joystick system
    void move_dir_normalize(float x, float y)
    {
        // hypotnuse of triangle made with jx and jy
        acutal_diagonal_move = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        
        // checking if it exceeds what should be the max
        if (acutal_diagonal_move > 5 || acutal_diagonal_move < -5)
        {
            // a = x dir
            // b = y dir
            // temp_1 = (b / a)^2
            // temp_1 += 1 (adding up the x^2 on the left side of equation)
            // c_good = 2500 (or 25 depedning on what nums i use (50^2 or 5^2))
            // temp_2 = c_good / temp_1 (moving temp to other side)
            // final_a = sqrt(temp_2)
            // final_b = (temp_1 * final_a)

            float a = x; // for testing set to 33
            float b = y; // for testing set to 44
            float b_over_a = (float)Math.Sqrt(Math.Pow(b / a, 2));


            float temp_1 = (float)(Math.Pow(b / a, 2.0) + 1); // gets the num that goes in front of b after you distribute the ^2

            int c_good = 1; // 50^2 cuz 50 is the max value we want to reach

            float temp_2 = (float)(Math.Sqrt(c_good / temp_1)); // moves temp so equation is now the x (not the joy stick x but instead the x when i was working this out on paper) is isolated and renamed to temp_1

            // the *-1 makes it go down or left based on what the x or y was and the *renormalize_val scales it back to what it originally was
            if (x > 0)
            {
                Debug.Log("x is pos");
                new_jx = temp_2 * renormalize_val;
            }
            else if (x < 0)
            {
                Debug.Log("x is neg");
                new_jx = temp_2 * -1 * renormalize_val;
            }

            if (y > 0)
            {
                Debug.Log("y is pos");
                new_jy = b_over_a * temp_2 * renormalize_val;
            }
            else if (y < 0)
            {
                Debug.Log("y is neg");
                new_jy = b_over_a * temp_2 * -1 * renormalize_val;
            }

            /*
            Debug.Log("new jx: " + new_jx);
            Debug.Log("new jy: " + new_jy);

            Debug.Log("new c: " + Math.Sqrt(Math.Pow(new_jx, 2.0) + Math.Pow(new_jy, 2))); // should ALWAYS be 5
            */
        }
        else
        {
            new_jx = x;
            new_jy = y;
        }
    }

    // checks if the grapple object is intersecting a ground object and will allow you to grapple to said object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            can_shoot = true;
        }
    }

    // the opposite of the previous void stops you from being able to grapple when you arent aiming at an object

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            can_shoot = false;
        }
    }
}
