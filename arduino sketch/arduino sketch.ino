String i;
#include <UTFTGLUE.h>              //use GLUE class and constructor
UTFTGLUE myGLCD(0, A2, A1, A3, A4, A0); //all dummy args
bool hshake_status = false;
void setup()
{
	Serial.begin(9600);
	pinMode(13, OUTPUT);
	digitalWrite(13, 0);
	myGLCD.InitLCD();
	myGLCD.setFont(SmallFont);
	myGLCD.fillScr(0, 0, 0);
}

void loop()
{
	Serial.println("before");
	if (hshake_status == false) {
		hshake();
	}
	else
	{
		myGLCD.setCursor(50, 50);
		//myGLCD.setRotation(0);
		myGLCD.setTextSize(4);
		myGLCD.println("PEEPEE");
	}
}

void hshake() {
	if (Serial.available() > 0) {
		i = Serial.readStringUntil('#');
		if (i == "ready?") {
			myGLCD.setCursor(0, 0);
			myGLCD.setTextSize(4);
			myGLCD.println("Handshake    Established");
			Serial.write("ready#");
			hshake_status = true;
			return;
		}
	}
}
