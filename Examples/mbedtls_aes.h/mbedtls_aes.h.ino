#include "mbedtls/aes.h"

void encryptLongText(char * plainText, char * key, unsigned char * outputBuffer) {
  const int blockLength = 16;
  const int plainTextLength = strlen(plainText);
  char block[blockLength];
  int blockIndex = 0;
  int outputBufferIndex = 0;
  for (int i = 0; i < plainTextLength; i++) {
    if (blockIndex == blockLength) {
      unsigned char encryptedBlock[blockLength];
      encrypt(block, key, encryptedBlock);
      strcat((char *)outputBuffer, (char *)encryptedBlock);
      blockIndex = 0;
    }
    block[blockIndex] = plainText[i];
    blockIndex++;
  }
  if (blockIndex == blockLength) {
    unsigned char encryptedBlock[blockLength];
    encrypt(block, key, encryptedBlock);
    strcat((char *)outputBuffer, (char *)encryptedBlock);
  } else {
    for (int i = blockIndex; i < blockLength; i++) {
      block[blockIndex] = ' ';
    }
    unsigned char encryptedBlock[blockLength];
    encrypt(block, key, encryptedBlock);
    strcat((char *)outputBuffer, (char *)encryptedBlock);
  }
}

void decryptLongText(unsigned char * chipherText, char * key, unsigned char * outputBuffer) {
  const int blockLength = 16;
  const int plainTextLength = strlen((char *)chipherText);
  unsigned char block[blockLength];
  int blockIndex = 0;
  int outputBufferIndex = 0;
  for (int i = 0; i < plainTextLength; i++) {
    if (blockIndex == blockLength) {
      unsigned char encryptedBlock[blockLength];
      decrypt(block, key, encryptedBlock);
      strcat((char *)outputBuffer, (char *)encryptedBlock);
      blockIndex = 0;
    }
    block[blockIndex] = chipherText[i];
    blockIndex++;
  }
  if (blockIndex == blockLength) {
    unsigned char encryptedBlock[blockLength];
    decrypt(block, key, encryptedBlock);
    strcat((char *)outputBuffer, (char *)encryptedBlock);
  } else {
    for (int i = blockIndex; i < blockLength; i++) {
      block[blockIndex] = ' ';
    }
    unsigned char encryptedBlock[blockLength];
    decrypt(block, key, encryptedBlock);
    strcat((char *)outputBuffer, (char *)encryptedBlock);
  }
}

void encrypt(char * plainText, char * key, unsigned char * outputBuffer) {
  mbedtls_aes_context aes;
  mbedtls_aes_init( &aes );
  mbedtls_aes_setkey_enc( &aes, (const unsigned char*) key, strlen(key) * 8 );
  mbedtls_aes_crypt_ecb( &aes, MBEDTLS_AES_ENCRYPT, (const unsigned char*)plainText, outputBuffer);
  mbedtls_aes_free( &aes );
}

void decrypt(unsigned char * chipherText, char * key, unsigned char * outputBuffer) {
  mbedtls_aes_context aes;
  mbedtls_aes_init( &aes );
  mbedtls_aes_setkey_dec( &aes, (const unsigned char*) key, strlen(key) * 8 );
  mbedtls_aes_crypt_ecb(&aes, MBEDTLS_AES_DECRYPT, (const unsigned char*)chipherText, outputBuffer);
  mbedtls_aes_free( &aes );
}

void setup() {

  Serial.begin(9600);

  delay(1000);

  char test[10];
  strcat(test, ";");
  Serial.println(test);

  char * key = "abcdefghijklmnop";

  char *plainText = "12345678123456781234567812345678";
  const int textLength = strlen(plainText);
  
  unsigned char cipherTextOutput[textLength];
  unsigned char decipheredTextOutput[textLength];

  encryptLongText(plainText, key, cipherTextOutput);
  decryptLongText(cipherTextOutput, key, decipheredTextOutput);

  Serial.println("\n\nDeciphered text:");
  for (int i = 0; i < 16; i++) {
    Serial.print((char)decipheredTextOutput[i]);
  }
}

void loop() {}
