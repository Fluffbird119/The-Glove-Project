using System;
using UnityEngine;
using System.Collections;
using System.IO.Ports;
public class get_num_from_ARD : MonoBehaviour
{
    // comments are in other script didnt want to rety
    SerialPort port_1;
    string data_recieved;

    public int nx = 0;
    public int ny = 0;
    public int nz = 0;

    int iteration_counter;

    void Start()
    {
        port_1 = new SerialPort("COM6", 38400);
        port_1.WriteTimeout = 300;
        port_1.ReadTimeout = 5000;
        port_1.DtrEnable = true;
        port_1.RtsEnable = true;
        port_1.Open();
        InvokeRepeating("get_data_from_ard", 0f, 0.001f);
    }

    private void get_data_from_ard()
    {
        if (port_1.IsOpen == false)
        {
            port_1.Open();
        }

        data_recieved = port_1.ReadExisting();

        if (data_recieved == "Initializing MPU...")
        {
            Debug.Log("NUM 1 BROKEN");
        }

        string[] split_data = data_recieved.Split(",");

        if (split_data.Length == 3)
        {
            nx = int.Parse(split_data[0]);
            ny = int.Parse(split_data[1]);
            nz = int.Parse(split_data[2]);

            // just an organized way to print out the 3 values for troubleshooting
            /*
            (int, int, int) ard_ints = (nx, ny, nz);
            Debug.Log("LEFT hand: " + ard_ints + " " + iteration_counter);
            */
        }
    }

    private void OnDestroy()
    {
        port_1.Close();
    }
}