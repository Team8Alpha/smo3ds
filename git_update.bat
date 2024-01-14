@echo off
echo Super Mario Odyssey 3DS demake,  Scripts by Moddimation
echo ACTION: GITHUB UPDATE FULL
echo =------------------------------------------------------------=

set GITEXE="C:\Program Files\Git\mingw64\libexec\git-core\git.exe"
set GITROOT=.git
echo Looking for remote updates...
%GITEXE% --git-dir=%GITROOT% pull
echo Adding changes...
%GITEXE% --git-dir=%GITROOT% add .
set /p DUMMY=enter update note: 
echo Comitting changes...
%GITEXE% --git-dir=%GITROOT% commit -m "%DUMMY%"
echo Uploading changes, log: "%DUMMY%"
%GITEXE% --git-dir=%GITROOT% push
echo Looking for remote updates, finishing...
%GITEXE% --git-dir=%GITROOT% pull
%GITEXE% --git-dir=%GITROOT% fetch
%GITEXE% --git-dir=%GITROOT% push