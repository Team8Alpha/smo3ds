git --git-dir=.gitmain pull
git --git-dir=.gitmain add .
git --git-dir=.gitmain commit -m "by dima"
git --git-dir=.gitmain push
git --git-dir=.gitmain pull
git --git-dir=.gitmain fetch

git --git-dir=.gitbuild pull
git --git-dir=.gitbuild add out.cci out.cci.xml out.cia
git --git-dir=.gitbuild commit -m "by dima"
git --git-dir=.gitbuild push
git --git-dir=.gitbuild pull
git --git-dir=.gitbuild fetch