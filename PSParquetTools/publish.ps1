Write-Output "Publishing ParquetTools to the PowerShell Gallery"

$source = Join-Path $PSScriptRoot 'bin\Release\netstandard2.0\publish'
$target = Join-Path $PSScriptRoot 'bin\Release\netstandard2.0\publish\ParquetTools'

if (Test-Path $target) { Remove-Item $target -Recurse -Force }
New-Item -ItemType Directory -Path $target | Out-Null
Get-ChildItem $source -File | Copy-Item -Destination $target

Publish-Module -Path $target -NuGetApiKey $env:NUGETAPIKEY
