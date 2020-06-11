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

#if ENGINEONLYBUILD
[assembly: InternalsVisibleTo("ConsoleRunner")]
#else
[assembly: InternalsVisibleTo("ProjectGenerationTests, PublicKey=002400000480000094000000060200000024000052534131000400000100010045d010f548ed5d0684f82179d503d442de13041d9799c444a0d011bd79e850c9e2d31a0383f8fc4442ee8e8590bde385acce695dd48ce313eae4fe72e5798ccea7a74056c4750c393e25fd69912f8926ef625c8589cd5694e3a9741549aa7fd67eedb3511ed70fe61ee4dcfee421201a42eb8083b275ecb70563ebb37b56c5aa")]
[assembly: InternalsVisibleTo("JustDecompile, PublicKey=002400000480000094000000060200000024000052534131000400000100010045d010f548ed5d0684f82179d503d442de13041d9799c444a0d011bd79e850c9e2d31a0383f8fc4442ee8e8590bde385acce695dd48ce313eae4fe72e5798ccea7a74056c4750c393e25fd69912f8926ef625c8589cd5694e3a9741549aa7fd67eedb3511ed70fe61ee4dcfee421201a42eb8083b275ecb70563ebb37b56c5aa")]
#endif