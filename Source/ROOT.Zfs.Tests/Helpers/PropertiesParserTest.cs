using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class PropertiesParserTest
    {
        #region responses
        private const string zpoolResponse = @"usage:
        get [-Hp] [-o ""all"" | field[,...]] <""all"" | property[,...]> <pool> ...

the following properties are supported:

        PROPERTY             EDIT   VALUES

        allocated              NO   <size>
        capacity               NO   <size>
        checkpoint             NO   <size>
        dedupratio             NO   <1.00x or higher if deduped>
        expandsize             NO   <size>
        fragmentation          NO   <percent>
        free                   NO   <size>
        freeing                NO   <size>
        guid                   NO   <guid>
        health                 NO   <state>
        leaked                 NO   <size>
        load_guid              NO   <load_guid>
        size                   NO   <size>
        altroot               YES   <path>
        ashift                YES   <ashift, 9-16, or 0=default>
        autoexpand            YES   on | off
        autoreplace           YES   on | off
        autotrim              YES   on | off
        bootfs                YES   <filesystem>
        cachefile             YES   <file> | none
        comment               YES   <comment-string>
        compatibility         YES   <file[,file...]> | off | legacy
        delegation            YES   on | off
        failmode              YES   wait | continue | panic
        listsnapshots         YES   on | off
        multihost             YES   on | off
        readonly              YES   on | off
        version               YES   <version>
        feature@...           YES   disabled | enabled | active

The feature@ properties must be appended with a feature name.
See zpool-features(7).
";

        private const string zfsResponse = @"usage:
        get [-rHp] [-d max] [-o ""all"" | field[,...]]
            [-t type[,...]] [-s source[,...]]
            <""all"" | property[,...]> [filesystem|volume|snapshot|bookmark] ...

The following properties are supported:

        PROPERTY       EDIT  INHERIT   VALUES

        available        NO       NO   <size>
        clones           NO       NO   <dataset>[,...]
        compressratio    NO       NO   <1.00x or higher if compressed>
        createtxg        NO       NO   <uint64>
        creation         NO       NO   <date>
        defer_destroy    NO       NO   yes | no
        encryptionroot   NO       NO   <filesystem | volume>
        filesystem_count  NO       NO   <count>
        guid             NO       NO   <uint64>
        keystatus        NO       NO   none | unavailable | available
        logicalreferenced  NO       NO   <size>
        logicalused      NO       NO   <size>
        mounted          NO       NO   yes | no
        objsetid         NO       NO   <uint64>
        origin           NO       NO   <snapshot>
        receive_resume_token  NO       NO   <string token>
        redact_snaps     NO       NO   <snapshot>[,...]
        refcompressratio  NO       NO   <1.00x or higher if compressed>
        referenced       NO       NO   <size>
        snapshot_count   NO       NO   <count>
        type             NO       NO   filesystem | volume | snapshot | bookmark
        used             NO       NO   <size>
        usedbychildren   NO       NO   <size>
        usedbydataset    NO       NO   <size>
        usedbyrefreservation  NO       NO   <size>
        usedbysnapshots  NO       NO   <size>
        userrefs         NO       NO   <count>
        written          NO       NO   <size>
        aclinherit      YES      YES   discard | noallow | restricted | passthrough | passthrough-x
        aclmode         YES      YES   discard | groupmask | passthrough | restricted
        acltype         YES      YES   off | nfsv4 | posix
        atime           YES      YES   on | off
        canmount        YES       NO   on | off | noauto
        casesensitivity  NO      YES   sensitive | insensitive | mixed
        checksum        YES      YES   on | off | fletcher2 | fletcher4 | sha256 | sha512 | skein | edonr
        compression     YES      YES   on | off | lzjb | gzip | gzip-[1-9] | zle | lz4 | zstd | zstd-[1-19] | zstd-fast | zstd-fast-[1-10,20,30,40,50,60,70,80,90,100,500,1000]
        context         YES       NO   <selinux context>
        copies          YES      YES   1 | 2 | 3
        dedup           YES      YES   on | off | verify | sha256[,verify] | sha512[,verify] | skein[,verify] | edonr,verify
        defcontext      YES       NO   <selinux defcontext>
        devices         YES      YES   on | off
        dnodesize       YES      YES   legacy | auto | 1k | 2k | 4k | 8k | 16k
        encryption       NO      YES   on | off | aes-128-ccm | aes-192-ccm | aes-256-ccm | aes-128-gcm | aes-192-gcm | aes-256-gcm
        exec            YES      YES   on | off
        filesystem_limit YES       NO   <count> | none
        fscontext       YES       NO   <selinux fscontext>
        keyformat        NO       NO   none | raw | hex | passphrase
        keylocation     YES       NO   prompt | <file URI> | <https URL> | <http URL>
        logbias         YES      YES   latency | throughput
        mlslabel        YES      YES   <sensitivity label>
        mountpoint      YES      YES   <path> | legacy | none
        nbmand          YES      YES   on | off
        normalization    NO      YES   none | formC | formD | formKC | formKD
        overlay         YES      YES   on | off
        pbkdf2iters      NO       NO   <iters>
        primarycache    YES      YES   all | none | metadata
        quota           YES       NO   <size> | none
        readonly        YES      YES   on | off
        recordsize      YES      YES   512 to 1M, power of 2
        redundant_metadata YES      YES   all | most
        refquota        YES       NO   <size> | none
        refreservation  YES       NO   <size> | none
        relatime        YES      YES   on | off
        reservation     YES       NO   <size> | none
        rootcontext     YES       NO   <selinux rootcontext>
        secondarycache  YES      YES   all | none | metadata
        setuid          YES      YES   on | off
        sharenfs        YES      YES   on | off | NFS share options
        sharesmb        YES      YES   on | off | SMB share options
        snapdev         YES      YES   hidden | visible
        snapdir         YES      YES   hidden | visible
        snapshot_limit  YES       NO   <count> | none
        special_small_blocks YES      YES   zero or 512 to 1M, power of 2
        sync            YES      YES   standard | always | disabled
        utf8only         NO      YES   on | off
        version         YES       NO   1 | 2 | 3 | 4 | 5 | current
        volblocksize     NO      YES   512 to 128k, power of 2
        volmode         YES      YES   default | full | geom | dev | none
        volsize         YES       NO   <size>
        vscan           YES      YES   on | off
        xattr           YES      YES   on | off | dir | sa
        zoned           YES      YES   on | off
        userused@...     NO       NO   <size>
        groupused@...    NO       NO   <size>
        projectused@...  NO       NO   <size>
        userobjused@...  NO       NO   <size>
        groupobjused@...  NO       NO   <size>
        projectobjused@...  NO       NO   <size>
        userquota@...   YES       NO   <size> | none
        groupquota@...  YES       NO   <size> | none
        projectquota@... YES       NO   <size> | none
        userobjquota@... YES       NO   <size> | none
        groupobjquota@... YES       NO   <size> | none
        projectobjquota@... YES       NO   <size> | none
        written@<snap>   NO       NO   <size>
        written#<bookmark>  NO       NO   <size>

Sizes are specified in bytes with standard units such as K, M, G, etc.

User-defined properties can be specified by using a name containing a colon (:).

The {user|group|project}[obj]{used|quota}@ properties must be appended with
a user|group|project specifier of one of these forms:
    POSIX name      (eg: ""matt"")
    POSIX id        (eg: ""126829"")
    SMB name@domain (eg: ""matt@sun"")
    SMB SID         (eg: ""S-1-234-567-89"")
";
        #endregion

        [TestMethod]
        public void ParseZfsTestShouldParseWithoutErrors()
        {
            var properties = PropertiesParser.FromStdOutput(zfsResponse, 4);
            Assert.IsNotNull(properties);
            Assert.IsNotNull(properties);
            // We only list editable, those where EDITABLE=YES
            Assert.AreEqual(53, properties.Count);
            Assert.AreEqual(properties.First().Name, "aclinherit");
            Assert.AreEqual(properties.Last().Name, "projectobjquota@...");
        }
        [TestMethod]
        public void ParseZpoolTestShouldParseWithoutErrors()
        {
            var properties = PropertiesParser.FromStdOutput(zpoolResponse, 3);
            Assert.IsNotNull(properties);
            // We only list editable, those where EDITABLE=YES
            Assert.AreEqual(16, properties.Count);
            Assert.AreEqual(properties.First().Name, "altroot");
            Assert.AreEqual(properties.Last().Name, "feature@...");
        }
    }
}
