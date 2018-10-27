#!/bin/bash
output=$(dotnet ./GitVersion/GitVersion.dll | grep -Eo "\"MajorMinorPatch\":\"([0-9]+\.[0-9]+\.[0-9]+)\"" | cut -d: -f2)
echo "Setting version number to ${output}"
echo "##vso[task.setvariable variable=VersionNumber]${output//\"}"
