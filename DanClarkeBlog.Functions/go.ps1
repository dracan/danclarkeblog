try {
    dotnet restore
    # & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
    dotnet build
    Push-Location .\bin\Debug\net462
    func host start
}
finally {
    Pop-Location
}
