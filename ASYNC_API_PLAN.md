# Proposed plan: async `Task`-based public API

## 1. Scope

Convert the five operational public interfaces in `ROOT.Zfs.Public`:

- `IZfs`
- `IDatasets`
- `IProperties`
- `ISnapshots`
- `IZPool`

Explicitly **not** changed:

- `IBasicZfs` — `CommandTimeout` and `RequiresSudo` stay synchronous.
- The `IZfs` composition properties (`Snapshots`, `Datasets`, `Properties`, `Pool`) — they are properties, not methods, and they return the same interfaces whose methods become async.
- The internal command-builder interfaces under `ROOT.Zfs.Core/Commands` — not part of the public library contract.

There are **41 interface methods** to convert.

I recommend an **async-first breaking change**: rename each method with an `Async` suffix and return `Task`/`Task<T>`, removing the synchronous method. The alternative is to keep sync methods and add async variants alongside them, which doubles the API and risks sync-over-async wrappers.

---

## 2. Proposed interface changes

### `IZfs` → implemented by `Zfs`

| Current | Proposed |
|---|---|
| `VersionInfo GetVersionInfo()` | `Task<VersionInfo> GetVersionInfoAsync(CancellationToken cancellationToken = default)` |
| `IEnumerable<DiskInfo> ListDisks()` | `Task<IEnumerable<DiskInfo>> ListDisksAsync(CancellationToken cancellationToken = default)` |
| `IEnumerable<SmartInfo> GetSmartInfos()` | `Task<IEnumerable<SmartInfo>> GetSmartInfosAsync(CancellationToken cancellationToken = default)` |
| `SmartInfo GetSmartInfo(string name)` | `Task<SmartInfo> GetSmartInfoAsync(string name, CancellationToken cancellationToken = default)` |

### `IDatasets` → implemented by `Datasets`

| Current | Proposed |
|---|---|
| `IList<Dataset> List(DatasetListArgs args)` | `Task<IReadOnlyList<Dataset>> ListAsync(DatasetListArgs args, CancellationToken cancellationToken = default)` |
| `Dataset Create(DatasetCreationArgs arguments)` | `Task<Dataset> CreateAsync(DatasetCreationArgs arguments, CancellationToken cancellationToken = default)` |
| `DatasetDestroyResponse Destroy(DatasetDestroyArgs args)` | `Task<DatasetDestroyResponse> DestroyAsync(DatasetDestroyArgs args, CancellationToken cancellationToken = default)` |
| `void Promote(PromoteArgs args)` | `Task PromoteAsync(PromoteArgs args, CancellationToken cancellationToken = default)` |
| `void Mount(MountArgs mountArgs)` | `Task MountAsync(MountArgs mountArgs, CancellationToken cancellationToken = default)` |
| `void Unmount(UnmountArgs unmountArgs)` | `Task UnmountAsync(UnmountArgs unmountArgs, CancellationToken cancellationToken = default)` |

### `IProperties` → implemented by `Properties`

| Current | Proposed |
|---|---|
| `IList<Property> GetAvailable(PropertyTarget targetType)` | `Task<IReadOnlyList<Property>> GetAvailableAsync(PropertyTarget targetType, CancellationToken cancellationToken = default)` |
| `IList<PropertyValue> Get(GetPropertyArgs args)` | `Task<IReadOnlyList<PropertyValue>> GetAsync(GetPropertyArgs args, CancellationToken cancellationToken = default)` |
| `PropertyValue Set(SetPropertyArgs args)` | `Task<PropertyValue> SetAsync(SetPropertyArgs args, CancellationToken cancellationToken = default)` |
| `void Reset(InheritPropertyArgs args)` | `Task ResetAsync(InheritPropertyArgs args, CancellationToken cancellationToken = default)` |

### `ISnapshots` → implemented by `Snapshots`

| Current | Proposed |
|---|---|
| `IList<Snapshot> List(SnapshotListArgs args)` | `Task<IReadOnlyList<Snapshot>> ListAsync(SnapshotListArgs args, CancellationToken cancellationToken = default)` |
| `void Destroy(SnapshotDestroyArgs args)` | `Task DestroyAsync(SnapshotDestroyArgs args, CancellationToken cancellationToken = default)` |
| `void Create(SnapshotCreateArgs args)` | `Task CreateAsync(SnapshotCreateArgs args, CancellationToken cancellationToken = default)` |
| `void Clone(SnapshotCloneArgs args)` | `Task CloneAsync(SnapshotCloneArgs args, CancellationToken cancellationToken = default)` |
| `void Hold(SnapshotHoldArgs args)` | `Task HoldAsync(SnapshotHoldArgs args, CancellationToken cancellationToken = default)` |
| `IList<SnapshotHold> Holds(SnapshotHoldsArgs args)` | `Task<IReadOnlyList<SnapshotHold>> HoldsAsync(SnapshotHoldsArgs args, CancellationToken cancellationToken = default)` |
| `void Release(SnapshotReleaseArgs args)` | `Task ReleaseAsync(SnapshotReleaseArgs args, CancellationToken cancellationToken = default)` |

### `IZPool` → implemented by `ZPool`

| Current | Proposed |
|---|---|
| `IEnumerable<CommandHistory> History(PoolHistoryArgs args)` | `Task<IEnumerable<CommandHistory>> HistoryAsync(PoolHistoryArgs args, CancellationToken cancellationToken = default)` |
| `PoolStatus Status(PoolStatusArgs args)` | `Task<PoolStatus> StatusAsync(PoolStatusArgs args, CancellationToken cancellationToken = default)` |
| `IList<PoolInfo> List()` | `Task<IReadOnlyList<PoolInfo>> ListAsync(CancellationToken cancellationToken = default)` |
| `PoolInfo List(PoolListArgs args)` | `Task<PoolInfo> ListAsync(PoolListArgs args, CancellationToken cancellationToken = default)` |
| `PoolStatus Create(PoolCreateArgs args)` | `Task<PoolStatus> CreateAsync(PoolCreateArgs args, CancellationToken cancellationToken = default)` |
| `void Destroy(PoolDestroyArgs args)` | `Task DestroyAsync(PoolDestroyArgs args, CancellationToken cancellationToken = default)` |
| `void Offline(PoolOfflineArgs args)` | `Task OfflineAsync(PoolOfflineArgs args, CancellationToken cancellationToken = default)` |
| `void Online(PoolOnlineArgs args)` | `Task OnlineAsync(PoolOnlineArgs args, CancellationToken cancellationToken = default)` |
| `void Clear(PoolClearArgs args)` | `Task ClearAsync(PoolClearArgs args, CancellationToken cancellationToken = default)` |
| `IOStats IOStats(PoolIOStatsArgs args)` | `Task<IOStats> IOStatsAsync(PoolIOStatsArgs args, CancellationToken cancellationToken = default)` |
| `void Resilver(PoolResilverArgs args)` | `Task ResilverAsync(PoolResilverArgs args, CancellationToken cancellationToken = default)` |
| `void Scrub(PoolScrubArgs args)` | `Task ScrubAsync(PoolScrubArgs args, CancellationToken cancellationToken = default)` |
| `void Trim(PoolTrimArgs args)` | `Task TrimAsync(PoolTrimArgs args, CancellationToken cancellationToken = default)` |
| `IList<UpgradeablePool> GetUpgradeablePools()` | `Task<IReadOnlyList<UpgradeablePool>> GetUpgradeablePoolsAsync(CancellationToken cancellationToken = default)` |
| `void Upgrade(PoolUpgradeArgs args)` | `Task UpgradeAsync(PoolUpgradeArgs args, CancellationToken cancellationToken = default)` |
| `void Detach(PoolDetachArgs args)` | `Task DetachAsync(PoolDetachArgs args, CancellationToken cancellationToken = default)` |
| `void Attach(PoolAttachArgs args)` | `Task AttachAsync(PoolAttachArgs args, CancellationToken cancellationToken = default)` |
| `void Replace(PoolReplaceArgs args)` | `Task ReplaceAsync(PoolReplaceArgs args, CancellationToken cancellationToken = default)` |
| `void Add(PoolAddArgs args)` | `Task AddAsync(PoolAddArgs args, CancellationToken cancellationToken = default)` |
| `void Remove(PoolRemoveArgs args)` | `Task RemoveAsync(PoolRemoveArgs args, CancellationToken cancellationToken = default)` |

I recommend `Task<IEnumerable<T>>` rather than `IAsyncEnumerable<T>` for `History`, `ListDisks`, and `GetSmartInfos`, to keep the API change mechanical and return types equivalent.

Methods that currently return `IList<T>` will return `IReadOnlyList<T>` in the async API. Implementations may continue to build a `List<T>` internally or use the existing parser results, but the public return type will be `IReadOnlyList<T>`.

Every async method will accept an optional `CancellationToken cancellationToken = default`. The plan assumes that `ROOT.Shared.Utils.OS.LoadResponseAsync` will be extended to accept a cancellation token (for example, `LoadResponseAsync(bool throwOnFailure, CancellationToken cancellationToken, params string[] arguments)`). The token will be passed through to the command execution layer so cancellation can actually abort the underlying command on a best-effort basis. The public interfaces will also need `using System.Threading;`.

---

## 3. Implementation changes

The key enabling change already exists in the working tree:

- `Source/Directory.Packages.props` references `ROOT.Shared.Utils.OS` 1.1.14.
- `FakeRemoteConnection` already implements `LoadResponseAsync`.
- `IProcessCall` exposes `Task<ProcessCallResult> LoadResponseAsync(bool, params string[])`; it is assumed that this overload will be extended to accept a `CancellationToken`.

I will preserve those existing uncommitted changes.

| File | Change |
|---|---|
| `Source/ROOT.Zfs.Core/ZfsBase.cs` | No async members needed. `BuildCommand(...)` stays synchronous; it only builds the command object. |
| `Source/ROOT.Zfs.Core/Zfs.cs` | Convert `GetVersionInfo`, `ListDisks`, `GetSmartInfos`, `GetSmartInfo`. `ListDisksAsync` can no longer use `yield return`; it must build a list and return it. `GetSmartInfosAsync` awaits `ListDisksAsync` and then runs the `smartctl` calls, sequentially by default. |
| `Source/ROOT.Zfs.Core/DataSets.cs` | Convert all six methods. `CreateAsync` awaits the create command and then `ListAsync`. `DestroyAsync` keeps dry-run handling. |
| `Source/ROOT.Zfs.Core/Properties.cs` | Convert all four methods, including the private `GetAvailable(GetPropertyArgs)` helper. `SetAsync` awaits the set command and then `GetAsync`. |
| `Source/ROOT.Zfs.Core/Snapshots.cs` | Convert all seven methods. `DestroyAsync` must await `ListAsync` when handling pattern-based deletion and recurse with `await DestroyAsync(...)`. |
| `Source/ROOT.Zfs.Core/ZPool.cs` | Convert all twenty methods. `CreateAsync` awaits create then `StatusAsync`. `ListAsync()` awaits the `zpool list` command then the `zdb` command. |
| `Source/ROOT.Zfs.Core/ZfsServiceCollectionExtensions.cs` | No change; DI registration remains the same. |
| `Source/ROOT.Zfs.Public/*.cs` | Update signatures and XML documentation with `<returns>A task ...</returns>`. |
| `README.md`, `Source/CLASSES.md` | Update examples and descriptions to use `await ...Async()`. |

Implementation rules:

- Replace synchronous `pc.LoadResponse(true)` calls with `await pc.LoadResponseAsync(true, cancellationToken)`.
- Call `cancellationToken.ThrowIfCancellationRequested()` at the start of each public async method so already-cancelled calls fail fast.
- Pass the same `CancellationToken` through internal/private async helpers and composite follow-up calls, for example `CreateAsync` → `ListAsync`, `SetAsync` → `GetAsync`, and `DestroyAsync` → `ListAsync`/`DestroyAsync`.
- Never wrap synchronous calls in `Task.Run(...)`; that fakes async and consumes extra threads.
- Do not use `.Result` or `.Wait()` in library code.
- Use `ConfigureAwait(false)` in Core awaits.
- Use `async`/`await` only where there is post-await parsing or follow-up work; otherwise return the `Task` directly.
- Keep the two `ZPool.ListAsync` commands sequential rather than `Task.WhenAll`: `FakeRemoteConnection.Pipe` mutates the same connection object, and real remote connections should not be assumed safe for concurrent command execution without verification.

---

## 4. Testing the async interface

### Migrate existing tests

The existing fake integration tests already exercise every method. They should become the primary async regression suite:

- `Source/ROOT.Zfs.Tests/Integration/Fake/FakeZpoolTest.cs`
- `Source/ROOT.Zfs.Tests/Integration/Fake/FakePropertiesTest.cs`
- `Source/ROOT.Zfs.Tests/Integration/Fake/SnapshotTest.cs`
- `Source/ROOT.Zfs.Tests/Integration/Fake/DataSet/FakeDataSetTest*.cs`
- `Source/ROOT.Zfs.Tests/Integration/Fake/FakeZfsTest.cs`
- `Source/ROOT.Zfs.Tests/Integration/Fake/SmartInfoTest.cs`

Changes:

- Test methods become `public async Task ...`.
- Calls become `await zfs.Pool.StatusAsync(...)`.
- Existing command-line assertions remain, so async conversion cannot silently change the commands sent.
- `TestPool` helper methods become async (`CreatePoolAsync`, disk setup, async cleanup/disposal as needed).
- Real integration tests under `Source/ROOT.Zfs.Tests/Integration` are migrated to `await`.

### Add async-specific tests

Add a dedicated test file, for example `Source/ROOT.Zfs.Tests/Async/AsyncApiTests.cs`, plus small additions to `FakeRemoteConnection`:

1. **Async-path guard**
   - Add counters for `LoadResponse` and `LoadResponseAsync` to `FakeRemoteConnection`.
   - Assert that each async public method calls `LoadResponseAsync` and never `LoadResponse`.
   - This catches accidental `Task.Run(() => LoadResponse(...))` or sync-over-async implementations.

2. **Result/behavior parity**
   - Assert each async method returns the same parsed result as the existing fake tests expect.
   - Cover composite methods explicitly:
     - `DataSets.CreateAsync` → create then list.
     - `Properties.SetAsync` → set then get.
     - `Snapshots.DestroyAsync` with a pattern → list then destroy matching snapshots.
     - `ZPool.CreateAsync` → create then status.
     - `ZPool.ListAsync()` → zpool list then zdb.
     - `Zfs.GetSmartInfosAsync` → list disks then one smartctl call per disk.

3. **Exception propagation**
   - Use a fake response with stderr or a failed exit code.
   - Assert the async methods throw the same exception type as before, using `Assert.ThrowsExactlyAsync`.
   - Especially important for `GetAvailableAsync`, which intentionally reads stderr.

4. **Lazy-enumeration behavior**
   - `ListDisksAsync` used to be a deferred iterator. Add a test confirming the async version returns a materialized list and that the command has already run when the task completes.

5. **Cancellation**
   - Assert that an already-cancelled token causes `OperationCanceledException` before any command is run.
   - Add a fake that records the `CancellationToken` passed to `LoadResponseAsync` and assert the public method forwards the caller's token, including through composite follow-up calls.
   - If the updated `LoadResponseAsync` supports mid-command cancellation, add a delayed fake test that cancels while the command is in flight and asserts `OperationCanceledException`.

6. **Build/CI**
   - `dotnet build Source/ROOT.Zfs.slnx`
   - `dotnet test Source/**/*.Tests.csproj --filter-not-trait Category=Integration`
   - CI integration filter: `--filter-trait Category=Integration`

Because `TreatWarningsAsErrors` is enabled, forgotten awaits and `async` methods without `await` will fail the build.

---

## 5. GitHub issue plan

**46 issues** were created:

- **41 method issues** — one per interface method listed above.
- **5 class tracking issues** — one each for `Zfs`, `Datasets`, `Properties`, `Snapshots`, and `ZPool`.
- **0 issues** for `IBasicZfs` or the `IZfs` composition properties.

Creation order:

1. Created the 41 method issues first.
2. Captured their issue numbers and URLs.
3. Created the 5 class tracking issues with a checklist linking directly to every method issue for that class.

Created titles:

- Method issues: `[Async] IZPool.Status -> StatusAsync`
- Overloads disambiguated: `[Async] IZPool.List() -> ListAsync()` and `[Async] IZPool.List(PoolListArgs) -> ListAsync(PoolListArgs)`
- Class issues: `[Async API] ZPool (IZPool) tracking issue`

Labels:

- Every issue: `enhancement` and `async`
- Class issues additionally: `tracking`
- The `async` and `tracking` labels will be created because they do not currently exist. The existing `enhancement` label is GitHub's "New feature or request" label.

Each method issue body will contain:

- Current signature
- Proposed async signature
- Interface and implementation file
- Implementation notes, including any follow-up calls that must be awaited
- Acceptance criteria:
  - async `Task` signature with `Async` suffix and `CancellationToken cancellationToken = default`
  - implementation uses `LoadResponseAsync` and forwards the cancellation token
  - relevant fake test migrated/added, including cancellation-token forwarding
  - no synchronous command execution on that path
  - XML docs updated

Each class issue body will contain a checklist of the corresponding method issues with direct links, plus a short implementation note and link to the plan/README changes.

---

## 6. Suggested delivery sequence

If this is split into pull requests, I recommend:

1. `IZfs`/`Zfs`
2. `IDatasets`/`Datasets`
3. `IProperties`/`Properties`
4. `ISnapshots`/`Snapshots`
5. `IZPool`/`ZPool`
6. Tests/docs/version bump

If the sync methods are removed, this is a breaking change and should be a major version bump from `1.1.10` to `2.0.0`.

---

## 7. Confirmed decisions

1. **Replace the synchronous methods** with `*Async` methods. This is a breaking change.
2. **Use `Task<IEnumerable<T>>`**, not `IAsyncEnumerable<T>`, for `HistoryAsync`, `ListDisksAsync`, and `GetSmartInfosAsync`.
3. **Add `CancellationToken cancellationToken = default`** to every async method. Assume `ROOT.Shared.Utils.OS.LoadResponseAsync` will accept and honour a cancellation token.
4. **Use the labels** `enhancement` and `async` on every issue, plus `tracking` on the class issues.
5. **GitHub CLI is installed and authenticated** as `bjornbouetsmith`.
6. Methods that currently return `IList<T>` will return `IReadOnlyList<T>`.
7. Issue creation will use the repository `bjornbouetsmith/ROOT.Zfs`.

---

## 8. Created issues

Created on 2026-11-04.

| Interface | Implementation | Method issues | Tracking issue |
|---|---|---|---|
| `IZfs` | `Zfs` | #39–#42 | [#80](https://github.com/bjornbouetsmith/ROOT.Zfs/issues/80) |
| `IDatasets` | `Datasets` | #43–#48 | [#81](https://github.com/bjornbouetsmith/ROOT.Zfs/issues/81) |
| `IProperties` | `Properties` | #49–#52 | [#82](https://github.com/bjornbouetsmith/ROOT.Zfs/issues/82) |
| `ISnapshots` | `Snapshots` | #53–#59 | [#83](https://github.com/bjornbouetsmith/ROOT.Zfs/issues/83) |
| `IZPool` | `ZPool` | #60–#79 | [#84](https://github.com/bjornbouetsmith/ROOT.Zfs/issues/84) |

Each tracking issue contains direct links to every method issue for its interface. All issues carry the `enhancement` and `async` labels; tracking issues additionally carry `tracking`.
