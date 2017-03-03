# HoloLensQr

This is a small HoloLens app that can read QR codes and from a server (currently hardcoded) load the corresponding menu. The QR codes needs to have the prefix "qrint:" as some kind of protocol.


To read QR codes it uses the following libraries:
* http://zxingnet.codeplex.com/
* https://github.com/mtaulty/QrCodes

Image for LED from:
* https://commons.wikimedia.org/wiki/File:RBG-LED.jpg

## Setup

Setup without server (on Cybercom Gothenburg)
* Clone or download all files (server catalog not needed)
* Open directory in Unity
* Build in Unity and compile/deploy in Visual Studio

Setup with server (outside Cybercom Gothenburg)
* Clone or download all files
* Copy the server files to your server
* Start server.py with python (and possibly also blinkserver.py)
* Open directory in Unity
* Change ip in "/Assets/Scripts/Backend.cs" to your server address
* Build in Unity and compile/deploy in Visual Studio

Unity settings:
* Build settings -> Plattform -> Windows store
* Build settings -> SDK -> Universal 10
* Build settings -> UWP Build Type -> D3D
* Build settings -> Unity C# Projects -> Check
* Build settings -> Build
* Create and select subfolder (App)

VS settings:
* Open the project in your subfolder (the one in App)
* Change Debug -> Release
* Change ARM -> x86
* Change Local machine -> Remote machine
* Start (without) debugging

## Demoversion
Demoversion can be activated by setting demomode to true in "/Assets/Scripts/QrScan.cs". It excludes scanning wich is needed if you are recording or showing content from the webcam. To fetch menus when scanning, just airtap anywhere.
