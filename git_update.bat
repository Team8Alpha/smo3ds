@echo off
echo Looking for remote updates...
git --git-dir=.gitmain pull
echo Adding changes...
git --git-dir=.gitmain add .
set /p DUMMY=enter update note: 
echo Comitting changes...
git --git-dir=.gitmain commit -m "%DUMMY%"
echo Uploading changes, log: \"%DUMMY%\"
git --git-dir=.gitmain push
echo Looking for remote updates, finishing...
git --git-dir=.gitmain pull
git --git-dir=.gitmain fetch

git --git-dir=.gitbuild pull
git --git-dir=.gitbuild add out.cci out.cci.xml out.cia
git --git-dir=.gitbuild commit -m "by dima"
git --git-dir=.gitbuild push
git --git-dir=.gitbuild pull
git --git-dir=.gitbuild fetch