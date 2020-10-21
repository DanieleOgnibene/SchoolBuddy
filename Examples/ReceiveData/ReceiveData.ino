#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>

BLECharacteristic *pCharacteristic;
bool deviceConnected = false;
int txValue = 0;

#define SERVICE_UUID           "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define CHARACTERISTIC_UUID_TX "beb5483e-36e1-4688-b7f5-ea07361b26a8"
#define CHARACTERISTIC_UUID_RX "beb5483e-36e1-4688-b7f5-ea07361b26a9"

class MyServerCallbacks: public BLEServerCallbacks {
  void onConnect(BLEServer* pServer) {
    deviceConnected = true;
  };

  void onDisconnect(BLEServer* pServer) {
    deviceConnected = false;
  };
};

class MyCallbacks: public BLECharacteristicCallbacks {
  void onWrite(BLECharacteristic *pCharacteristic) {
    std::string rxValue = pCharacteristic->getValue();

    if(rxValue.length() > 0) {
      Serial.println("===START=RECEIVE===");
      Serial.print("Received Value: ");

      for (int i = 0; i < rxValue.length(); i++) {
        Serial.print(rxValue[i]);
      }

      Serial.println();

      // Do stuff based on the command received from the app
      if (rxValue.find("1") != -1) {
        Serial.println("Turning ON!");
      } else if (rxValue.find("0") != -1) {
        Serial.println("Turning OFF!");
      }

      Serial.println();
      Serial.println("===END=RECEIVE===");
    }
  };
};

void setup() {
  Serial.begin(9600);

  // Create the BLE Device
  BLEDevice::init("ESP32");

  // Create the BLE Server
  BLEServer *pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());

  // Create the BLE Service
  BLEService *pService = pServer->createService(SERVICE_UUID);

  // Create a BLE Characteristic for transmitting end
  pCharacteristic = pService->createCharacteristic(
                      CHARACTERISTIC_UUID_TX,
                      BLECharacteristic::PROPERTY_NOTIFY
                    );

  // BLE2902 needed to notify
  pCharacteristic->addDescriptor(new BLE2902());

  // Create a BLE Characteristic for receiving end
  pCharacteristic = pService->createCharacteristic(
                      CHARACTERISTIC_UUID_RX,
                      BLECharacteristic::PROPERTY_WRITE
                    );

  pCharacteristic->setCallbacks(new MyCallbacks());

  // Start the service
  pService->start();

  // Start advertisting
  pServer->getAdvertising()->start();
  Serial.println("Waiting for a client connection to notify...");
}

void loop() {
  if (deviceConnected) {
    txValue = random(-10, 20);

    // Conversion of txValue
    char txString[8];
    dtostrf(txValue, 1, 2, txString);

    // Setting the value to the characteristic
    pCharacteristic->setValue(txString);

    pCharacteristic->notify();
    Serial.println("Sent value: " + String(txString));
    delay(500);
  }
}
