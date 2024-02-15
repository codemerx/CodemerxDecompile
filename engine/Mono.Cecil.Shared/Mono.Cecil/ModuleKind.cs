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
        // TODO add more platforms https://github.com/dotnet/runtime/blob/61c658183231100a5836e833c86446ff51a4654b/src/coreclr/src/inc/pedecoder.h#L90-L104
    }
    public enum PlatformSpecificTargetArchitecture
    {
        I386Windows = TargetArchitecture.I386 ^ NativeOSOverrideCode.Windows,
        AMD64Windows = TargetArchitecture.AMD64 ^ NativeOSOverrideCode.Windows,
        IA64Windows = TargetArchitecture.IA64 ^ NativeOSOverrideCode.Windows,
        ARMWindows = TargetArchitecture.ARM ^ NativeOSOverrideCode.Windows,
        ARMv7Windows = TargetArchitecture.ARMv7 ^ NativeOSOverrideCode.Windows,
        ARM64Windows = TargetArchitecture.ARM64 ^ NativeOSOverrideCode.Windows,

        I386Linux = TargetArchitecture.I386 ^ NativeOSOverrideCode.Linux,
        AMD64Linux = TargetArchitecture.AMD64 ^ NativeOSOverrideCode.Linux,
        IA64Linux = TargetArchitecture.IA64 ^ NativeOSOverrideCode.Linux,
        ARMLinux = TargetArchitecture.ARM ^ NativeOSOverrideCode.Linux,
        ARMv7Linux = TargetArchitecture.ARMv7 ^ NativeOSOverrideCode.Linux,
        ARM64Linux = TargetArchitecture.ARM64 ^ NativeOSOverrideCode.Linux,

        I386Apple = TargetArchitecture.I386 ^ NativeOSOverrideCode.Apple,
        AMD64Apple = TargetArchitecture.AMD64 ^ NativeOSOverrideCode.Apple,
        IA64Apple = TargetArchitecture.IA64 ^ NativeOSOverrideCode.Apple,
        ARMApple = TargetArchitecture.ARM ^ NativeOSOverrideCode.Apple,
        ARMv7Apple = TargetArchitecture.ARMv7 ^ NativeOSOverrideCode.Apple,
        ARM64Apple = TargetArchitecture.ARM64 ^ NativeOSOverrideCode.Apple
    }

    public enum TargetArchitecture
	{
        // Applying bb40c2108ecf303691d0536c4f9d3b9035790c5c from jbevain/cecil
        I386 = 0x014c,
        AMD64 = 0x8664,
        IA64 = 0x0200,
        AnyCPU, /*Telerik Authorship*/
        ARM = 0x01c0,
        ARMv7 = 0x01c4,
        ARM64 = 0xaa64
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
