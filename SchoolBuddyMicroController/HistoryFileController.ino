void RemoveHistoryFiles() {
  for (int fileIndex = 0; fileIndex < 15; fileIndex++) {
    SPIFFS.remove(GetHistoryFilePath(fileIndex));
  }
}

void UpdateHistoryFileContent(String sensorAddressesWithTimeStamp[], int sensorAddressesWithTimeStampLength) {
  String newContent = "";
  for (int i = 0; i < sensorAddressesWithTimeStampLength; i++) {
    newContent += sensorAddressesWithTimeStamp[i] + '\n';
  }
  File file = SPIFFS.open(GetHistoryFilePath(0), FILE_APPEND);
  file.print(newContent);
  file.close();
}

String GetCurrentHistoryFileContent() {
  return GetHistoryFileContent(0);
}

String GetHistoryFileContent(int fileIndex) {
  const String historyFilePath = GetHistoryFilePath(fileIndex);
  String currentContent = "";
  if (SPIFFS.exists(historyFilePath) != 0) {
    File file = SPIFFS.open(historyFilePath);
    while (file.available()) {
      const char currentChar = file.read();
      currentContent += currentChar;
    }
    file.close();
  }
  return currentContent;
}

void RunHistoryFileContentCleanup() {
  SPIFFS.remove(GetHistoryFilePath(14));
  for (int fileIndex = 13; fileIndex >= 0; fileIndex--) {
    SPIFFS.rename(GetHistoryFilePath(fileIndex), GetHistoryFilePath(fileIndex + 1));
  }
}

String GetHistoryFilePath(int fileIndex) {
  const String baseHistoryPath = "/history_";
  const String fileExtension = ".txt";
  return baseHistoryPath + fileIndex + fileExtension;
}
