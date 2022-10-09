The point of `IVersionResponse` is to make it possible to make tests agnostic to different responses from the binaries in the system.

The library was initially built with version `2.1.5-2` of [zfs on linux](https://github.com/openzfs/zfs/releases/tag/zfs-2.1.5).

So everything is built into the `VersionAgnosticResponse` because its the default version.

When new versions are tested and found to behave exactly as the current a new Implementation of `IVersionResponse` can be made that simply does not override any responses.

If any responses are different, then an implementation similar to this would be made
~~~csharp
internal class VersionResponse2_1_7 : VersionAgnosticResponse
{
    public VersionResponse2_1_7() : base("2.1.7")
    {

    }
    public override (string StdOut, string StdError) LoadResponse(string commandLine)
    {
        switch (commandLine)
        {
            case "/sbin/zfs get all tank/myds -H":
                return("Data how version 2.1.7 would return data",null);
            default:
                return base.LoadResponse(commandLine);
        }
    }
}
~~~

Having "fakes" made like this makes it possible to test many versions of ZFS easily without having to have duplicates of tests.