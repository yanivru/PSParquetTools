# PSParquetTools
PowerShell cmdlets to read Apache Parquet files.

## Installing
Install-Module ParquetTools

## Basic usage
    Read-Parquet .\data.parquet

## Selecting columns
    Read-Parquet -Path .\data.parquet -Column id, name

Column selection filters the output; each row group is still read in full.

## Filtering / first rows
    Read-Parquet .\data.parquet | Where-Object { $_.name -like 'a*' }
    Read-Parquet .\data.parquet | Select-Object -First 10

## Multiple files
    Get-ChildItem *.parquet | Read-Parquet

## Export to CSV
    Read-Parquet .\data.parquet | Export-Csv data.csv -NoTypeInformation

## Schema
    Read-ParquetSchema .\data.parquet

## Nested data
Structs become PSObjects, lists become arrays and maps become hashtables.

## Building
    dotnet build PSParquetTools/PSParquetTools.csproj -c Release
    dotnet run --project tools/SampleGen   # regenerates test sample files
    Invoke-Pester ./PSParquetTools/IntegrationTests
