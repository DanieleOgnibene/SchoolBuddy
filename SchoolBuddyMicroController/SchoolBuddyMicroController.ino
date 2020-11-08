#include <SPIFFS.h>
#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEAdvertisedDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>
#include <Wire.h>
#include "RTClib.h"

const int SERIAL_PORT = 9600;

TaskHandle_t ProximitySensorTask;
TaskHandle_t SerialCommunicationTask;

RTC_DS3231 rtc;
bool isHandlingProximitySensorResults = false;

void setup() {
  InitRTC();
  InitSerial();
  InitSPIFFS();
  InitProximitySensorTask();
  InitSerialCommunicationTask();
}

void loop() {}

void InitRTC() {
  if (!rtc.begin()) {
    Serial.println("Couldn't find RTC");
    return;
  }
  if (rtc.lostPower()) {
    Serial.println("RTC lost power, lets set the time!");
    // Comment out below lines once you set the date & time.
    // Following line sets the RTC to the date & time this sketch was compiled
    // rtc.adjust(DateTime(F(__DATE__), F(__TIME__)));
  
    // Following line sets the RTC with an explicit date & time
    // for example to set January 27 2017 at 12:56 you would call:
    // rtc.adjust(DateTime(2017, 1, 27, 12, 56, 0));
  }
}

void InitSerial() {
  Serial.begin(SERIAL_PORT);
}

void InitSPIFFS() {
  SPIFFS.begin(true);
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
