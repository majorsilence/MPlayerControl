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
if [ ! -f ./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe ]; then
	nuget "Install" "NUnit.Console" "-OutputDirectory" "packages" "-Version" "3.8.0" "-ExcludeVersion"
fi

read osversion junk <<< $(getos; echo $?)
if [ "$osversion" = 'windows' ];
then
	MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
	"$MSBUILD" "../MPlayerControl.sln" /property:Configuration="Release";Platform="Any CPU"
else
	#echo "linux"
	"$MSBUILD" "../MPlayerControl.sln" /p:Configuration="Release";Platform="Any CPU"
fi

CURRENTPATH=`pwd`
if [ "$osversion" = 'windows' ];
then
	./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe "../MplayerUnitTests/bin/Release/MplayerUnitTests.exe" -result:"nunit-result.xml" -labels:All
else
	# for some reason --x86 is need with mono/linux even when built as Any CPU.
	mono ./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe "../MplayerUnitTests/bin/Release/MplayerUnitTests.exe" -result:"$CURRENTPATH/nunit-result.xml" -labels:All -workers=1 --x86 #-inprocess
fi
PACKAGEDIR="MPlayerControl-dot-net-4.7-AnyCPU"

rm -rf ./build-output/MPlayerControl-dot-net-4.7-AnyCPU
rm -rf "./build-output/$PACKAGEDIR.zip"
mkdir -p ./build-output/MPlayerControl-dot-net-4.7-AnyCPU

cp ../MediaPlayer/bin/Release/LibImages.dll "./build-output/$PACKAGEDIR/LibImages.dll"
cp ../MediaPlayer/bin/Release/LibMplayerCommon.dll "./build-output/$PACKAGEDIR/LibMplayerCommon.dll"
cp ../MediaPlayer/bin/Release/LibMplayerCommon.xml "./build-output/$PACKAGEDIR/LibMplayerCommon.xml"
cp ../MediaPlayer/bin/Release/MediaPlayer.exe "./build-output/$PACKAGEDIR/MediaPlayer.exe"
cp ../LibMPlayerWinform/bin/Release/LibMPlayerWinform.dll "./build-output/$PACKAGEDIR/LibMPlayerWinform.dll"
cp ../LibMPlayerWinform/bin/Release/LibMPlayerWinform.xml "./build-output/$PACKAGEDIR/LibMPlayerWinform.xml"
cp ../SlideShow/bin/Release/SlideShow.exe "./build-output/$PACKAGEDIR/SlideShow.exe"


cd build-output
zip -r "$PACKAGEDIR.zip" "$PACKAGEDIR"
cd ..

# nuget stuff
rm -rf "./build-output/*.nupkg"

# Begin Core Nuget Package
cd nuget
cd MPlayerControl
rm -rf lib/net45
rm -rf content
mkdir -p lib/net45
mkdir content

cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibImages.dll" lib/net45/LibImages.dll
cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibMplayerCommon.dll" lib/net45/LibMplayerCommon.dll
cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibMplayerCommon.xml" lib/net45/LibMplayerCommon.xml
cp "$CURRENTPATH/build-output/$PACKAGEDIR/MediaPlayer.exe" content/MediaPlayer.exe

nuget pack "$CURRENTPATH/nuget/MPlayerControl/MPlayerControl.nuspec" -OutputDirectory "$CURRENTPATH/build-output"

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

