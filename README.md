# Snapshot

## SI - Set 1

<p align="center">
  <img src="https://user-images.githubusercontent.com/73041886/162007995-de799515-409a-4ec0-8a11-b6baad338c48.png" alt="snapshot-logo"/>
</p>


------------


## Introduction

Snapshot is a Windows application that can be used to take photos and record videos on devices with one or multiple cameras. If these actions are event-triggered, the cameras turn on automatically when the specified event occurs, and store the recordings or photos on the local hard drive. The software supports both IP and USB cameras, but it was designed predominantly to be used with 1-3 USB cameras. It's recommended that the software is used on Windows 7 x86 or Windows 10 x64. Snapshot can be used offline as a desktop application, or accessed through our web application. The app is relatively lightweight, and it doesn't consume too many resources, which allows it to run alongside other applications. The device where the app is installed can be connected to the [web app](https://github.com/icatic1/SI-Grupa2 "web app"). This is enabled by setting the Server IP address, media path and JSON configuration path.
The web app device can then be synchronized with the local version.

------------

## Features:

- Installation package
- Silent install/deinstall using command prompt
- Licencing (along with choosing the licensing server source)
- Demo version for unregistered users
- Configuration import and export
- Event-triggered recording (regex and/or motion detection)
- Face detection
- Three different types of captures - video, photo, and burst mode
- Choose which camera you want to use and what resolution you need
- Preview of photos and recordings
- Desktop and web app for both offline and online access
- Recorded files can be synced with a remote server
- You can save your configuration on the remote server as well
- Choose how long you want to keep your captures both locally and on the server
- Tooltips for easier understanding
- Debug log

------------

##Installation:
Download the instalation package from the source corresponding to your OS. The file should be called instal*Version*.zip (ie. *Install64.zip*, *Install32.zip*, etc.). As you have noticed, the instalation package comes in the form of an archive, so you will need to extract it to your desired location.

After extracting, there should be two files at the desired location: *setup.exe* and *setup.msi*, which can be used to install the application.

###Instalation packages:
- Windows 10 x64
- Windows 7 x86

###One click server setup:
- Windows 10
- Linux Ubuntu 20.04

If you have any trouble with the installation, please refer to our [installation guide](https://docs.google.com/document/d/1TrzqUu_w05X0L3bb2tfjRY48pdPEnpBR-CYDOV_qfxQ/edit?usp=sharing "installation guide").


------------
#Contact us

If you have questions about Snapshot, or you would like to reach out to us about an issue you're having or for development advice as you work on a Snapshot issue, you can reach us by opening an issue and prefix the issue title with [Question].
