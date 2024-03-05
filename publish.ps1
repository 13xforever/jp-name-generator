#/usr/bin/pwsh
dotnet publish ./JpNameGenerator/JpNameGenerator.csproj -c Release -r win-x64 -p:PublishTrimmed=true -p:PublishAot=True -o ./distrib