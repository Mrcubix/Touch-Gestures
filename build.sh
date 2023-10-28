#!/bin/sh

# build the plugin
dotnet publish Touch-Gestures -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -o build/plugin

# build the desktop on linux & windows
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r linux-x64 -o build/ux/linux
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r linux-arm64 -o build/ux/linux-arm64
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r linux-arm -o build/ux/linux-arm

dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r win-x64 -o build/ux/win
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r win-x86 -o build/ux/win-x86
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r win-arm64 -o build/ux/win-arm64