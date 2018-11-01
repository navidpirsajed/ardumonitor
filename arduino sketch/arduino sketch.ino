String i;
int ii;
#include <Adafruit_GFX.h>// Hardware-specific library
#include <MCUFRIEND_kbv.h>
#include <MemoryFree.h>
#include <FreeDefaultFonts.h>

MCUFRIEND_kbv tft;

#define LCD_CS A3 // Chip Select goes to Analog 3
#define LCD_CD A2 // Command/Data goes to Analog 2
#define LCD_WR A1 // LCD Write goes to Analog 1
#define LCD_RD A0 // LCD Read goes to Analog 0
#define LCD_RESET A4 // Can alternately just connect to Arduino's reset pin

#define	BLACK   0x0000
#define	BLUE    0x001F
#define	RED     0xF800
#define	GREEN   0x07E0
#define CYAN    0x07FF
#define MAGENTA 0xF81F
#define YELLOW  0xFFE0
#define WHITE   0xFFFF

void setup()
{
    Serial.begin(9600);
    uint16_t ID = tft.readID();
    if (ID == 0xD3) ID = 0x9481;
    tft.begin(ID);
    tft.setRotation(0);
    tft.fillScreen(BLACK);
}

void loop()
{
    if (Serial.available() > 0) {
        i = Serial.readStringUntil('$');
        tft.setTextColor(WHITE, BLACK);
        tft.setTextSize(2);
        tft.setCursor(0, 0);
        tft.print(freeMemory());
        tft.setCursor(70, 0);
        tft.print(ii);
        tft.setCursor(0, 25);
        tft.print(i);
        ii++;
    }
    //myGLCD.fillScr(0, 0, 0);
}