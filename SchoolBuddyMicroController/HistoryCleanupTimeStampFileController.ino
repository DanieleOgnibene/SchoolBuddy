const String HISTORY_CLEANUP_TIME_STAMP_FILE_PATH = "/historyCleanupTimeStamp.txt";

void RemoveHistoryCleanupTimeStampFile() {
  SPIFFS.remove(HISTORY_CLEANUP_TIME_STAMP_FILE_PATH);
}

void UpdateHistoryCleanupTimeStampFileContent(int newTimeStamp) {
  File file = SPIFFS.open(HISTORY_CLEANUP_TIME_STAMP_FILE_PATH, FILE_WRITE);
  file.print(newTimeStamp);
  file.close();
}

String GetHistoryCleanupTimeStampFileContent() {
  File file = SPIFFS.open(HISTORY_CLEANUP_TIME_STAMP_FILE_PATH);
  String currentContent = "";
  while (file.available()) {
    const char currentLine = file.read();
    currentContent += currentLine;
  }
  file.close();
  return currentContent;
}
