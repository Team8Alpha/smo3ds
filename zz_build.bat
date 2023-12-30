@echo off
echo Super Mario Odyssey 3DS demake,  Scripts by Moddimation
echo ACTION: BUILD GAME
echo =------------------------------------------------------------=
echo Note: Output is printed once building has finished. Check LOG.txt
C:\Nintendo\UnityN3DS_2.0.1\Editor\Unity.exe -quit -batchmode -projectPath="C:\Users\user\Documents\MODDY\smo3ds" -executeMethod "BuildCommand.PerformBuild" -buildTarget "N3DS" -logFile LOG.txt
powershell -Command "Get-Content 'LOG.txt' -Wait"
