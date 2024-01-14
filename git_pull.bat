@echo off
echo Super Mario Odyssey 3DS demake,  Scripts by Moddimation
echo ACTION: GITHUB UPDATE PULL
echo =------------------------------------------------------------=
if exist .gitmain\ (
    set GITROOT=.gitmain
) else (
    set GITROOT=.git
)
set GITEXE="C:\Program Files\Git\mingw64\libexec\git-core\git.exe"
echo Looking for remote updates, pulling...
%GITEXE% --git-dir=%GITROOT% pull
%GITEXE% --git-dir=%GITROOT% fetch