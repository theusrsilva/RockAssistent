On a Mac, the radio is always an **ESP32** on USB. macOS doesn't give the low-level Wi-Fi access the
Switch's local wireless needs, so the Mac's Wi-Fi card doesn't work, and you **don't need** any
special network card.

**USB port:** the ESP32 shows up as `/dev/cu.usbmodem…` (boards on **native USB**, like the ESP32-S3
on its "USB" port or the ESP32-C3) or `/dev/cu.usbserial…` (boards with a **CP210x** or **CH340**
converter). On recent macOS versions these converters usually work without installing anything; if the
port doesn't show up, install the driver from the manufacturer's official page (Silicon Labs for the
CP210x, WCH for the CH340). If the Mac only has USB-C, use a cable or adapter that carries data, not a
charge-only cable.
