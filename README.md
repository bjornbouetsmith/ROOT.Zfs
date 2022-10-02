# Introduction 
ZFS .NET library which interfaces with the zfs/zpool binaries to manipulate the ZFS file system

[![.NET CI Build](https://github.com/bjornbouetsmith/ROOT.Zfs/actions/workflows/dotnet-ci-build.yml/badge.svg)](https://github.com/bjornbouetsmith/ROOT.Zfs/actions/workflows/dotnet-ci-build.yml)

[![codecov](https://codecov.io/github/bjornbouetsmith/ROOT.Zfs/branch/main/graph/badge.svg?token=HVSPMWW7NZ)](https://codecov.io/github/bjornbouetsmith/ROOT.Zfs) (Bad I know - but much of the code is tested via integration tests, that I cannot get to run on github yet - if ever)

## Example

~~~c#

// First decide if you are connecting to localhost or another machine
// null indicates that you are running on the same machine where the zfs pool is located
// Otherwise you would do: new SSHProcessCall("username","hostname",true);
SSHProcessCall connection = null;

// Create instance of zfs class
var zfs = new Core.Zfs(null);

// Get status of the pool tank
var status = zfs.Pool.GetStatus("tank");

if (status.State == State.Online)
{
    return;
}

var pool = status.Pool;
foreach (var vdev in pool.VDevs)
{
    if (vdev.State == State.Online)
    {
        continue;
    }

    Console.WriteLine("vdev:{0}, State:{1}", vdev.Name, vdev.State);
    foreach (var device in vdev.Devices)
    {
        if (device.State == State.Online)
        {
            continue;
        }

        Console.WriteLine("Device:{0}, State:{1}", device.DeviceName, device.State);
    }
}
~~~

## Help needed
If you want to help make this library greater either by 
* Contributing code 
* Writing examples
* Better documentation
* Writing tests and writing expected responses for a given version of zfs - if it differs from version to version
* In any other way, feel free to contact [@bjornbouetsmith](https://github.com/bjornbouetsmith).

## NOTE

Nuget packages found here on github is to be considered development versions and is only meant to enable ci/cd within the ROOT projects.

Packages not found on nuget.org should not be used in production code unless you are ready to change stuff if interfaces or something changes.

Its fine to take packages from here to test out patches etc, but do not expect stable interfaces on new features until its released on nuget.org.
