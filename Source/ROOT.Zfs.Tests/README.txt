If tests start to time out, never completing on the build server.

Make sure that you have copied the build servers ssh public key to the zfs api server.

in /home/bbs/.ssh/authorized_keys

Otherwise the code will wait for a password indefinately - which will never happen.