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


nuget restore ../MPlayerControl.sln -NonInteractive
nuget restore ../MPlayerGtkWidget.sln -NonInteractive
if [ ! -f ./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe ]; then
	nuget "Install" "NUnit.Console" "-OutputDirectory" "packages" "-Version" "3.8.0" "-ExcludeVersion"
fi

read osversion junk <<< $(getos; echo $?)

nuget restore ../WpfMPlayer.sln -NonInteractive
MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
"$MSBUILD" "../WpfMPlayer.sln" /property:Configuration="Release";Platform="Any CPU"

CURRENTPATH=`pwd`
PACKAGEDIR="MPlayerControl-dot-net-4.7-AnyCPU-wpf"

rm -rf ./build-output/MPlayerControl-dot-net-4.7-AnyCPU
rm -rf "./build-output/$PACKAGEDIR.zip"
mkdir -p ./build-output/MPlayerControl-dot-net-4.7-AnyCPU

cp ../WpfMPlayer/bin/Release/WpfMPlayer.exe "./build-output/$PACKAGEDIR/WpfMPlayer.exe"


cd build-output
zip -r "$PACKAGEDIR.zip" "$PACKAGEDIR"
cd ..

# nuget stuff
cp "$CURRENTPATH/build-output/$PACKAGEDIR/WpfMPlayer.exe" content/WpfMPlayer.exe

cd "$CURRENTPATH"
# End Core Nuget Package


# Begin winform Nuget Package
cd nuget
cd MPlayerControl-Winform
rm -rf lib/net45
mkdir -p lib/net45

cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibMPlayerWinform.dll" lib/net45/LibMPlayerWinform.dll
cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibMPlayerWinform.xml" lib/net45/LibMPlayerWinform.xml

nuget pack "$CURRENTPATH/nuget/MPlayerControl-Winform/MPlayerControl-Winform.nuspec" -OutputDirectory "$CURRENTPATH/build-output"
cd "$CURRENTPATH"
# End winform Nuget Package


