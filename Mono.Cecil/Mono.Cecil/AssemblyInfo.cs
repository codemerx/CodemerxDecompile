//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle ("Mono.Cecil")]

[assembly: Guid ("fd225bb4-fa53-44b2-a6db-85f5e48dcb54")]

#if ENGINEONLYBUILD || (JUSTASSEMBLY && !JUSTASSEMBLYSERVER)
[assembly: InternalsVisibleTo("Telerik.JustDecompile.Mono.Cecil.Pdb")]
[assembly: InternalsVisibleTo("Telerik.JustDecompile.Mono.Cecil.Mdb")]
#else
[assembly: InternalsVisibleTo("Telerik.JustDecompile.Mono.Cecil.Pdb, PublicKey=002400000480000094000000060200000024000052534131000400000100010045d010f548ed5d0684f82179d503d442de13041d9799c444a0d011bd79e850c9e2d31a0383f8fc4442ee8e8590bde385acce695dd48ce313eae4fe72e5798ccea7a74056c4750c393e25fd69912f8926ef625c8589cd5694e3a9741549aa7fd67eedb3511ed70fe61ee4dcfee421201a42eb8083b275ecb70563ebb37b56c5aa")]
[assembly: InternalsVisibleTo("Telerik.JustDecompile.Mono.Cecil.Mdb, PublicKey=002400000480000094000000060200000024000052534131000400000100010045d010f548ed5d0684f82179d503d442de13041d9799c444a0d011bd79e850c9e2d31a0383f8fc4442ee8e8590bde385acce695dd48ce313eae4fe72e5798ccea7a74056c4750c393e25fd69912f8926ef625c8589cd5694e3a9741549aa7fd67eedb3511ed70fe61ee4dcfee421201a42eb8083b275ecb70563ebb37b56c5aa")]
#endif
//[assembly: InternalsVisibleTo ("Mono.Cecil.Rocks, PublicKey=002400000480000094000000060200000024000052534131000400000100010079159977d2d03a8e6bea7a2e74e8d1afcc93e8851974952bb480a12c9134474d04062447c37e0e68c080536fcf3c3fbe2ff9c979ce998475e506e8ce82dd5b0f350dc10e93bf2eeecf874b24770c5081dbea7447fddafa277b22de47d6ffea449674a4f9fccf84d15069089380284dbdd35f46cdff12a1bd78e4ef0065d016df")]
//[assembly: InternalsVisibleTo ("Mono.Cecil.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010079159977d2d03a8e6bea7a2e74e8d1afcc93e8851974952bb480a12c9134474d04062447c37e0e68c080536fcf3c3fbe2ff9c979ce998475e506e8ce82dd5b0f350dc10e93bf2eeecf874b24770c5081dbea7447fddafa277b22de47d6ffea449674a4f9fccf84d15069089380284dbdd35f46cdff12a1bd78e4ef0065d016df")]
