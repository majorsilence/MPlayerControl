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

Copy-Item ../src/Majorsilence.Media.Images/bin/Release/netstandard2.0/Majorsilence.Media.Images.dll -Destination "./build-output/$PACKAGEDIR/Majorsilence.Media.Images.dll"
Copy-Item ../src/Majorsilence.Media.Videos/bin/Release/netstandard2.0/Majorsilence.Media.Videos.dll -Destination "./build-output/$PACKAGEDIR/Majorsilence.Media.Videos.dll"
Copy-Item ../src/Majorsilence.Media.Player/bin/Release/net6.0-windows/Majorsilence.Media.Player.exe -Destination "./build-output/$PACKAGEDIR/Majorsilence.Media.Player.exe"
Copy-Item ../src/Majorsilence.Media.PlayerControls/bin/Release/net6.0-windows/Majorsilence.Media.PlayerControls.dll -Destination "./build-output/$PACKAGEDIR/Majorsilence.Media.PlayerControls.dll"
Copy-Item ../src/Majorsilence.Media.SlideShow/bin/Release/net6.0-windows/Majorsilence.Media.SlideShow.exe -Destination "./build-output/$PACKAGEDIR/Majorsilence.Media.SlideShow.exe"


cd build-output
7za a -t7z "$PACKAGEDIR.7z" -r $PACKAGEDIR -bd
cd ..

# NUGET 

cd "$CURRENTPATH/../src"
Get-ChildItem -Recurse *.nupkg | Copy-Item -Destination "$CURRENTPATH\build-output"

cd "$CURRENTPATH"



