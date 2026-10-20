using System;
using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class get_2_num_from_ARD : MonoBehaviour
{
    SerialPort port_2;
    string data_recieved;

    public int gnx;
    public int gny;
    public int gnz;

    public int jx;
    public int jy;
    public int sw;

    public int az;

    void Start()
    {
        port_2 = new SerialPort("COM5", 38400);
        port_2.WriteTimeout = 300;
        port_2.ReadTimeout = 5000;
        port_2.DtrEnable = true;
        port_2.RtsEnable = true;
        port_2.Open();
        InvokeRepeating("get_data_from_ard_2", 0f, 0.001f);
    }

    private void get_data_from_ard_2()
    {
        // lil check to reopen port if it can open in Start()
        if (port_2.IsOpen == false)
        {
            port_2.Open();
        }

        data_recieved = port_2.ReadExisting();

        if(data_recieved == "Initializing MPU...")
        {
            Debug.Log("NUM 2 BROKEN");
        }

        string[] split_data = data_recieved.Split(",");

        
        gnx = int.Parse(split_data[0]);
        gny = int.Parse(split_data[1]);
        gnz = int.Parse(split_data[2]);

        jx = int.Parse(split_data[3]);
        jy = int.Parse(split_data[4]);

        // the joystick never hits 0,0 so we have to add a check that will zero it out when its not moving
        if (jx <= 10 && jx >= 0)
        {
            jx = 0;
        }
        if (jy <= 10 && jy >= 0)
        {
            jy = 0;
        }

        jy *= -1; // the * -1 makes it work more sensically (when you push joystick up it goes up and vise versa)

        sw = int.Parse(split_data[5]);

        az = int.Parse(split_data[6]);

        if (az <= -25)
        {
            jx *= -1; // if glove is turned upside down it makes the joystick move sensically
        }
    }

    // after unity stops running it closes to port so ard IDE can access it again
    private void OnDestroy()
    {
        port_2.Close();
    }
}
