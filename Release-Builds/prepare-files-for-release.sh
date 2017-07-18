#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

CURRENTPATH=`pwd`


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

read osversion junk <<< $(getos; echo $?)
if [ "$osversion" = 'windows' ];
then
# Increment minor number by 1
# See http://www.codeproject.com/Articles/31236/How-To-Update-Assembly-Version-Number-Automaticall
    ./tools/AssemblyInfoUtil.exe -inc:2 "$CURRENTPATH/../LibImages/Properties/AssemblyInfo.cs"
    ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../LibMPlayerCommon/Properties/AssemblyInfo.cs"
    ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../MediaPlayer/Properties/AssemblyInfo.cs"
    ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../LibMPlayerWinform/Properties/AssemblyInfo.cs"
    ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../MPlayerGtkWidget/AssemblyInfo.cs"
    ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../SlideShow/Properties/AssemblyInfo.cs"
    ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../WpfMPlayer/Properties/AssemblyInfo.cs"
else
    mono ./tools/AssemblyInfoUtil.exe -inc:2 "$CURRENTPATH/../LibImages/Properties/AssemblyInfo.cs"
    mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../LibMPlayerCommon/Properties/AssemblyInfo.cs"
    mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../MediaPlayer/Properties/AssemblyInfo.cs"
    mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../LibMPlayerWinform/Properties/AssemblyInfo.cs"
    mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../MPlayerGtkWidget/AssemblyInfo.cs"
    mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../SlideShow/Properties/AssemblyInfo.cs"
    mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../WpfMPlayer/Properties/AssemblyInfo.cs"
fi

VERSION_FULL_LINE=$(grep -F "AssemblyVersion" "$CURRENTPATH/../LibMPlayerCommon/Properties/AssemblyInfo.cs")
VERSION_WITH_ASTERIK=$(echo $VERSION_FULL_LINE | awk -F\" '{print $(NF-1)}')
VERSION="${VERSION_WITH_ASTERIK%??}"


cd ..
cd Release-Builds

# Update NUGET spec files
sed -i "5s/.*/    <version>$VERSION<\/version>/" "$CURRENTPATH/nuget/MPlayerControl/MPlayerControl.nuspec"
sed -i "5s/.*/    <version>$VERSION<\/version>/" "$CURRENTPATH/nuget/MPlayerControl-Winform/MPlayerControl-Winform.nuspec"
sed -i "5s/.*/    <version>$VERSION<\/version>/" "$CURRENTPATH/nuget/MPlayerControl-Gtk/MPlayerControl-Gtk.nuspec"


