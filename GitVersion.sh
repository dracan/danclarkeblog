#!/bin/bash
output=$(dotnet ./GitVersion/GitVersion.dll | grep -Eo "\"MajorMinorPatch\":\"([0-9]+\.[0-9]+\.[0-9]+)\"" | cut -d: -f2)
output=${output//\"}
echo "Setting version number to ${output}"
echo "##vso[task.setvariable variable=VersionNumber]${output}"
echo "##vso[build.updatebuildnumber]${output}"
