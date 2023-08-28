@echo off
if exist .gitmain\ (
    set GITROOT=.gitmain
) else (
    set GITROOT=.git
)
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

if exist .gitbuild\(
    git --git-dir=.gitbuild pull
    git --git-dir=.gitbuild add out.cci out.cci.xml out.cia
    git --git-dir=.gitbuild commit -m "by dima"
    git --git-dir=.gitbuild push
    git --git-dir=.gitbuild pull
    git --git-dir=.gitbuild fetch
)