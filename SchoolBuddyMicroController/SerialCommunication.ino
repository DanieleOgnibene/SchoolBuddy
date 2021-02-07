const String END_COMMAND = "END_COMMAND";
const char COMMAND_PARAMTERS_SEPARATOR = ';';
const String COMMAND_OK_RESPONSE = "OK";
const String COMMAND_KO_RESPONSE = "KO";
const String GET_HISTORY_COMMAND = "GET_HISTORY_";
const String GET_DEVICE_INFO_COMMAND = "GET_DEVICE_INFO_";
const String RESET_DEVICE_COMMAND = "RESET_DEVICE_";
const String INIT_DEVICE_COMMAND = "INIT_DEVICE_";
const int SERIAL_COMMUNICATION_INTERVAL = 2000;

String serialInput = "";
bool serialCommandReceived = false;

void SerialCommunicationTaskCode( void * pvParameters ){
  SerialCommunicationSetup();
  for(;;){
    SerialCommunicationLoop();
  } 
}

void SerialCommunicationSetup() {}

void SerialCommunicationLoop() {
  ReadSerialInput();
  ReadSerialCommand();
  delay(SERIAL_COMMUNICATION_INTERVAL);
}

void ReadSerialInput() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();
    serialInput += inChar;
    if (serialInput.endsWith(END_COMMAND)) {
      serialCommandReceived = true;
    }
  }
}

void ReadSerialCommand() {
  if (!isHandlingProximitySensorResults && serialCommandReceived) {
    isHandlingSerialCommunication = true;
    HandleSerialCommand();
    isHandlingSerialCommunication = false;
  }
}

void HandleSerialCommand() {
  if (serialInput.startsWith(GET_HISTORY_COMMAND)) {
    HandleGetHistoryCommand();
  }
  if (serialInput.startsWith(RESET_DEVICE_COMMAND)) {
    HandleResetDeviceCommand();
  }
  if (serialInput.startsWith(INIT_DEVICE_COMMAND)) {
    HandleInitDeviceCommand();
  }
  if (serialInput.startsWith(GET_DEVICE_INFO_COMMAND)) {
    HandleGetDeviceNameCommand();
  }
  serialInput = "";
  serialCommandReceived = false;
}

void HandleGetHistoryCommand() {
  Serial.print(GET_HISTORY_COMMAND);
  for (int fileIndex = 0; fileIndex < 15; fileIndex++) {
    Serial.print(GetHistoryFileContent(fileIndex));
    Serial.print("END_FILE");
  }
  Serial.print(END_COMMAND);
}

void HandleResetDeviceCommand() {
  ResetDevice();
  Serial.print(RESET_DEVICE_COMMAND);
  Serial.print(COMMAND_OK_RESPONSE);
  Serial.print(END_COMMAND);
}

void HandleInitDeviceCommand() {
  ResetDevice();
  InitDevice(); 
  Serial.print(INIT_DEVICE_COMMAND);
  Serial.print(COMMAND_OK_RESPONSE);
  Serial.print(END_COMMAND);
}

void HandleGetDeviceNameCommand() {
  Serial.print(GET_DEVICE_INFO_COMMAND);
  Serial.print(GetDeviceNameFileContent());
  Serial.print(COMMAND_PARAMTERS_SEPARATOR);
  Serial.print(BLEDevice::getAddress().toString().c_str());
  Serial.print(END_COMMAND);
}

void ResetDevice() {
  RemoveHistoryFiles();
  RemoveDeviceNameFile();
  RemoveHistoryCleanupTimeStampFile();
}

void InitDevice() {
  String configuration = serialInput.substring(INIT_DEVICE_COMMAND.length(), serialInput.length() - END_COMMAND.length());
  int splitIndex = configuration.indexOf(COMMAND_PARAMTERS_SEPARATOR);
  int unixTime = configuration.substring(0, splitIndex).toInt();
  String deviceName = configuration.substring(splitIndex + 1);
  InitRtc(unixTime);
  UpdateDeviceNameFileContent(deviceName);
}

void InitRtc(int unixTime) {
  rtc.adjust(DateTime(unixTime));
}
