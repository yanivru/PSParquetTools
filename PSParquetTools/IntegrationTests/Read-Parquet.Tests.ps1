BeforeAll {
    $modulePath = Join-Path $PSScriptRoot '..\bin\Release\netstandard2.0\ParquetTools.psd1'
    Import-Module $modulePath -Force
    $script:flat = Join-Path $PSScriptRoot 'flat.parquet'
    $script:nested = Join-Path $PSScriptRoot 'nested.parquet'
    $script:multi = Join-Path $PSScriptRoot 'multi.parquet'
}

Describe 'Read-Parquet' {
    Context 'flat file' {
        It 'returns one object per row with values' {
            $rows = @(Read-Parquet -Path $flat)
            $rows.Count | Should -Be 3
            $rows[0].id | Should -Be 1
            $rows[0].name | Should -Be 'alice'
            $rows[0].score | Should -Be 1.5
            $rows[0].ts | Should -BeOfType [datetime]
        }
        It 'maps null to $null' {
            (Read-Parquet -Path $flat)[1].name | Should -BeNullOrEmpty
        }
        It 'selects only requested columns' {
            $row = Read-Parquet -Path $flat -Column id, name | Select-Object -First 1
            ($row.PSObject.Properties.Name -join ',') | Should -Be 'id,name'
        }
        It 'writes an error for an unknown column' {
            $r = Read-Parquet -Path $flat -Column nope -ErrorVariable err -ErrorAction SilentlyContinue
            $r | Should -BeNullOrEmpty
            $err.Count | Should -BeGreaterThan 0
        }
        It 'writes an error for a missing file' {
            Read-Parquet -Path (Join-Path $PSScriptRoot 'missing.parquet') -ErrorVariable err -ErrorAction SilentlyContinue | Out-Null
            $err.Count | Should -BeGreaterThan 0
        }
        It 'accepts multiple paths from the pipeline' {
            @($flat, $multi | Read-Parquet).Count | Should -Be 9
        }
    }
    Context 'multiple row groups' {
        It 'returns all rows in order' {
            $ids = @(Read-Parquet -Path $multi).id
            $ids -join ',' | Should -Be '1,2,3,4,5,6'
        }
    }
    Context 'nested file' {
        BeforeAll { $script:row = Read-Parquet -Path $nested | Select-Object -First 1 }
        It 'maps struct to PSObject' {
            $row.addr.city | Should -Be 'paris'
            $row.addr.zip | Should -Be '75001'
        }
        It 'maps list to array' {
            @($row.tags) | Should -Be @('a', 'b')
        }
        It 'maps map to hashtable' {
            $row.attrs | Should -BeOfType [System.Collections.IDictionary]
            $row.attrs['k1'] | Should -Be 'v1'
        }
        It 'selects a nested column' {
            $r = Read-Parquet -Path $nested -Column addr | Select-Object -First 1
            ($r.PSObject.Properties.Name -join ',') | Should -Be 'addr'
            $r.addr.city | Should -Be 'paris'
        }
    }
}

Describe 'Read-ParquetSchema' {
    It 'lists columns and types' {
        $s = @(Read-ParquetSchema -Path $flat)
        ($s.Name -join ',') | Should -Be 'id,name,score,ts'
        $s[0].Type | Should -Not -BeNullOrEmpty
        $s[1].IsNullable | Should -BeTrue
    }
}
