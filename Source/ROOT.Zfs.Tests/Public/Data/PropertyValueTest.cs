using Xunit;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Public.Data;

public class PropertyValueTest
{

    [Theory]
    [InlineData("atime", "off", "noatime")]
    [InlineData("atime", "on", "atime")]
    [InlineData("canmount", "off", "noauto")]
    [InlineData("canmount", "on", "auto")]
    [InlineData("devices", "off", "nodev")]
    [InlineData("devices", "on", "dev")]
    [InlineData("exec", "off", "noexec")]
    [InlineData("exec", "on", "exec")]
    [InlineData("readonly", "off", "rw")]
    [InlineData("readonly", "on", "ro")]
    [InlineData("realatime", "off", "norealatime")]
    [InlineData("realatime", "on", "realatime")]
    [InlineData("setuid", "off", "nosuid")]
    [InlineData("setuid", "on", "suid")]
    [InlineData("xattr", "off", "noxattr")]
    [InlineData("xattr", "on", "xattr")]
    [InlineData("nbmand", "off", "nomand")]
    [InlineData("nbmand", "on", "mand")]
    [InlineData("context", "bbs", "context=bbs")]
    [InlineData("fscontext", "bbs", "fscontext=bbs")]
    [InlineData("defcontext", "bbs", "defcontext=bbs")]
    [InlineData("rootcontext", "bbs", "rootcontext=bbs")]
    [InlineData("canmount", "", "")]
    [InlineData("canmount", null, "")]
    [InlineData("", "off", "")]
    [InlineData(null, "off", "")]
    [InlineData(null, null, "")]
    [InlineData(null, "", "")]
    [InlineData("", null, "")]
    [InlineData("", "", "")]
    [InlineData("vscan", "off", "")]
    public void ToMountArgumentTest(string property, string value, string expectedMountArg)
    {
        var prop = new PropertyValue { Property = property, Value = value };
        var mountArg = prop.ToMountArgument();
        Assert.Equal(expectedMountArg, mountArg);
    }
}