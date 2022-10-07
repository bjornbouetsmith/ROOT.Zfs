# Introduction 
ZFS .NET library which interfaces with the zfs/zpool binaries to manipulate the ZFS file system.

[![.NET CI Build](https://github.com/bjornbouetsmith/ROOT.Zfs/actions/workflows/dotnet-ci-build.yml/badge.svg)](https://github.com/bjornbouetsmith/ROOT.Zfs/actions/workflows/dotnet-ci-build.yml)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=bjornbouetsmith_ROOT.Zfs&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=bjornbouetsmith_ROOT.Zfs)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=bjornbouetsmith_ROOT.Zfs&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=bjornbouetsmith_ROOT.Zfs)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=bjornbouetsmith_ROOT.Zfs&metric=bugs)](https://sonarcloud.io/summary/new_code?id=bjornbouetsmith_ROOT.Zfs)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=bjornbouetsmith_ROOT.Zfs&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=bjornbouetsmith_ROOT.Zfs)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=bjornbouetsmith_ROOT.Zfs&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=bjornbouetsmith_ROOT.Zfs)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=bjornbouetsmith_ROOT.Zfs&metric=coverage)](https://sonarcloud.io/summary/new_code?id=bjornbouetsmith_ROOT.Zfs)

(Coverage is bad I know - but much of the code is tested via integration tests, that I cannot get to run on github yet - if ever )

Tested against ZoL version 2.1.5-2 - will most likely work against different versions, but some of the commands will fail against ZFS on different platforms, since the supporting commands used to load disk/partition information are different.

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

## Dependencies
Obviously this software requires ZFS to be installed on the target server.
Besides ZFS - some methods also invokes the following binaries:

`smartctl`, `lsblk`, `ls`

`smartctl` is usually found in the package called `smartmontools` the two others should be available in the base installation of the OS.



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
