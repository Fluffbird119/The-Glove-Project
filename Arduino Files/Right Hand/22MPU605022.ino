#include <MPU6050.h>

#include <Keyboard.h>
#include <Mouse.h>

/*
  MPU6050 Raw

  A code for obtaining raw data from the MPU6050 module with the option to
  modify the data output format.

  Find the full MPU6050 library documentation here:
  https://github.com/ElectronicCats/mpu6050/wiki
*/
#include "I2Cdev.h"
#include "MPU6050.h"

#include "Keyboard.h"

#define SW 9
#define jx A0
#define jy A3

/* MPU6050 default I2C address is 0x68*/
MPU6050 mpu;
//MPU6050 mpu(0x69);         //Use for AD0 high
//MPU6050 mpu(0x68, &Wire1); //Use for AD0 low, but 2nd Wire (TWI/I2C) object.

/* OUTPUT FORMAT DEFINITION----------------------------------------------------------------------------------
- Use "OUTPUT_READABLE_ACCELGYRO" if you want to see a tab-separated list of the accel 
X/Y/Z and gyro X/Y/Z values in decimal. Easy to read, but not so easy to parse, and slower over UART.

- Use "OUTPUT_BINARY_ACCELGYRO" to send all 6 axes of data as 16-bit binary, one right after the other. 
As fast as possible without compression or data loss, easy to parse, but impossible to read for a human. 
This output format is used as an output.
--------------------------------------------------------------------------------------------------------------*/ 

float pre_ax;
float pre_ay;
float pre_az;

float pre_gx = 0;
float pre_gy = 0;
float pre_gz = 0;

int ngx = 0;
int ngy = 0;
int ngz = 0;

int write_num;

#define OUTPUT_READABLE_ACCELGYRO
//#define OUTPUT_BINARY_ACCELGYRO

int16_t ax, ay, az;
int16_t gx, gy, gz;
bool blinkState;

bool g_vals_normal = true;

void setup() {
  /*--Start I2C interface--*/
  #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    Wire.begin(); 
  #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
    Fastwire::setup(400, true);
  #endif

  Serial.begin(38400); //Initializate Serial no work well at 8MHz/16MHz

  // joystick initialize
  pinMode(SW, INPUT_PULLUP);
  pinMode(jx, INPUT);
  pinMode(jy, INPUT);

  /*Initialize device and check connection*/ 
  Serial.println("Initializing MPU...");
  mpu.initialize();
  Serial.println("Testing MPU6050 connection...");
  if(mpu.testConnection() ==  false){
    Serial.println("MPU6050 connection failed");
    // while(true);
  }
  else{
    Serial.println("MPU6050 connection successful");
  }

  /* Use the code below to change accel/gyro offset values. Use MPU6050_Zero to obtain the recommended offsets */ 
  Serial.println("Updating internal sensor offsets...\n");
  mpu.setXAccelOffset(0); //Set your accelerometer offset for axis X
  mpu.setYAccelOffset(0); //Set your accelerometer offset for axis Y
  mpu.setZAccelOffset(0); //Set your accelerometer offset for axis Z
  mpu.setXGyroOffset(0);  //Set your gyro offset for axis X
  mpu.setYGyroOffset(0);  //Set your gyro offset for axis Y
  mpu.setZGyroOffset(0);  //Set your gyro offset for axis Z
  /*Print the defined offsets*/
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

  /*Configure board LED pin for output*/ 
  pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {
  if(mpu.testConnection() ==  false)
  {
    //Serial.println("Retrying");
    mpu.initialize();
    mpu.setXAccelOffset(0); //Set your accelerometer offset for axis X
    mpu.setYAccelOffset(0); //Set your accelerometer offset for axis Y
    mpu.setZAccelOffset(0); //Set your accelerometer offset for axis Z
    mpu.setXGyroOffset(0);  //Set your gyro offset for axis X
    mpu.setYGyroOffset(0);  //Set your gyro offset for axis Y
    mpu.setZGyroOffset(0);  //Set your gyro offset for axis Z
  }

  // getting data from joystick
  int jrx = analogRead(jx);
  int jry = analogRead(jy);

  // maping said joystick data properly
  jrx = map(jrx, 0, 659, -500, 500); // 659 for 3v3, 1023 for 5v
  jry = map(jry, 0, 659, 500, -500); // 659 for 3v3, 1023 for 5v

  pre_gx = ngx;
  pre_gy = ngy;
  pre_gz = ngz;

  //mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
  mpu.getAcceleration(&ax, &ay, &az);
  mpu.getRotation(&gx, &gy, &gz);

  /* what a and g do
    g values give the speed at which direction it is traveling
    when you quick move (rotation or otherwise) it will sharply increase in values for a short period of time

    a values give more "rotational" values when you put it at certain angles it makes the numbers largely neg or pos
  */

  if((ax + ay + az + gx + gy + gz) == 0)
  {
    //Serial.println("Retrying 2");
    mpu.initialize();
    mpu.setXAccelOffset(0); //Set your accelerometer offset for axis X
    mpu.setYAccelOffset(0); //Set your accelerometer offset for axis Y
    mpu.setZAccelOffset(0); //Set your accelerometer offset for axis Z
    mpu.setXGyroOffset(0);  //Set your gyro offset for axis X
    mpu.setYGyroOffset(0);  //Set your gyro offset for axis Y
    mpu.setZGyroOffset(0);  //Set your gyro offset for axis Z
  }

  int nx = normalize_a(ax);
  int ny = normalize_a(ay);
  int nz = normalize_a(az);
  ngx = normalize_g(gx);
  ngy = normalize_g(gy);
  ngz = normalize_g(gz);

  pre_ax = nx;
  pre_ay = ny;
  pre_az = nz;

/*
  // if (ngx < 0 && ngy >= 0 && ngz < 0)
  // {
  //   Serial.println("thrust forward");
  // }
  
  // if (ngx == 1 && ngy != 2)
  // {
  //   Serial.println("pull back");
  // }

  // if (pre_gx == 3 && (ngx == 2 || ngx == 1))
  // {
  //   //Serial.println("forward");
  //   write_num = 5;
  // }
  // else if (pre_gx == -3 && (ngx == -2 || ngx == -1))
  // {
  //   //Serial.println("backward");
  //   write_num = 6;
  // }
  
  
  //Serial.write(write_num);

  // Serial.print(nx); Serial.print("\t");
  // Serial.print(ny); Serial.print("\t");
  // Serial.println(nz); // Seril.print("\t");
*/

  // MPU data print
  Serial.print(String(ngx));
  Serial.print(",");
  Serial.print(String(ngy));
  Serial.print(",");
  Serial.print(String(ngz));
  Serial.print(",");

  // joystick data pring
  Serial.print(jrx);
  Serial.print(",");
  Serial.print(jry);
  Serial.print(",");
  Serial.print(!digitalRead(SW));
  Serial.print(",");

  // a values
  Serial.print(nz);
  Serial.print("\n");

  Serial.flush();
  delay(1);
  
}

int normalize_a(int a_num)
{
  int new_a_num = a_num/1000;
  return new_a_num;
}

int normalize_g(int g_num)
{
  int new_g_num = g_num/10000;
  new_g_num *= -1;
  return new_g_num;
}

