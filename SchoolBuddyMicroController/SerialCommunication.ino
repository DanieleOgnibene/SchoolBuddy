const char END_COMMAND_CHAR = '#';
const char COMMAND_PARAMTERS_SEPARATOR = ';';
const String COMMAND_OK_RESPONSE = "OK";
const String COMMAND_KO_RESPONSE = "KO";
const String GET_HISTORY_COMMAND = "GET_HISTORY_";
const String GET_DEVICE_NAME_COMMAND = "GET_DEVICE_NAME_";
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
    if (inChar == END_COMMAND_CHAR) {
      serialCommandReceived = true;
    }
  }
}

void ReadSerialCommand() {
  if (!isHandlingProximitySensorResults && serialCommandReceived) {
    HandleSerialCommand();
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
  if (serialInput.startsWith(GET_DEVICE_NAME_COMMAND)) {
    HandleGetDeviceNameCommand();
  }
  serialInput = "";
  serialCommandReceived = false;
}

void HandleGetHistoryCommand() {
  Serial.print(GET_HISTORY_COMMAND);
  Serial.print(GetHistoryFileContent());
  Serial.print(END_COMMAND_CHAR);
}

void HandleResetDeviceCommand() {
  ResetDevice();
  Serial.print(RESET_DEVICE_COMMAND);
  Serial.print(COMMAND_OK_RESPONSE);
  Serial.print(END_COMMAND_CHAR);
}

void HandleInitDeviceCommand() {
  ResetDevice();
  InitDevice(); 
  Serial.print(INIT_DEVICE_COMMAND);
  Serial.print(COMMAND_OK_RESPONSE);
  Serial.print(END_COMMAND_CHAR);
}

void HandleGetDeviceNameCommand() {
  Serial.print(GET_DEVICE_NAME_COMMAND);
  Serial.print(GetDeviceNameFileContent());
  Serial.print(END_COMMAND_CHAR);
}

void ResetDevice() {
  RemoveHistoryFile();
  RemoveDeviceNameFile();
}

void InitDevice() {
  String configuration = serialInput.substring(INIT_DEVICE_COMMAND.length(), serialInput.length() - 1);
  int splitIndex = configuration.indexOf(COMMAND_PARAMTERS_SEPARATOR);
  int unixTime = configuration.substring(0, splitIndex).toInt();
  String deviceName = configuration.substring(splitIndex + 1);
  InitRtc(unixTime);
  UpdateDeviceNameFileContent(deviceName);
}

void InitRtc(int unixTime) {
  rtc.adjust(DateTime(unixTime));
}
