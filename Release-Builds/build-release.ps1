#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"
$CURRENTPATH = $pwd.Path


function delete_files_and_folders([string]$path) {
	If (Test-Path $path) {
		Write-Host "Deleting path $path" -ForegroundColor Green
		Remove-Item -recurse -force $path
	}
}

delete_files_and_folders "$CURRENTPATH/build-output"


# BUILD
cd ../src
dotnet restore Majorsilence.Winforms.MPlayerControl.sln
dotnet build Majorsilence.Winforms.MPlayerControl.sln -p:Configuration="Release"


# TESTS
cd "$CURRENTPATH/../src/MplayerUnitTests/bin/Release/net6.0/"

dotnet vstest "$CURRENTPATH/../src/MplayerUnitTests/bin/Release/net6.0/MplayerUnitTests.dll" --logger:"nunit;LogFileName=$CURRENTPATH/../src/MplayerUnitTests/bin/Release/net6.0/nunit-result.xml"

cd "$CURRENTPATH"
echo "tests finished"


# OLD SCHOOL PACKAGE
$PACKAGEDIR="MPlayerControl-dot-net-6.0"
mkdir -p "./build-output/$PACKAGEDIR"

Copy-Item ../src/LibImages/bin/Release/netstandard2.0/LibImages.dll -Destination "./build-output/$PACKAGEDIR/LibImages.dll"
Copy-Item ../src/LibMPlayerCommon/bin/Release/netstandard2.0/LibMplayerCommon.dll -Destination "./build-output/$PACKAGEDIR/LibMplayerCommon.dll"
Copy-Item ../src/MediaPlayer/bin/Release/net6.0-windows/MediaPlayer.exe -Destination "./build-output/$PACKAGEDIR/MediaPlayer.exe"
Copy-Item ../src/LibMPlayerWinform/bin/Release/net6.0-windows/LibMPlayerWinform.dll -Destination "./build-output/$PACKAGEDIR/LibMPlayerWinform.dll"
Copy-Item ../src/SlideShow/bin/Release/net6.0-windows/SlideShow.exe -Destination "./build-output/$PACKAGEDIR/SlideShow.exe"


cd build-output
7za a -t7z "$PACKAGEDIR.7z" -r $PACKAGEDIR -bd
cd ..

# NUGET 

cd "$CURRENTPATH/../src"
Get-ChildItem -Recurse *.nupkg | Copy-Item -Destination "$CURRENTPATH\build-output"

cd "$CURRENTPATH"



