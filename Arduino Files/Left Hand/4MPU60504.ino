#include <MPU6050.h>

#include <MPU6050.h>

#include <MPU6050.h>

/*
  MPU6050 Raw

  A code for obtaining raw data from the MPU6050 module with the option to
  modify the data output format.

  Find the full MPU6050 library documentation here:
  https://github.com/ElectronicCats/mpu6050/wiki
*/
#include "I2Cdev.h"
#include "MPU6050.h"

MPU6050 mpu;

float pre_ax;
float pre_ay;
float pre_az;
float pre_gx;
float pre_yx;
float pre_zx;

int nx;
int ny;
int nz;

int offset_num = 3;
int offset_num_y = 1;
int write_num;

#define OUTPUT_READABLE_ACCELGYRO
//#define OUTPUT_BINARY_ACCELGYRO

int16_t ax, ay, az;
int16_t gx, gy, gz;
bool blinkState;

void setup() {
  /*--Start I2C interface--*/
  #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    Wire.begin(); 
  #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
    Fastwire::setup(400, true);
  #endif

  Serial.begin(38400); //Initializate Serial no work well at 8MHz/16MHz

  /*Initialize device */
  mpu.initialize();

  mpu.setXAccelOffset(0); //Set your accelerometer offset for axis X
  mpu.setYAccelOffset(0); //Set your accelerometer offset for axis Y
  mpu.setZAccelOffset(0); //Set your accelerometer offset for axis Z
  mpu.setXGyroOffset(0);  //Set your gyro offset for axis X
  mpu.setYGyroOffset(0);  //Set your gyro offset for axis Y
  mpu.setZGyroOffset(0);  //Set your gyro offset for axis Z
  /*Print the defined offsets*/
/*
  Serial.print("\t");
  Serial.print(mpu.getXAccelOffset());
  Serial.print("\t");
  Serial.print(mpu.getYAccelOffset()); 
  Serial.print("\t");
  Serial.print(mpu.getZAccelOffset());
  Serial.print("\t");
  Serial.print(mpu.getXGyroOffset()); 
  Serial.print("\t");
  Serial.print(mpu.getYGyroOffset());
  Serial.print("\t");
  Serial.print(mpu.getZGyroOffset());
  Serial.print("\n");
*/  

  /*Configure board LED pin for output*/ 
  pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {
  if(mpu.testConnection() ==  false)
  {
    Serial.println("Retrying");
    mpu.initialize();
    mpu.setXAccelOffset(0); //Set your accelerometer offset for axis X
    mpu.setYAccelOffset(0); //Set your accelerometer offset for axis Y
    mpu.setZAccelOffset(0); //Set your accelerometer offset for axis Z
    mpu.setXGyroOffset(0);  //Set your gyro offset for axis X
    mpu.setYGyroOffset(0);  //Set your gyro offset for axis Y
    mpu.setZGyroOffset(0);  //Set your gyro offset for axis Z
  }

  mpu.getRotation(&gx, &gy, &gz);
/*
  /*Print the obtained data on the defined format*/
  // #ifdef OUTPUT_READABLE_ACCELGYRO
  //   Serial.print("a/g:\t");
  //   Serial.print(ax); Serial.print("\t");
  //   Serial.print(ay); Serial.print("\t");
  //   Serial.print(az); Serial.print("\t");
  //   Serial.print(gx); Serial.print("\t");
  //   Serial.print(gy); Serial.print("\t");
  //   Serial.println(gz);
  //   delay(300);
  // #endif

  /* what a and g do
    g values give the speed at which direction it is traveling
    when you quick move (rotation or otherwise) it will sharply increase in values for a short period of time

    a values give more "rotational" values when you put it at certain angles it makes the numbers largely neg or pos
  

  // if((ax + ay + az + gx + gy + gz) == 0)
  // {
  //   Serial.println("Retrying 2");
  //   mpu.initialize();
  //   mpu.setXAccelOffset(0); //Set your accelerometer offset for axis X
  //   mpu.setYAccelOffset(0); //Set your accelerometer offset for axis Y
  //   mpu.setZAccelOffset(0); //Set your accelerometer offset for axis Z
  //   mpu.setXGyroOffset(0);  //Set your gyro offset for axis X
  //   mpu.setYGyroOffset(0);  //Set your gyro offset for axis Y
  //   mpu.setZGyroOffset(0);  //Set your gyro offset for axis Z
  // }
  */

  /*
  // not moving perfectly flat
  if (nx >= -17 && nx <= -10)
  {
    if (ny >= 5 && ny <= 12) 
    {
      if (nz >= 0 && nz <= 2)
      {
        write_num = 0;

        Serial.println("flat");
      }
    }
  }

  // * For all directions add "semi" parts to it later cuz it takes too much time kinsa like typing this message

  // left 
  if (nx >= -14 && nx <= 3)
  {
    if (ny >= -7 && ny <= 4)
    {
      if (nz >= -22 && nz <= 0)
      {
        write_num = 1;

        Serial.println("left");
      }
    }
  }

  // right
  if (nx >= -19 && nx <= -9)
  {
    if (ny >= 13 && ny <= 26)
    {
      if (nz >= -17 && nz <= 1)
      {
        write_num = 2;

        Serial.println("right");
      }
    }
  }

  // front
  if (nx >= -28 && nx <= -15)
  {
    if (ny >= -6 && ny <= 23)
    {
      if (nz >= -17 && nz <= 0)
      {
        write_num = 3;

        Serial.println("front");
      }
    }
  }

  // back -5, 8, 0
  if (nx >= -9 && nx <= 4)
  {
    if (ny >= 6 && ny <= 12)
    {
      if (nz >= -7 && nz <= 2)
      {
        write_num = 4;

        //Serial.println("back");
      }
    }
  }
*/

  int ngx = normalize_g(gx);
  int ngy = normalize_g(gy);

  mpu.getAcceleration(&ax, &ay, &az);

  nx = normalize_ax(ay); // THIS IS NORMAL cuz now side to side is x movement
  ny = normalize_ay(ax); // and up and down movement is printed as y movement

  if (nx > 12)
  {
    nx = 12;
  }
  else if (nx < -12)
  {
    nx = -12;
  }

  if (ny > 12)
  {
    ny = 12;
  }
  else if (ny < -12)
  {
    ny = -12;
  }

/*
  pre_ax = nx;
  pre_ay = ny;
  pre_az = nz;

  // normalize these later
  pre_gx = gx;
  pre_yx = gy;
  pre_zx = gz;
*/

 // this print statement is everything that unity will read from here
  Serial.print(String(nx) + "," + String(ny) +"," + String(ngx) + "," + String(ngy));
  //Serial.print(String(nz)); // z vals do nothing ive come to realize with how i coded everything
  Serial.flush();
  delay(6); // leave at 6 change the invoke reapeating in unity 
  // 50 initially
}

// makes numbers make sense and be at zero when hand is flat
int normalize_ax(int ax)
{
  int nx = (ax/1000) + 4;

  if (nx >= (-1 * offset_num) && nx <= offset_num)
  {
    nx = 0;
  }

  if (nx < 0)
  {
    nx += offset_num;
  }
  else if (nx > 0)
  {
    nx -= offset_num;
  }

  return nx;
}

int normalize_ay(int ay)
{
  int ny = (ay/1000) * -1; // * -1 so that backwards is negative and forwards is positive

  if (ny >= (-1 * offset_num_y) && ny <= offset_num_y)
  {
    ny = 0;
  }

  if (ny < 0)
  {
    ny += offset_num_y;
  }
  else if (ny > 0)
  {
    ny -= offset_num_y;
  }

  return ny;
}

int normalize_az(int az)
{
  int nz = az/10000;

  if (nz >= -2 && 1 <= 2)
  {
    nz = 0;
  }

  return nz;
}

int normalize_g(int g_num)
{
  int new_g_num = g_num/1000;
  new_g_num *= -1;
  return new_g_num;
}
