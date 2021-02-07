#include "Cipher.h"

#define CIPHER_DEBUG = false;

Cipher * cipher = new Cipher();

void setup() {
  Serial.begin(9600);
}

void loop() {
  char * key = "qwertyuiopasdf12";
  cipher->setKey(key);
  String data = "126a738b8k9sdsdsadsadadsadsa3";
  String cipherString = cipher->encryptString(data);
  String decipheredString = cipher->decryptString(cipherString);
  Serial.println(cipherString);
  Serial.println(decipheredString);
  delay(1000);
}
