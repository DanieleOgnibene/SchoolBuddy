const String DEVICE_NAME_FILE_PATH = "/deviceName.txt";

void RemoveDeviceNameFile() {
  SPIFFS.remove(DEVICE_NAME_FILE_PATH);
}

void UpdateDeviceNameFileContent(String newDeviceName) {
  File file = SPIFFS.open(DEVICE_NAME_FILE_PATH, FILE_WRITE);
  file.print(newDeviceName);
  file.close();
}

String GetDeviceNameFileContent() {
  File file = SPIFFS.open(DEVICE_NAME_FILE_PATH);
  String currentContent = "";
  while (file.available()) {
    const char currentLine = file.read();
    currentContent += currentLine;
  }
  file.close();
  return currentContent;
}
