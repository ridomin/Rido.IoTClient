$NUPKG_DIR = "./_nupkgs"
$API_KEY = Read-Host -Prompt "NuGet API Key"

if (Test-Path $NUPKG_DIR) {
    Remove-Item $NUPKG_DIR -Force -Recurse
}

dotnet pack -c Release -o $NUPKG_DIR .\Rido.IoTClient.sln
dotnet nuget push  $NUPKG_DIR/* --api-key $API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
