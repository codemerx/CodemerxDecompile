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

    /* AGPL */
    public enum NativeOSOverrideCode
	{
		Windows = 0,
		Linux = 0x7B79,
		Apple = 0x4644
    }

	public enum BaseTargetArchitecture
	{
        I386 = 0x014c,
        AMD64 = 0x8664,
        IA64 = 0x0200,
        ARM = 0x01c0,
        ARMv7 = 0x01c4,
        ARM64 = 0xaa64
    }

    public enum TargetArchitecture {
		// Applying bb40c2108ecf303691d0536c4f9d3b9035790c5c from jbevain/cecil
		I386Windows = BaseTargetArchitecture.I386 ^ NativeOSOverrideCode.Windows,
		AMD64Windows = BaseTargetArchitecture.AMD64 ^ NativeOSOverrideCode.Windows,
		IA64Windows = BaseTargetArchitecture.IA64 ^ NativeOSOverrideCode.Windows,
		AnyCPU, /*Telerik Authorship*/
		ARMWindows = BaseTargetArchitecture.ARM ^ NativeOSOverrideCode.Windows,
		ARMv7Windows = BaseTargetArchitecture.ARMv7 ^ NativeOSOverrideCode.Windows,
		ARM64Windows = BaseTargetArchitecture.ARM64 ^ NativeOSOverrideCode.Windows,

        I386Linux = BaseTargetArchitecture.I386 ^ NativeOSOverrideCode.Linux,
        AMD64Linux = BaseTargetArchitecture.AMD64 ^ NativeOSOverrideCode.Linux,
        IA64Linux = BaseTargetArchitecture.IA64 ^ NativeOSOverrideCode.Linux,
        ARMLinux = BaseTargetArchitecture.ARM ^ NativeOSOverrideCode.Linux,
        ARMv7Linux = BaseTargetArchitecture.ARMv7 ^ NativeOSOverrideCode.Linux,
        ARM64Linux = BaseTargetArchitecture.ARM64 ^ NativeOSOverrideCode.Linux,

        I386Apple = BaseTargetArchitecture.I386 ^ NativeOSOverrideCode.Apple,
        AMD64Apple = BaseTargetArchitecture.AMD64 ^ NativeOSOverrideCode.Apple,
        IA64Apple = BaseTargetArchitecture.IA64 ^ NativeOSOverrideCode.Apple,
        ARMApple = BaseTargetArchitecture.ARM ^ NativeOSOverrideCode.Apple,
        ARMv7Apple = BaseTargetArchitecture.ARMv7 ^ NativeOSOverrideCode.Apple,
        ARM64Apple = BaseTargetArchitecture.ARM64 ^ NativeOSOverrideCode.Apple
    }
    /* End AGPL */

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
