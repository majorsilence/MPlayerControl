#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

MSBUILD="msbuild"

getos()
{
	if [ "$(expr $(uname -s))" = "windows32" ]; then
		echo "windows"
	elif [ "$(expr substr $(uname -s) 1 5)" = "MINGW" ]; then
		echo "windows"
	else
	    echo "linux"
	fi
}

nuget restore ../MPlayerGtkWidget.sln -NonInteractive
if [ ! -f ./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe ]; then
	nuget "Install" "NUnit.Console" "-OutputDirectory" "packages" "-Version" "3.8.0" "-ExcludeVersion"
fi

read osversion junk <<< $(getos; echo $?)
if [ "$osversion" = 'windows' ];
then
	nuget restore ../WpfMPlayer.sln -NonInteractive
	MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
	"$MSBUILD" "../MPlayerGtkWidget.sln" /property:Configuration="Release";Platform="Any CPU"
else
	#echo "linux"
	"$MSBUILD" "../MPlayerGtkWidget.sln" /p:Configuration="Release";Platform="Any CPU"
fi

CURRENTPATH=`pwd`
PACKAGEDIR="MPlayerControl-dot-net-4.7-AnyCPU-gtk"

rm -rf ./build-output/MPlayerControl-dot-net-4.7-AnyCPU
rm -rf "./build-output/$PACKAGEDIR.zip"
mkdir -p ./build-output/MPlayerControl-dot-net-4.7-AnyCPU

cp ../MPlayerGtkWidget/bin/Release/MPlayerGtkWidget.dll "./build-output/$PACKAGEDIR/MPlayerGtkWidget.dll"


cd build-output
zip -r "$PACKAGEDIR.zip" "$PACKAGEDIR"
cd ..


cd "$CURRENTPATH"
# End Core Nuget Package

# Begin gtk Nuget Package
cd nuget
cd MPlayerControl-Gtk
rm -rf lib/net45
mkdir -p lib/net45

cp "$CURRENTPATH/build-output/$PACKAGEDIR/MPlayerGtkWidget.dll" lib/net45/MPlayerGtkWidget.dll

nuget pack "$CURRENTPATH/nuget/MPlayerControl-Gtk/MPlayerControl-Gtk.nuspec" -OutputDirectory "$CURRENTPATH/build-output"
cd "$CURRENTPATH"
# End winform Nuget Package
