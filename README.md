# CodemerxDecompile

## Overview 

CodemerxDecompile is a truly multi-platform (Windows/MAC/Linux) .NET decompiler. It is based on the [fastest .NET decompiler engine](https://github.com/telerik/JustDecompileEngine) and [VS Code](https://github.com/microsoft/vscode) UI.

CodemerxDecompile is maintained by the original JustDecompile creators and it is a continuation of [JustDecompile Engine](https://github.com/telerik/JustDecompileEngine). Sadly, the original JustDecompile seems to have gone out of support after 2018. Now it returns big time with CodemerxDecompile.

## Quick Start Guide

### Windows
1. Extract the archive
2. Start `CodemerxDecompile.exe`

### Linux
1. Extract the archive using `mkdir CodemerxDecompile && tar -xzpf ./CodemerxDecompile-linux-x64.tar.gz -C CodemerxDecompile`
2. Start the app using `./CodemerxDecompile/CodemerxDecompile`

### MacOS
1. Extract the archive using `tar -xzpf ./CodemerxDecompile-macos-arm64.tar`
2. Remove the quarantine attribute using `xattr -d com.apple.quarantine CodemerxDecompile.app`. This is necessary as the app is not signed and MacOS reports it as "CodemerxDecompile is damaged and can't be opened.".
3. Start `CodemerxDecompile.app`

## License

This project is [AGPL](https://github.com/codemerx/CodemerxDecompile/blob/master/COPYING) licensed. It includes [JustDecompile Engine](https://github.com/telerik/JustDecompileEngine) and [Mono.Cecil](https://github.com/jbevain/cecil) libraries which are Apache 2.0 and MIT licensed respectively. It also includes [VS Code](https://github.com/microsoft/vscode) which is MIT licensed.
