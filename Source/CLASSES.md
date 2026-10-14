# ROOT.Zfs Codebase Class Reference

This document provides an overview and explanation of the core classes, interfaces, structures, and helper components across the projects in the `Source` directory.

---

## Architecture Overview

The codebase is organized into primary projects under `Source`:

1. **`ROOT.Zfs.Public`**: Defines public interfaces, domain models, data transfer objects (DTOs), command arguments, and enums representing ZFS entities and operations.
2. **`ROOT.Zfs.Core`**: Implements the public interfaces by executing ZFS CLI commands via remote or local shell connections, parsing stdout/stderr responses, and constructing typed result objects.
3. **`ROOT.Zfs.Tests`**: Contains unit and integration tests verifying command generation, output parsing, and API behavior.

---

## 1. Core Implementation (`ROOT.Zfs.Core`)

### Main API Classes

- **`ZfsBase`**
  - Abstract base class for all operational ZFS managers (`ZPool`, `Zfs`, `Snapshots`, `Properties`, `DataSets`).
  - Stores and exposes the `IRemoteConnection` instance used for command execution.

- **`Zfs` (`ROOT.Zfs.Core.Zfs`)**
  - Main entry point implementing `IZfs` and inheriting `ZfsBase`.
  - Exposes properties to access sub-managers for ZFS operations:
    - `DataSets` (`IDataSets`)
    - `Snapshots` (`ISnapshots`)
    - `Properties` (`IProperties`)
    - `Pool` (`IZPool`)
  - Provides system-level helper methods like fetching ZFS version info, disk listings via `lsblk`, and SMART hardware information.

- **`ZPool`**
  - Implements `IZPool`.
  - Handles ZFS storage pool operations such as creation, destruction, import/export, scrubbing, status retrieval, vdev modification, and performance IO statistics parsing.

- **`DataSets`**
  - Implements `IDataSets`.
  - Handles dataset operations including creating, destroying, mounting, unmounting, promoting, listing datasets, and fetching dataset properties.

- **`Snapshots`**
  - Implements `ISnapshots`.
  - Manages ZFS snapshots and holds, including snapshot creation, cloning, destruction, hold, release, and listing holds or snapshots.

- **`Properties`**
  - Implements `IProperties`.
  - Manages reading, setting, and inheriting ZFS properties on datasets, pools, or snapshots.

---

### Command Builders (`ROOT.Zfs.Core.Commands`)

- **`Commands`**: Helper creating standard CLI process calls or `ProcessBootstrap` configurations.
- **`ZpoolCommands`**: Generates `zpool` executable CLI arguments for pool actions (e.g., `zpool status`, `zpool create`, `zpool iostat`).
- **`ZfsCommands`**: Base helper for ZFS dataset and property CLI invocation.
- **`DataSetCommands`**: Builds arguments for `zfs create`, `zfs destroy`, `zfs list`, `zfs mount`, and `zfs unmount`.
- **`SnapshotCommands`**: Builds arguments for `zfs snapshot`, `zfs rollback`, `zfs clone`, `zfs hold`, and `zfs release`.
- **`PropertyCommands`**: Builds arguments for `zfs get`, `zfs set`, and `zfs inherit`.
- **`ZdbCommands`**: Generates `zdb` command arguments for inspecting ZFS pool structures directly.

---

### Parsing and Helpers (`ROOT.Zfs.Core.Helpers`)

- **`ZPoolStatusParser`**: Parses text output from `zpool status` into structured `PoolStatus`, `VDev`, and `Device` model trees.
- **`ZPoolIOStatParser`**: Parses output from `zpool iostat` into `IOStats` data.
- **`ZPoolInfoParser`**: Extracts general pool configuration and metadata from `zpool list` outputs.
- **`PropertiesParser`**: Parses tab- or space-delimited `zfs get` and `zpool get` output into typed `Property` and `PropertyValue` lists.
- **`SnapshotHelper` & `SnapshotHoldParser`**: Formats arguments and parses `zfs list -t snapshot` and `zfs holds` command outputs.
- **`DataSetHelper` & `DataSetProperties`**: Converts `zfs list` raw output lines into `DataSet` model instances.
- **`SmartInfoParser`**: Parses output from `smartctl` into `SmartInfo` DTOs containing drive health data.
- **`DiskHelper`**: Parses `lsblk` JSON or text outputs into structured `DiskInfo` items.
- **`StateParser`**: Translates string status values into `State` enums.
- **`CommandHistoryHelper`**: Parses `zpool history` logs into `CommandHistory` objects.
- **`UpgradeablePoolsParser`**: Parses `zpool status` outputs listing pools that can be upgraded.
- **`ExceptionHelper`**: Utilities for throwing normalized ZFS exceptions on command execution failures.

---

## 2. Public Interfaces & Domain Models (`ROOT.Zfs.Public`)

### Primary Interfaces

- **`IZfs`**: Top-level interface aggregating `IDataSets`, `ISnapshots`, `IProperties`, and `IZPool`.
- **`IZPool`**: Interface contract for storage pool administration.
- **`IDataSets`**: Interface contract for managing ZFS filesystems and volumes.
- **`ISnapshots`**: Interface contract for snapshot creation, hold management, and restoration.
- **`IProperties`**: Interface contract for querying and mutating ZFS properties.
- **`IBasicZfs`**: Common abstraction for connection lifecycle management.

---

### Core Domain Models (`ROOT.Zfs.Public.Data`)

- **`Pool`**: High-level model representing a ZFS pool.
- **`PoolStatus`**: Detailed runtime health and structure tree of a pool.
- **`VDev`**: Represents virtual device configurations (e.g., Mirror, RaidZ1, RaidZ2, Cache, Log).
- **`Device`**: Represents individual physical disk devices or partitions within a VDev.
- **`DataSet`**: Represents a ZFS dataset (filesystem, volume, snapshot).
- **`Snapshot`**: Represents a ZFS snapshot point-in-time record.
- **`SnapshotHold`**: Represents a hold placed on a ZFS snapshot to prevent deletion.
- **`Property` & `PropertyValue`**: Represents ZFS configuration keys, current values, source flags, and target types.
- **`DiskInfo`**: Information on available storage block devices.
- **`SmartInfo`**: SMART monitoring attributes and drive health state.
- **`IOStats`**: Input/Output performance statistics for pools or devices.
- **`VersionInfo`**: Version information for installed ZFS modules and tools.

---

### Command Argument Classes (`ROOT.Zfs.Public.Arguments`)

Classes under `ROOT.Zfs.Public.Arguments` wrap inputs required for ZFS CLI invocation:

- **Base Class**: `Args` (Encapsulates validation and command-line string formatting logic).
- **Pool Arguments**: `PoolCreateArgs`, `PoolDestroyArgs`, `PoolStatusArgs`, `PoolScrubArgs`, `PoolTrimArgs`, `PoolUpgradeArgs`, `PoolAttachArgs`, `PoolDetachArgs`, `PoolReplaceArgs`, `PoolRemoveArgs`, `PoolIOStatsArgs`, etc.
- **Dataset Arguments**: `DatasetCreationArgs`, `DatasetDestroyArgs`, `DatasetListArgs`, `MountArgs`, `UnmountArgs`, `PromoteArgs`.
- **Snapshot Arguments**: `SnapshotCreateArgs`, `SnapshotDestroyArgs`, `SnapshotListArgs`, `SnapshotCloneArgs`, `SnapshotHoldArgs`, `SnapshotReleaseArgs`.
- **Property Arguments**: `GetPropertyArgs`, `SetPropertyArgs`, `InheritPropertyArgs`.

---

## Summary Diagram

```
+-------------------------------------------------------------+
|                          IZfs                               |
|   (Exposes IDataSets, ISnapshots, IProperties, IZPool)      |
+------------------------------+------------------------------+
                               | Implemented by
                               v
+-------------------------------------------------------------+
|                          Zfs                                |
|   (Uses Commands, Parsers, & RemoteConnection execution)   |
+----+-------------------+-------------------+----------------+
     |                   |                   |                |
     v                   v                   v                v
+----+----+         +----+----+         +----+----+      +----+----+
|DataSets |         |Snapshots|         |Properties|      |  ZPool  |
+---------+         +---------+         +---------+      +---------+
```
