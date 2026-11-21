@{
    RootModule           = 'PSParquetTools.dll'
    ModuleVersion        = '0.1.0'
    GUID                 = '6f1c2a54-3b7e-4d0a-9c58-2e4a7d1b9f30'
    Author               = 'ParquetTools contributors'
    Description          = 'PowerShell cmdlets to read Apache Parquet files.'
    PowerShellVersion    = '5.1'
    FunctionsToExport    = @()
    CmdletsToExport      = @('Read-Parquet', 'Read-ParquetSchema')
    VariablesToExport    = @()
    AliasesToExport      = @()
    PrivateData          = @{
        PSData = @{
            Tags = @('Parquet', 'Data')
        }
    }
}
