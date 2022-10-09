If tests start to time out, never completing on the build server.

Make sure that you have copied the build servers ssh public key to the zfs api server.

in /home/[username/.ssh/authorized_keys

Where [username] is the username you ssh into the server with.

Otherwise the code will wait for a password indefinately - which will never happen.
