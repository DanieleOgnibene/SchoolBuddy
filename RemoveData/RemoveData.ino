#include <SPIFFS.h>

void setup() {
  SPIFFS.begin(true);
  SPIFFS.remove("/data.txt");
}

void loop() {
  // put your main code here, to run repeatedly:

}
