#include <SPIFFS.h>
#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEAdvertisedDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>
#include <Wire.h>
#include "RTClib.h"
#include "Cipher.h"

Cipher * cipher = new Cipher();

const int SERIAL_PORT = 9600;

TaskHandle_t ProximitySensorTask;
TaskHandle_t SerialCommunicationTask;

RTC_DS3231 rtc;
bool isHandlingProximitySensorResults = false;
bool isHandlingSerialCommunication = false;

void setup() {
  InitSerial();
  InitRTC();
  InitSPIFFS();
  InitCipher();
  InitProximitySensorTask();
  InitSerialCommunicationTask();
}

void loop() {}

void InitRTC() {
  if (!rtc.begin()) {
    Serial.println("Couldn't find RTC");
    return;
  }
}

void InitSerial() {
  Serial.begin(SERIAL_PORT);
}

void InitSPIFFS() {
  SPIFFS.begin(true);
}

void InitCipher() {
  char * key = "qwertyuiopasdfgs";
  cipher->setKey(key);
}

void InitProximitySensorTask() {
  xTaskCreatePinnedToCore(
                    ProximitySensorTaskCode,   /* Task function. */
                    "ProximitySensorTask",     /* name of task. */
                    10000,       /* Stack size of task */
                    NULL,        /* parameter of the task */
                    1,           /* priority of the task */
                    &ProximitySensorTask,      /* Task handle to keep track of created task */
                    0);
}

void InitSerialCommunicationTask() {
    xTaskCreatePinnedToCore(
                    SerialCommunicationTaskCode,   /* Task function. */
                    "SerialCommunicationTask",     /* name of task. */
                    10000,       /* Stack size of task */
                    NULL,        /* parameter of the task */
                    1,           /* priority of the task */
                    &SerialCommunicationTask,      /* Task handle to keep track of created task */
                    1);
}
