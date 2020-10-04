#include <SPIFFS.h>
#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEAdvertisedDevice.h>

const int LED = 2;
const int MIN_QUALITY = -60;
const int SCAN_INTERVAL = 10000;
const int NUM_CICLES_TO_SAVE = 3;
const char* PUBLIC_DEVICE_NAME = "SchoolBuddy";
const String DATA_FILE_PATH = "/data.txt";

BLEScan* pBLEScan;

void setup() {
  InitSerial();
  InitSPIFFS();
  InitLed();
  InitBLEDevice();
}

void loop() {
  pBLEScan->start(1, OnScanResults, false);
  pBLEScan->clearResults();
  delay(SCAN_INTERVAL);
}

void InitSerial() {
  Serial.begin(115200);
}

void InitSPIFFS() {
  SPIFFS.begin(true);
}

void InitLed() {
  pinMode(LED, OUTPUT);
}

void InitBLEDevice() {
  BLEDevice::init(PUBLIC_DEVICE_NAME);
  BLEDevice::startAdvertising();
  pBLEScan = BLEDevice::getScan();
  pBLEScan->setActiveScan(true);
  pBLEScan->setInterval(1);
  pBLEScan->setWindow(1);
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
  const String currentDataFileContent = GetCurrentDataFileContent();
  return currentDataFileContent.indexOf(sensorAddress) == -1;
}

void ManageSensorInProximity(String sensorName, String sensorAddress, int sensorRssi) {
  PrintSensorData(sensorName, sensorAddress, sensorRssi);
  UpdateDataFileContent(sensorAddress);
}

void PrintSensorData(String sensorName, String sensorAddress, int sensorRssi) {
  Serial.println("----------------------");
  Serial.println("New SchoolBuddy found!");
  Serial.println("Mac: " + sensorAddress + " --- " + "Name: " + sensorName + " --- " + "RSSI: " + sensorRssi);
  Serial.println("----------------------");
}

void UpdateDataFileContent(String sensorAddress) {
  const String currentContent = GetCurrentDataFileContent();
  const String newContent = currentContent + sensorAddress + ";\n";
  File file = SPIFFS.open(DATA_FILE_PATH, FILE_WRITE);
  file.print(newContent);
  file.close();
  Serial.println(newContent);
}

String GetCurrentDataFileContent() {
  File file = SPIFFS.open(DATA_FILE_PATH);
  String currentContent = "";
  while (file.available()) {
    const char currentLine = file.read();
    currentContent += currentLine;
  }
  file.close();
  return currentContent;
}
