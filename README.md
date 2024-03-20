# CodemerxDecompile

## Overview 

CodemerxDecompile is a truly multi-platform (Windows/MAC/Linux) .NET decompiler. It is based on the [fastest .NET decompiler engine](https://github.com/codemerx/JustDecompileEngine).

CodemerxDecompile is maintained by the original JustDecompile creators and it is a continuation of [JustDecompile Engine](https://github.com/codemerx/JustDecompileEngine). Sadly, the original JustDecompile seems to have gone out of support after 2018. Now it returns big time with CodemerxDecompile.

## Quick Start Guide

### Windows
1. Extract the archive
2. Start `CodemerxDecompile.exe`

### Linux
1. Extract the archive using `mkdir CodemerxDecompile && tar -xzpf ./CodemerxDecompile-linux-x64.tar.gz -C CodemerxDecompile`
2. Start the app using `./CodemerxDecompile/CodemerxDecompile`

### MacOS
1. Extract the archive using `tar -xzpf ./CodemerxDecompile-macos-arm64.tar.gz`
2. Remove the quarantine attribute using `xattr -d com.apple.quarantine CodemerxDecompile.app`. This is necessary as the app is not signed and MacOS reports it as "CodemerxDecompile is damaged and can't be opened.".
3. Start `CodemerxDecompile.app`

## License

This project is [AGPL](https://github.com/codemerx/CodemerxDecompile/blob/master/COPYING) licensed. It includes [JustDecompile Engine](https://github.com/codemerx/JustDecompileEngine) and [Mono.Cecil](https://github.com/jbevain/cecil) libraries which are AGPL and MIT licensed respectively.

## History

Versions < 1.0 are using a trimmed down version of MS Visual Studio Code as UI. Versions > 1.0 have custom built UI based on Avalonia UI. 
