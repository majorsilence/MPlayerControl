
CURRENTPATH=`pwd`

# Increment minor number by 1
# See http://www.codeproject.com/Articles/31236/How-To-Update-Assembly-Version-Number-Automaticall
mono ./tools/AssemblyInfoUtil.exe -inc:2 "$CURRENTPATH/../LibImages/Properties/AssemblyInfo.cs"
mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../LibMPlayerCommon/Properties/AssemblyInfo.cs"
mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../MediaPlayer/Properties/AssemblyInfo.cs"
mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../LibMPlayerWinform/Properties/AssemblyInfo.cs"
mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../MPlayerGtkWidget/AssemblyInfo.cs"
mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../SlideShow/Properties/AssemblyInfo.cs"
mono ./tools/AssemblyInfoUtil.exe  -inc:2 "$CURRENTPATH/../WpfMPlayer/Properties/AssemblyInfo.cs"


VERSION_FULL_LINE=$(grep -F "AssemblyVersion" "$CURRENTPATH/../LibMPlayerCommon/Properties/AssemblyInfo.cs")
VERSION_WITH_ASTERIK=$(echo $VERSION_FULL_LINE | awk -F\" '{print $(NF-1)}')
VERSION="${VERSION_WITH_ASTERIK%??}"


cd ..
cd Release-Builds

# Update NUGET spec files
sed -i "5s/.*/ <version>$VERSION<\/version>/" "$CURRENTPATH/nuget/MPlayerControl/MPlayerControl.nuspec"
sed -i "5s/.*/ <version>$VERSION<\/version>/" "$CURRENTPATH/nuget/MPlayerControl-Winform/MPlayerControl-Winform.nuspec"
sed -i "5s/.*/ <version>$VERSION<\/version>/" "$CURRENTPATH/nuget/MPlayerControl-Gtk/MPlayerControl-Gtk.nuspec"


