const int LED = 2;
const int RSSI_MIN_VALUE = -60;
const int EXPOSURE_SCAN_NUMBER = 1; // Number of scans in which the device must appear in order to be considered
const int BLE_SCAN_INTERVAL = 2000; // Time in milliseconds between each scan
const int MAX_NUMBER_OF_DEVICES_IN_RANGE = 10; // Maximum number of devices that can have an RSSI higher than the minimum threshold in a single scan
const char* PUBLIC_DEVICE_NAME = "SchoolBuddy";

BLEScan* pBLEScan;

struct AddressWithExposure {
  String address;
  int exposure;
};
const int exposuresMaxLength = MAX_NUMBER_OF_DEVICES_IN_RANGE;
struct AddressWithExposure exposures[exposuresMaxLength];
int exposuresLength = 0;

const int sensorAddressesToBeSavedMaxLength = MAX_NUMBER_OF_DEVICES_IN_RANGE;
String sensorAddressesToBeSaved[sensorAddressesToBeSavedMaxLength];
int sensorAddressesToBeSavedLength = 0;
int sensorAddressesToBeSavedFreeIndex = 0;

void ProximitySensorTaskCode( void * pvParameters ){
  ProximitySensorSetup();
  for(;;){
    ProximitySensorLoop();
  } 
}

void ProximitySensorSetup() {
  InitLed();
  InitBLEDevice();
  InitBLEScan();
}

void ProximitySensorLoop() {
  CheckForHistoryCleanup();
  pBLEScan->start(1, OnScanResults, false);
  pBLEScan->clearResults();
  delay(BLE_SCAN_INTERVAL);
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

void OnScanResults(BLEScanResults scanResults) {
  if (isHandlingSerialCommunication) {
    return;
  }
  isHandlingProximitySensorResults = true;
  int count = scanResults.getCount();
  bool isBuddyFound = false;
  struct AddressWithExposure newExposures[exposuresMaxLength];
  int newExposuresFreeIndex = 0;
  int newExposuresLength = 0;
  for (int i = 0; i < count; i++) {
    BLEAdvertisedDevice bleSensor = scanResults.getDevice(i);
    String sensorName = bleSensor.getName().c_str();
    String sensorAddress = bleSensor.getAddress().toString().c_str();
    int sensorRssi = bleSensor.getRSSI();
    if (sensorRssi > RSSI_MIN_VALUE) {
      int currentExposureValue = 1;
      for (int index = 0; index < exposuresLength; index++) {
        struct AddressWithExposure currentElement = exposures[index];
        if (currentElement.address == sensorAddress) {
          currentExposureValue += currentElement.exposure;
          break;
        }
      }
      if (HasToBeSaved(sensorName, currentExposureValue)) {
        sensorAddressesToBeSaved[sensorAddressesToBeSavedFreeIndex] = sensorAddress;
        if (sensorAddressesToBeSavedLength < sensorAddressesToBeSavedMaxLength) {
          sensorAddressesToBeSavedLength++;
          sensorAddressesToBeSavedFreeIndex++;
        }
      }
      if (currentExposureValue < EXPOSURE_SCAN_NUMBER) {
        struct AddressWithExposure updatedAddressWithExposure = { .address = sensorAddress, .exposure = currentExposureValue };
        newExposures[newExposuresFreeIndex] = updatedAddressWithExposure;
        if (newExposuresLength < exposuresMaxLength) {
          newExposuresLength++;
          newExposuresFreeIndex++;
        }
      }
    }
  }
  for (int index = 0; index < newExposuresLength; index++) {
    exposures[index] = newExposures[index];
  }
  if (sensorAddressesToBeSavedLength > 0) {
    String newSensorAddressesWithTimeStampToBeSaved[sensorAddressesToBeSavedLength];
    int newSensorAddressesWithTimeStampToBeSavedLength = 0;
    String currentHistoryFileContent = GetCurrentHistoryFileContent();
    int nowTimeStamp = rtc.now().unixtime();
    for (int index = 0; index < sensorAddressesToBeSavedLength; index++) {
      const String currentSensorAddress = sensorAddressesToBeSaved[index];
      if (currentHistoryFileContent.indexOf(currentSensorAddress) == -1){
        newSensorAddressesWithTimeStampToBeSaved[newSensorAddressesWithTimeStampToBeSavedLength] = currentSensorAddress + ";" + nowTimeStamp;
        newSensorAddressesWithTimeStampToBeSavedLength++;
      }
    }
    sensorAddressesToBeSavedLength = 0;
    sensorAddressesToBeSavedFreeIndex = 0;
    if (newSensorAddressesWithTimeStampToBeSavedLength > 0) {
      isBuddyFound = true;
      UpdateHistoryFileContent(newSensorAddressesWithTimeStampToBeSaved, newSensorAddressesWithTimeStampToBeSavedLength); 
    }
  }
  exposuresLength = newExposuresLength;
  isHandlingProximitySensorResults = false;
  digitalWrite(LED, isBuddyFound ? HIGH : LOW);
}

// TODO: deviceName == PUBLIC_DEVICE_NAME
bool HasToBeSaved(String sensorName, int exposure) {
  return EXPOSURE_SCAN_NUMBER <= exposure;
}

void CheckForHistoryCleanup() {
  String lastCleanupTimeStamp = GetHistoryCleanupTimeStampFileContent();
  bool hasToCleanup = lastCleanupTimeStamp == "" || GetIfHistoryHasToBeCleanup(lastCleanupTimeStamp.toInt());
  if (hasToCleanup) {
    RunHistoryCleanup();
  }
}

bool GetIfHistoryHasToBeCleanup(int lastCleanupTimeStamp) {
  DateTime lastCleanupDateTime = DateTime(lastCleanupTimeStamp);
  DateTime currentDateTime = DateTime(rtc.now().unixtime());
  return lastCleanupDateTime.year() < currentDateTime.year() ||
    (
      lastCleanupDateTime.year() == currentDateTime.year() &&
      lastCleanupDateTime.month() < currentDateTime.month()
    ) ||
    (
      lastCleanupDateTime.year() == currentDateTime.year() &&
      lastCleanupDateTime.month() == currentDateTime.month() &&
      lastCleanupDateTime.day() < currentDateTime.day()
    );
}

void RunHistoryCleanup() {
  RunHistoryFileContentCleanup();
  UpdateHistoryCleanupTimeStampFileContent(rtc.now().unixtime());
}
