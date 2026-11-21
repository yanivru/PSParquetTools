@{
    RootModule           = 'PSParquetTools.dll'
    ModuleVersion        = '0.1.0'
    GUID                 = '6f1c2a54-3b7e-4d0a-9c58-2e4a7d1b9f30'
    Author               = 'Yaniv'
    Copyright            = '(c) 2026 Yaniv'
    Description          = 'PowerShell cmdlets to read Apache Parquet files.'
    PowerShellVersion    = '5.1'
    FunctionsToExport    = @()
    CmdletsToExport      = @('Read-Parquet', 'Read-ParquetSchema')
    VariablesToExport    = @()
    AliasesToExport      = @()
    PrivateData          = @{
        PSData = @{
            Tags       = @('Parquet', 'Data')
            ProjectUri = 'https://github.com/yanivru/PSParquetTools'
            LicenseUri = 'https://github.com/yanivru/PSParquetTools/blob/main/LICENSE'
        }
    }
}
