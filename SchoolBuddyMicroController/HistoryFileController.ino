const String HISTORY_FILE_PATH = "/history.txt";

void RemoveHistoryFile() {
  SPIFFS.remove(HISTORY_FILE_PATH);
}

void UpdateHistoryFileContent(String sensorAddress, int timeStamp) {
  const String currentContent = GetHistoryFileContent();
  const String newContent = currentContent + sensorAddress + ';' + timeStamp + '\n';
  File file = SPIFFS.open(HISTORY_FILE_PATH, FILE_WRITE);
  file.print(newContent);
  file.close();
}

String GetHistoryFileContent() {
  File file = SPIFFS.open(HISTORY_FILE_PATH);
  String currentContent = "";
  while (file.available()) {
    const char currentChar = file.read();
    currentContent += currentChar;
  }
  file.close();
  return currentContent;
}

void RunHistoryFileContentCleanup(int minimumTimeStamp) {
  File file = SPIFFS.open(HISTORY_FILE_PATH);
  String currentContent = "";
  String currentLine = "";
  while (file.available()) {
    const char currentChar = file.read();
    if (currentChar == '\n') {
      int separatorIndex = currentLine.indexOf(';');
      int timeStamp = currentLine.substring(separatorIndex + 1).toInt();
      if (timeStamp > minimumTimeStamp) {
        currentContent += currentLine + currentChar;
      } else {
        currentLine = "";
      }
    } else {
      currentLine += currentChar;
    }
  }
  file.close();
}
