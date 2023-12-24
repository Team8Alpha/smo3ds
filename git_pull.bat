@echo off
echo Super Mario Odyssey 3DS demake,  Scripts by Moddimation
echo ACTION: GITHUB UPDATE PULL
echo =------------------------------------------------------------=
if exist .gitmain\ (
    set GITROOT=.gitmain
) else (
    set GITROOT=.git
)
echo Looking for remote updates, pulling...
git --git-dir=%GITROOT% pull
git --git-dir=%GITROOT% fetch