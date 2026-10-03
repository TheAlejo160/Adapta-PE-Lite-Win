$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
& dotnet run --project (Join-Path $root 'tests/AdaptaPELite.Checks/AdaptaPELite.Checks.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Comprobaciones fallidas.' }
