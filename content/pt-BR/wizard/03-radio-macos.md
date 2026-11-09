No Mac, o rádio é sempre um **ESP32** ligado no USB. O macOS não dá o acesso de baixo nível ao Wi-Fi
que o wireless local do Switch exige, então a placa Wi-Fi do Mac não serve, e você **não precisa** de
nenhuma placa de rede especial.

**Porta USB:** o ESP32 aparece como `/dev/cu.usbmodem…` (placas pela **USB nativa**, como o ESP32-S3
pela porta "USB" ou o ESP32-C3) ou `/dev/cu.usbserial…` (placas com conversor **CP210x** ou
**CH340**). Nas versões recentes do macOS, esses conversores costumam funcionar sem instalar nada; se
a porta não aparecer, instale o driver da página oficial do fabricante (Silicon Labs para o CP210x,
WCH para o CH340). Se o Mac só tem USB-C, use um cabo ou adaptador que passe dados, não um cabo só de
carga.
