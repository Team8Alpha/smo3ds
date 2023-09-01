@echo off
echo Super Mario Odyssey 3DS demake,  Github updater by Mochima

set GITROOT=.git
echo Looking for remote updates...
git --git-dir=%GITROOT% pull
echo Adding changes...
git --git-dir=%GITROOT% add .
set /p DUMMY=enter update note: 
echo Comitting changes...
git --git-dir=%GITROOT% commit -m "%DUMMY%"
echo Uploading changes, log: "%DUMMY%"
git --git-dir=%GITROOT% push
echo Looking for remote updates, finishing...
git --git-dir=%GITROOT% pull
git --git-dir=%GITROOT% fetch