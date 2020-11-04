#include <SPIFFS.h>
#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEAdvertisedDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>

#define SERVICE_UUID        "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define CHARACTERISTIC_UUID "beb5483e-36e1-4688-b7f5-ea07361b26a8"

const int LED = 2;
const int MIN_QUALITY = -60;
const int SCAN_INTERVAL = 2000;
const char* PUBLIC_DEVICE_NAME = "SchoolBuddy";
const String HISTORY_FILE_PATH = "/history.txt";
const String GET_HISTORY_COMMAND = "GET_HISTORY#";

BLEScan* pBLEScan;
String serialInput = "";
bool serialCommandReceived = false;

void setup() {
  InitSerial();
  InitSPIFFS();
  InitLed();
  InitBLEDevice();
  InitBLEScan();
}

void loop() {
  pBLEScan->start(1, OnScanResults, false);
  pBLEScan->clearResults();
  CheckSerialInput();
  delay(SCAN_INTERVAL);
}

void CheckSerialInput() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();
    serialInput += inChar;
    if (inChar == '#') {
      serialCommandReceived = true;
    }
  }
  if (serialCommandReceived) {
    HandleSerialCommand();
  }
}

void InitSerial() {
  Serial.begin(9600);
}

void InitSPIFFS() {
  SPIFFS.begin(true);
}

void InitLed() {
  pinMode(LED, OUTPUT);
}

void InitBLEDevice() {
  BLEDevice::init(PUBLIC_DEVICE_NAME);
}

void InitBLEScan() {
  pBLEScan = BLEDevice::getScan();
  pBLEScan->setActiveScan(true);
  pBLEScan->setInterval(100);
  pBLEScan->setWindow(99);
}

void HandleSerialCommand() {
  if (serialInput.equals(GET_HISTORY_COMMAND)) {
    Serial.print(GetCurrentHistoryFileContent());
    Serial.print('#');
  }
  serialInput = "";
  serialCommandReceived = false;
}

void OnScanResults(BLEScanResults scanResults) {
  int count = scanResults.getCount();
  bool isBuddyFound = false;
  for (int i = 0; i < count; i++) {
    BLEAdvertisedDevice bleSensor = scanResults.getDevice(i);
    String sensorName = bleSensor.getName().c_str();
    String sensorAddress = bleSensor.getAddress().toString().c_str();
    int sensorRssi = bleSensor.getRSSI();
    if (HasToBeSaved(sensorAddress, sensorRssi)) {
      isBuddyFound = true;
      ManageSensorInProximity(sensorName, sensorAddress, sensorRssi);
    }
  }
  digitalWrite(LED, isBuddyFound ? HIGH : LOW);
}

// TODO: deviceName == PUBLIC_DEVICE_NAME
bool HasToBeSaved(String sensorAddress, int sensorRssi) {
  const bool hasEnoughtQuality = sensorRssi > MIN_QUALITY;
  return hasEnoughtQuality && IsNewSensorAddress(sensorAddress);
}

// TODO: Date check
bool IsNewSensorAddress(String sensorAddress) {
  const String currentHistoryFileContent = GetCurrentHistoryFileContent();
  return currentHistoryFileContent.indexOf(sensorAddress) == -1;
}

void ManageSensorInProximity(String sensorName, String sensorAddress, int sensorRssi) {
  PrintSensorData(sensorName, sensorAddress, sensorRssi);
  UpdateHistoryFileContent(sensorAddress);
}

void UpdateHistoryFileContent(String sensorAddress) {
  const String currentContent = GetCurrentHistoryFileContent();
  const String newContent = currentContent + sensorAddress + ";\n";
  File file = SPIFFS.open(HISTORY_FILE_PATH, FILE_WRITE);
  file.print(newContent);
  file.close();
  Serial.println(newContent);
}

String GetCurrentHistoryFileContent() {
  File file = SPIFFS.open(HISTORY_FILE_PATH);
  String currentContent = "";
  while (file.available()) {
    const char currentLine = file.read();
    currentContent += currentLine;
  }
  file.close();
  return currentContent;
}

void PrintSensorData(String sensorName, String sensorAddress, int sensorRssi) {
  Serial.println("----------------------");
  Serial.println("New SchoolBuddy found!");
  Serial.println("Mac: " + sensorAddress + " --- " + "Name: " + sensorName + " --- " + "RSSI: " + sensorRssi);
  Serial.println("----------------------");
}
