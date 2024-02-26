#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

#export LC_ALL=en_US.UTF-8
CURRENTPATH=`pwd`
VERSION=`cat src/Majorsilence.Media.Web/Majorsilence.Media.Web.csproj | grep "<Version>"  | sed 's/[^0-9.]*//g'`

clean_build(){
	rm -rf ./build

	# Delete "bin" and "obj" folders
	rm -rf `find -type d -name bin`
	rm -rf `find -type d -name obj`
	rm -rf `find -type d -name TestResults`
}


remove_bom_from_file_encoding()
{
	# hack to remove character encoding BOM if it exists
	STARTING_PATH=$1
	NAME_WILDCARD=$2
	# Find the first file matching the pattern
	file=$(find $STARTING_PATH -name "*cobertura.xml" | head -n 1)

	# Check if a file was found
	if [ -n "$file" ]; then
		# Remove the BOM and overwrite the original file
		/usr/bin/sed -i '1s/^\xEF\xBB\xBF//' "$file"
		echo "BOM removed from $file"
	else
		echo "No file matching the pattern was found."
	fi
}

systemd_package_build()
{
	mkdir -p build/linux-x64
	dotnet restore src/Majorsilence.Media.Web.sln
	dotnet build -c Release src/Majorsilence.Media.Web.sln	
	dotnet build "src/Majorsilence.Media.Web" -c Release -r linux-x64
	dotnet publish "src/Majorsilence.Media.Web" -c Release -r linux-x64 --self-contained true -o build/linux-x64/media-web-linux-x64-$VERSION
	dotnet test src/Majorsilence.Media.Web.sln --collect:"XPlat Code Coverage" --logger:"nunit"
	remove_bom_from_file_encoding "./src/Majorsilence.Media.Web/TestResults" "*cobertura.xml"
	dotnet build "src/Majorsilence.Media.WorkerService" -c Release -r linux-x64
	dotnet publish "src/Majorsilence.Media.WorkerService" -c Release -r linux-x64 --self-contained true -o build/linux-x64/media-workerservice-linux-x64-$VERSION
	
	cd build/linux-x64
	tar -czvf media-web-linux-x64-$VERSION.tar.gz media-web-linux-x64-$VERSION
	tar -czvf media-workerservice-linux-x64-$VERSION.tar.gz media-workerservice-linux-x64-$VERSION
	cd $CURRENTPATH
}

clean_build
systemd_package_build
