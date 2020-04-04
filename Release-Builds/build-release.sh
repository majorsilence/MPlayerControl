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

read osversion junk <<< $(getos; echo $?)
if [ "$osversion" = 'windows' ];
then
	MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\bin\MSBuild.exe"
	"$MSBUILD" "../MPlayerControl.sln" /property:Configuration="Release";Platform="Any CPU"
else
	#echo "linux"
	"$MSBUILD" "../MPlayerControl.sln" /p:Configuration="Release";Platform="Any CPU"
fi

CURRENTPATH=`pwd`
cd "../MplayerUnitTests/bin/Release/net48/"
if [ "$osversion" = 'windows' ];
then
	echo "disable until new test videos are added"
	".MplayerUnitTests.exe" -result:"nunit-result.xml" -labels:All
else
	#echo "disable until new test videos are added"
	# for some reason --x86 is need with mono/linux even when built as Any CPU.
	echo "start linux tests"
	mono "./MplayerUnitTests.exe" -result:"$CURRENTPATH/nunit-result.xml" -labels:All -workers=1
fi
cd "$CURRENTPATH"
echo "tests finished"

PACKAGEDIR="MPlayerControl-dot-net-4.8-AnyCPU"

rm -rf "./build-output/$PACKAGEDIR"
rm -rf "./build-output/$PACKAGEDIR.7z"
mkdir -p "./build-output/$PACKAGEDIR"

cp ../LibImages/bin/Release/net45/LibImages.dll "./build-output/$PACKAGEDIR/LibImages.dll"
cp ../LibMPlayerCommon/bin/Release/net45/LibMplayerCommon.dll "./build-output/$PACKAGEDIR/LibMplayerCommon.dll"
cp ../LibMPlayerCommon/bin/Release/net45/LibMplayerCommon.xml "./build-output/$PACKAGEDIR/LibMplayerCommon.xml"
cp ../MediaPlayer/bin/Release/net48/MediaPlayer.exe "./build-output/$PACKAGEDIR/MediaPlayer.exe"
cp ../LibMPlayerWinform/bin/Release/net45/LibMPlayerWinform.dll "./build-output/$PACKAGEDIR/LibMPlayerWinform.dll"
cp ../LibMPlayerWinform/bin/Release/net45/LibMPlayerWinform.xml "./build-output/$PACKAGEDIR/LibMPlayerWinform.xml"
cp ../SlideShow/bin/Release/net48/SlideShow.exe "./build-output/$PACKAGEDIR/SlideShow.exe"


cd build-output
7za a -t7z "$PACKAGEDIR.7z" -r $PACKAGEDIR -bd
cd ..

# nuget stuff
rm -rf "./build-output/*.nupkg"

# Begin Core Nuget Package
cd nuget
cd MPlayerControl
rm -rf lib/net45
rm -rf lib/netstandard2.0
rm -rf content
mkdir -p lib/net45
mkdir -p lib/netstandard2.0
mkdir content

cp "$CURRENTPATH/../LibImages/bin/Release/net45/LibImages.dll" lib/net45/LibImages.dll
cp "$CURRENTPATH/../LibMPlayerCommon/bin/Release/net45/LibMplayerCommon.dll" lib/net45/LibMplayerCommon.dll
cp "$CURRENTPATH/../LibMPlayerCommon/bin/Release/LibMplayerCommon.xml" lib/net45/LibMplayerCommon.xml
cp "$CURRENTPATH/../MediaPlayer/bin/Release/net48/MediaPlayer.exe" content/MediaPlayer.exe
cp "$CURRENTPATH/../LibImages/bin/Release/netstandard2.0/LibImages.dll" lib/netstandard2.0/LibImages.dll
cp "$CURRENTPATH/../LibMPlayerCommon/bin/Release/netstandard2.0/LibMplayerCommon.dll" lib/netstandard2.0/LibMplayerCommon.dll
cp "$CURRENTPATH/../LibMPlayerCommon/bin/Release/LibMplayerCommon.xml" lib/netstandard2.0/LibMplayerCommon.xml
#cp "$CURRENTPATH/../lgpl-2.1.txt" files/lgpl-2.1.txt

nuget pack "$CURRENTPATH/nuget/MPlayerControl/MPlayerControl.nuspec" -OutputDirectory "$CURRENTPATH/build-output"

cd "$CURRENTPATH"
# End Core Nuget Package


# Begin winform Nuget Package
cd nuget
cd MPlayerControl-Winform
rm -rf lib/net45
mkdir -p lib/net45

cp "$CURRENTPATH/../LibMPlayerWinform/bin/Release/net45/LibMPlayerWinform.dll" lib/net45/LibMPlayerWinform.dll
cp "$CURRENTPATH/../LibMPlayerWinform/bin/Release/net45/LibMPlayerWinform.xml" lib/net45/LibMPlayerWinform.xml

nuget pack "$CURRENTPATH/nuget/MPlayerControl-Winform/MPlayerControl-Winform.nuspec" -OutputDirectory "$CURRENTPATH/build-output"
cd "$CURRENTPATH"
# End winform Nuget Package

