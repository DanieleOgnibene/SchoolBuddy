#include "Cipher.h"

Cipher * cipher = new Cipher();

void setup() {
  Serial.begin(9600);
  char * key = "testkey";
  cipher->setKey(key);
  String data = "126a738b8k93";
  String cipherString = cipher->encryptString(data);
  String decipheredString = cipher->decryptString(cipherString);
  Serial.println(cipherString);
  Serial.println(cipherString.equals(cipherString));
  Serial.println(decipheredString);
}

void loop() {
}
