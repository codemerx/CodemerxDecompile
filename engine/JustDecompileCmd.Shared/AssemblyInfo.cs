using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("JustDecompileCmdShell")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("198493c3-3527-4dc4-8d8b-7195e95ff18d")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

/* AGPL */
#if RELEASE
[assembly: InternalsVisibleTo("ConsoleRunner, PublicKey=00240000048000009400000006020000002400005253413100040000010001008d3ec49d2c78b3f6029c19200a28c4a8dc279a9dfddcc7413c0737c73b49e08a3761e148e745fe2007d8d057a962c7eaf22e7eded052bb08f1e7f0d7794db4827b09124ffa61625879af57120f8078fad84c8c7d4f6c6ebb9ab14de089d606ca0ed66b9af0c67795fa4e34f61ce62732180d06fb67b3ec93b202f045e10a99d3")]
[assembly: InternalsVisibleTo("Decompiler.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010001be52ebf131facc15d6b5893afa9799462506e031d439d70017b9c1a87147029ee619e119d71f63222c25f0910b4a2f77d6635b110a2229375e3e1e23f337eca02258c497263d9d1191af05fff461d222da5fbf2552031dca79a5341f7e00582af029c9b0d14ba113e35180fcbab106ac896c9e902dff8a76b14535218cabb0")]
#else
[assembly: InternalsVisibleTo("ConsoleRunner")]
[assembly: InternalsVisibleTo("ConsoleRunnerDotNet6")]
[assembly: InternalsVisibleTo("Decompiler.Tests")]
/* End AGPL */
#endif
