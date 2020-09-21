//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System;

namespace Mono.Cecil {

	public enum ModuleKind {
		Dll,
		Console,
		Windows,
		NetModule,
	}

	public enum TargetArchitecture {
		/* AGPL */
		// Applying bb40c2108ecf303691d0536c4f9d3b9035790c5c from jbevain/cecil
		I386 = 0x014c,
		AMD64 = 0x8664,
		IA64 = 0x0200,
		/* End AGPL */
		AnyCPU, /*Telerik Authorship*/
		/* AGPL */
		ARM = 0x01c0,
		ARMv7 = 0x01c4,
		ARM64 = 0xaa64
		/* End AGPL */
	}

	[Flags]
	public enum ModuleAttributes {
		ILOnly = 1,
		Required32Bit = 2,
		StrongNameSigned = 8,
		Preferred32Bit = 0x00020000,
	}

	[Flags]
	public enum ModuleCharacteristics {
		HighEntropyVA = 0x0020,
		DynamicBase = 0x0040,
		NoSEH = 0x0400,
		NXCompat = 0x0100,
		AppContainer = 0x1000,
		TerminalServerAware = 0x8000,
	}
}
