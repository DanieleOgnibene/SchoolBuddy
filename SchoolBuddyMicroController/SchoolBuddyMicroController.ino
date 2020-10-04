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
const int SCAN_INTERVAL = 10000;
const int NUM_CICLES_TO_SAVE = 3;
const char* PUBLIC_DEVICE_NAME = "SchoolBuddy";
const String HISTORY_FILE_PATH = "/history.txt";
const char* GET_NUMBER_OF_SECTIONS = "GET_NUMBER_OF_SECTIONS";
const char* GET_SECTION = "GET_SECTION_";
const int MAX_BLE_MESSAGE_BYTES = 20;

BLECharacteristic *pCharacteristic;
BLEScan* pBLEScan;

void UpdateHistoryFileContent(String sensorAddress) {
  const String currentContent = GetCurrentHistoryFileContent();
  const String newContent = currentContent + sensorAddress + ";";
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

void SendHistorySection(int sectionIndex) {
  Serial.println("SENDIND HISTORY:");
  const String historyData = GetCurrentHistoryFileContent();
  const String message = GetMessageFragment(historyData, sectionIndex);
  SendMessage(message);
};

String GetMessageFragment(String data, int index) {
  const int dataLength = data.length();
  if (dataLength == 0) {
    return data;
  }
  const int startIndex = index * MAX_BLE_MESSAGE_BYTES;
  const int endIndex = startIndex + MAX_BLE_MESSAGE_BYTES;
  const int maxIndex = dataLength;
  return data.substring(startIndex >= 0 ? startIndex : 0, endIndex > maxIndex ? maxIndex : endIndex);
};

void SendMessage(String message) {
  const int responseLength = message.length() + 1;
  char response[responseLength];
  message.toCharArray(response, responseLength);
  pCharacteristic->setValue(response);
  pCharacteristic->notify();
  Serial.println(response);
}

class MyBLECharacteristicCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
      std::string rxValue = pCharacteristic->getValue();

      if (rxValue.length() > 0) {
        Serial.println("===START=RECEIVE===");
        Serial.print("Received Value: ");

        for (int i = 0; i < rxValue.length(); i++) {
          Serial.print(rxValue[i]);
        }

        Serial.println();

        if (rxValue.find(GET_NUMBER_OF_SECTIONS) != -1) {
          const String historyData = GetCurrentHistoryFileContent();
          const int dataLength = historyData.length();
          const int numberOfSections = floor(dataLength / MAX_BLE_MESSAGE_BYTES);
          SendMessage((String)numberOfSections);
        } else if (rxValue.find(GET_SECTION) != -1) {
          String sectionNumber = "";
          for (int i = strlen(GET_SECTION); i < rxValue.length(); i++) {
            sectionNumber += rxValue[i];
          }
          bool isValidDigit = true;
          for (int i = 0; i < sectionNumber.length(); i++) {
            const bool isCurrentADigit = isDigit(sectionNumber.charAt(i));
            if (!isCurrentADigit) {
              isValidDigit = false;
            }
          }
          if (isValidDigit) {
            SendHistorySection(sectionNumber.toInt()); 
          }
        } else {
          Serial.println("REFUSED");
        }

        Serial.println();
        Serial.println("===END=RECEIVE===");
      }
    };
};

void setup() {
  InitSerial();
  InitSPIFFS();
  InitLed();
  InitBLEDevice();
  InitBLEScan();
  InitBLEDataCommunication();
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
}

void InitBLEScan() {
  pBLEScan = BLEDevice::getScan();
  pBLEScan->setActiveScan(true);
  pBLEScan->setInterval(100);
  pBLEScan->setWindow(99);
}

void InitBLEDataCommunication() {
  BLEServer *pServer = BLEDevice::createServer();

  BLEService *pService = pServer->createService(SERVICE_UUID);

  pCharacteristic = pService->createCharacteristic(
                      CHARACTERISTIC_UUID,
                      BLECharacteristic::PROPERTY_NOTIFY |
                      BLECharacteristic::PROPERTY_WRITE
                    );
  pCharacteristic->addDescriptor(new BLE2902());
  pCharacteristic->setCallbacks(new MyBLECharacteristicCallbacks());

  pService->start();
  pServer->getAdvertising()->start();
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

void PrintSensorData(String sensorName, String sensorAddress, int sensorRssi) {
  Serial.println("----------------------");
  Serial.println("New SchoolBuddy found!");
  Serial.println("Mac: " + sensorAddress + " --- " + "Name: " + sensorName + " --- " + "RSSI: " + sensorRssi);
  Serial.println("----------------------");
}
