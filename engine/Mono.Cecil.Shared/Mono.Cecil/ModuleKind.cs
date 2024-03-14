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
		Apple = 0x4644,
		FreeBSD = 0xADC4,
		NetBSD = 0x1993,
		Sun = 0x1992
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
		ARM64Apple = TargetArchitecture.ARM64 ^ NativeOSOverrideCode.Apple,

		I386FreeBSD = TargetArchitecture.I386 ^ NativeOSOverrideCode.FreeBSD,
		AMD64FreeBSD = TargetArchitecture.AMD64 ^ NativeOSOverrideCode.FreeBSD,
		IA64FreeBSD = TargetArchitecture.IA64 ^ NativeOSOverrideCode.FreeBSD,
		ARMFreeBSD = TargetArchitecture.ARM ^ NativeOSOverrideCode.FreeBSD,
		ARMv7FreeBSD = TargetArchitecture.ARMv7 ^ NativeOSOverrideCode.FreeBSD,
		ARM64FreeBSD = TargetArchitecture.ARM64 ^ NativeOSOverrideCode.FreeBSD,

		I386NetBSD = TargetArchitecture.I386 ^ NativeOSOverrideCode.NetBSD,
		AMD64NetBSD = TargetArchitecture.AMD64 ^ NativeOSOverrideCode.NetBSD,
		IA64NetBSD = TargetArchitecture.IA64 ^ NativeOSOverrideCode.NetBSD,
		ARMNetBSD = TargetArchitecture.ARM ^ NativeOSOverrideCode.NetBSD,
		ARMv7NetBSD = TargetArchitecture.ARMv7 ^ NativeOSOverrideCode.NetBSD,
		ARM64NetBSD = TargetArchitecture.ARM64 ^ NativeOSOverrideCode.NetBSD,

		I386Sun = TargetArchitecture.I386 ^ NativeOSOverrideCode.Sun,
		AMD64Sun = TargetArchitecture.AMD64 ^ NativeOSOverrideCode.Sun,
		IA64Sun = TargetArchitecture.IA64 ^ NativeOSOverrideCode.Sun,
		ARMSun = TargetArchitecture.ARM ^ NativeOSOverrideCode.Sun,
		ARMv7Sun = TargetArchitecture.ARMv7 ^ NativeOSOverrideCode.Sun,
		ARM64Sun = TargetArchitecture.ARM64 ^ NativeOSOverrideCode.Sun
	}
	/* End AGPL */

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
