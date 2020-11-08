const int LED = 2;
const int RSSI_MIN_VALUE = -60;
const int BLE_SCAN_INTERVAL = 2000;
const char* PUBLIC_DEVICE_NAME = "SchoolBuddy";

BLEScan* pBLEScan;

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
  pBLEScan->start(1, OnScanResults, false);
  pBLEScan->clearResults();
  CheckForHistoryCleanup();
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
  isHandlingProximitySensorResults = true;
  int count = scanResults.getCount();
  bool isBuddyFound = false;
  for (int i = 0; i < count; i++) {
    BLEAdvertisedDevice bleSensor = scanResults.getDevice(i);
    String sensorName = bleSensor.getName().c_str();
    String sensorAddress = bleSensor.getAddress().toString().c_str();
    int sensorRssi = bleSensor.getRSSI();
    int timeStamp = rtc.now().unixtime();
    if (HasToBeSaved(sensorAddress, sensorRssi, timeStamp)) {
      isBuddyFound = true;
      ManageSensorInProximity(sensorAddress, timeStamp);
    }
  }
  isHandlingProximitySensorResults = false;
  digitalWrite(LED, isBuddyFound ? HIGH : LOW);
}

// TODO: deviceName == PUBLIC_DEVICE_NAME
bool HasToBeSaved(String sensorAddress, int sensorRssi, int timeStamp) {
  bool hasEnoughtQuality = sensorRssi > RSSI_MIN_VALUE;
  return hasEnoughtQuality && IsNewSensorAddress(sensorAddress, timeStamp);
}

bool IsNewSensorAddress(String sensorAddress, int timeStamp) {
  String currentHistoryFileContent = GetHistoryFileContent();
  int sensorAddressIndex = currentHistoryFileContent.lastIndexOf(sensorAddress);
  if (sensorAddressIndex == -1) {
    return true;
  }
  int lastTimeStampIndex = sensorAddressIndex + sensorAddress.length() + 1;
  int lastTimeStampEndIndex = currentHistoryFileContent.indexOf(';', lastTimeStampIndex);
  String lastTimeStampString = currentHistoryFileContent.substring(lastTimeStampIndex, lastTimeStampEndIndex);
  int lastTimeStamp = lastTimeStampString.toInt();
  int minimumTimeBetweenSaves = 86400;
  return (timeStamp - minimumTimeBetweenSaves) > lastTimeStamp;
}

void ManageSensorInProximity(String sensorAddress, int timeStamp) {
  UpdateHistoryFileContent(sensorAddress, timeStamp);
}

void CheckForHistoryCleanup() {
  String lastCleanupTimeStamp = GetHistoryCleanupTimeStampFileContent();
  bool hasToCleanup = lastCleanupTimeStamp == "" || (rtc.now().unixtime() - 86400) < lastCleanupTimeStamp.toInt();
  if (hasToCleanup) {
    RunHistoryCleanup();
  }
}

void RunHistoryCleanup() {
  int nowTimeStamp = rtc.now().unixtime();
  int minTimeStamp = nowTimeStamp - 1296000;
  RunHistoryFileContentCleanup(minTimeStamp);
  UpdateHistoryCleanupTimeStampFileContent(nowTimeStamp);
}
