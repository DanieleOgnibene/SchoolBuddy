const String HISTORY_FILE_PATH = "/history.txt";

void RemoveHistoryFile() {
  SPIFFS.remove(HISTORY_FILE_PATH);
}

void UpdateHistoryFileContent(String sensorAddress, int timeStamp) {
  const String currentContent = GetHistoryFileContent();
  const String newContent = currentContent + sensorAddress + ";" + timeStamp + ";\n";
  File file = SPIFFS.open(HISTORY_FILE_PATH, FILE_WRITE);
  file.print(newContent);
  file.close();
}

String GetHistoryFileContent() {
  File file = SPIFFS.open(HISTORY_FILE_PATH);
  String currentContent = "";
  while (file.available()) {
    const char currentLine = file.read();
    currentContent += currentLine;
  }
  file.close();
  return currentContent;
}
