using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Tests.Integration.Fake;

public class FakePropertiesTest
{
    private readonly ITestOutputHelper _output;

    public FakePropertiesTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly FakeRemoteConnection _remoteProcessCall = new("2.1.5-2");

    private Properties GetProperties()
    {
        return ZfsTestFactory.CreateProperties(_remoteProcessCall);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task ListAllPropertiesAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();
        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds" };
        var props = await pr.GetAsync(args, TestContext.Current.CancellationToken);
        Assert.NotNull(props);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        foreach (var prop in props)
        {
            _output.WriteLine(prop.Dump(new JsonFormatter()));
        }
    }

    [Fact]
    [Trait("Category", "FakeIntegration")]
    public void SetDataSetProperty()
    {
        var pr = GetProperties();
        var args = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime", Value = "off" };
        pr.Set(args);

        var commands = _remoteProcessCall.GetCommandsInvoked();
        Assert.Equal(2, commands.Count); // Set+Get

        Assert.Equal("/sbin/zfs set atime=off tank/myds", commands[0]);
        Assert.Equal("/sbin/zfs get atime tank/myds -H", commands[1]);
    }

    /// <summary>
    /// We cannot test that setting works with the current fake implementation.
    /// </summary>
    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetDataSetPropertyAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();
        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime"};
        var val = (await pr.GetAsync(args, TestContext.Current.CancellationToken))[0];
            
        Assert.Equal("off", val.Value);
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        var commands = _remoteProcessCall.GetCommandsInvoked();
        var command = Assert.Single(commands); // Get
            
        Assert.Equal("/sbin/zfs get atime tank/myds -H", command);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();
        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime" };

        await pr.GetAsync(args, TestContext.Current.CancellationToken);

        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();
        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime" };
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => pr.GetAsync(args, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetAvailableDatasetPropertiesAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();

        var props = await pr.GetAvailableAsync(PropertyTarget.Dataset, TestContext.Current.CancellationToken);

        Assert.NotEmpty(props);
        Assert.Contains("/sbin/zfs get -H", _remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        _output.WriteLine(props.Dump(new JsonFormatter()));
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetAvailablePoolPropertiesAsyncTest(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();

        var props = await pr.GetAvailableAsync(PropertyTarget.Pool, TestContext.Current.CancellationToken);

        Assert.NotEmpty(props);
        Assert.Contains("/sbin/zpool get -H", _remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(1, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
        _output.WriteLine(props.Dump(new JsonFormatter()));
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetAvailableAsyncForwardsCancellationToken(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();

        await pr.GetAvailableAsync(PropertyTarget.Dataset, TestContext.Current.CancellationToken);

        var token = Assert.Single(_remoteProcessCall.AsyncCancellationTokens);
        Assert.Equal(TestContext.Current.CancellationToken, token);
    }

    [Theory]
    [Trait("Category", "FakeIntegration")]
    [InlineData(true), InlineData(false)]
    public async Task GetAvailableAsyncWithCancelledTokenDoesNotRunCommand(bool completeAsyncResponses)
    {
        _remoteProcessCall.CompleteAsyncResponses = completeAsyncResponses;
        var pr = GetProperties();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => pr.GetAvailableAsync(PropertyTarget.Dataset, cancellationTokenSource.Token));

        Assert.Empty(_remoteProcessCall.GetCommandsInvoked());
        Assert.Equal(0, _remoteProcessCall.AsyncLoadResponseCalls);
        Assert.Equal(0, _remoteProcessCall.SyncLoadResponseCalls);
    }

    /// <summary>
    /// We cannot test that reset works with the current fake implementation.
    /// </summary>
    [Fact]
    [Trait("Category", "FakeIntegration")]
    public async Task ResetPropertyTestAsync()
    {
        var pr = GetProperties();

        var target = "tank/myds";
        var property = "atime";

        var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
        var before = (await pr.GetAsync(args, TestContext.Current.CancellationToken))[0];
        Assert.Equal("off", before.Value);
        var setArg = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property, Value = "on" };
        pr.Set(setArg);
        var inheritArgs = new InheritPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
        pr.Reset(inheritArgs);

        var commands = _remoteProcessCall.GetCommandsInvoked();

        _output.WriteLine(commands.Dump(new JsonFormatter()));
        // Get, set & get, inherit
        Assert.Equal(4, commands.Count);

    }
}