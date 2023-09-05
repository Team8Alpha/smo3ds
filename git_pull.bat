@echo off
echo Super Mario Odyssey 3DS demake,  Github updater by Moddimation
echo ACTION: UPDATE PULL
echo =------------------------------------------------------------=
if exist .gitmain\ (
    set GITROOT=.gitmain
) else (
    set GITROOT=.git
)
echo Looking for remote updates, pulling...
git --git-dir=%GITROOT% pull
git --git-dir=%GITROOT% fetch