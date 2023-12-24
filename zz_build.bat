@echo off
echo Super Mario Odyssey 3DS demake,  Scripts by Moddimation
echo ACTION: BUILD GAME
echo =------------------------------------------------------------=
echo Note: if no output, then if fcking broke and check LOG.txt
C:\Nintendo\UnityN3DS_1.3.10\Editor\Unity.exe -quit -batchmode -projectPath="C:\Users\user\Documents\MODDY\smo3ds" -executeMethod "BuildCommand.PerformBuild" -buildTarget "N3DS" -logFile LOG.txt
powershell -Command "Get-Content 'LOG.txt' -Wait"
rem LOG.txt
