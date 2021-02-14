#include <AESLib.h>

void setup() {
  Serial.begin(9600);
  uint8_t key[] = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
  uint8_t iv[] =  {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15};
  char data[]= "0123456701234567012345670123456701234567012345670123456701234567"
                "0123456701234567012345670123456701234567012345670123456701234567"
                "0123456701234567012345670123456701234567012345670123456701234567"
                "0123456701234567012345670123456701234567012345670123456701234567"
                "01234567012345670123456701234567&^%*($#%$%^T&*)(I)__~!@#$%^&*())";

  Serial.print("Data length (bytes) = ");
  Serial.println(strlen(data));
  Serial.println();
 
  Serial.println("Orig Data:");
  Serial.println(data);
  Serial.println();
 
  aes128_cbc_enc(key, iv, data, 320);
  Serial.println("encrypted:");
  Serial.println(data);
  Serial.println();
 
  aes128_cbc_dec(key, iv, data, 320);
  Serial.println("decrypted:");
  Serial.println(data); 
}

void loop() {}
