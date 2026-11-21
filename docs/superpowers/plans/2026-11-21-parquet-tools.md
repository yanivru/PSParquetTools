# ParquetTools Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** PowerShell binary module `ParquetTools` with `Read-Parquet` and `Read-ParquetSchema`.

**Architecture:** C# netstandard2.0 cmdlets. Per row group, read the selected leaf columns with Parquet.Net, assemble nested rows with `Parquet.Rows.Table(schema, columns, rowCount)`, and map each `Row` to a `PSObject` via `ParquetRowMapper`.

**Tech Stack:** C#, Parquet.Net 4.25.0 (last 4.x, has the Table API), PowerShellStandard.Library 5.1.1, Pester 5.

**Constraint:** No employer-related data anywhere. Neutral/personal author. Do not commit with the global git identity.

---

## File structure
- `PSParquetTools/PSParquetTools.csproj`, `ParquetRowMapper.cs`, `ReadParquetCmdlet.cs`, `ReadParquetSchemaCmdlet.cs`, `ParquetTools.psd1`, `publish.ps1`
- `tools/SampleGen/`: console project writing `flat.parquet`, `nested.parquet`, `multi.parquet` into `PSParquetTools/IntegrationTests/`
- `PSParquetTools/IntegrationTests/Read-Parquet.Tests.ps1`
- `.github/workflows/build.yml`, `README.md`, `LICENSE`, `.gitignore`

### Task 1: Scaffold + sample generator
- [ ] Create csproj, sln, `.gitignore`, and SampleGen. Files: flat (id, name nullable, score, ts), nested (id, struct addr{city,zip}, list tags, map attrs), multi (3 row groups x 2 rows). Generic data only.
- [ ] `dotnet run --project tools/SampleGen`; expect three files.

### Task 2: Failing Pester tests
- [ ] Cases: flat values; null is `$null`; `-Column` subset; unknown column errors; nested struct/list/map; multi-row-group order; missing path error; pipeline of two paths; `Read-ParquetSchema` names/types.
- [ ] `Invoke-Pester` fails (cmdlets missing).

### Task 3: Mapper + cmdlets
- [ ] `ParquetRowMapper`: Row → PSObject, IDictionary → hashtable, IEnumerable (not string/byte[]) → object[], null/DBNull → `$null`.
- [ ] `Read-Parquet`: resolve wildcards, validate `-Column`, sub-schema of selected top-level fields, per row group read leaf columns, `Table`, emit rows; per-file try/catch with `WriteError`.
- [ ] `Read-ParquetSchema`: Name, Type, IsNullable, Children.
- [ ] Build Release, Pester passes.

### Task 4: Packaging and docs
- [ ] psd1, publish.ps1, GitHub workflow, README, MIT LICENSE ("ParquetTools contributors").
- [ ] Grep for employer names: no matches.
- [ ] Commit only after a personal git identity is configured locally.
