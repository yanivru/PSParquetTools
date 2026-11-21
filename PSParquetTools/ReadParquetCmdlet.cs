using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Parquet;
using Parquet.Rows;
using Parquet.Schema;

namespace PSParquetTools
{
    [Cmdlet(VerbsCommunications.Read, "Parquet")]
    [OutputType(typeof(PSObject))]
    public class ReadParquetCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("FullName")]
        public string[] Path { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string[] Column { get; set; }

        protected override void ProcessRecord()
        {
            foreach (var path in Path)
            {
                foreach (var file in Resolve(path))
                {
                    try
                    {
                        ReadFile(file);
                    }
                    catch (PipelineStoppedException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        WriteError(new ErrorRecord(ex, "ParquetReadFailed", ErrorCategory.ReadError, file));
                    }
                }
            }
        }

        private IEnumerable<string> Resolve(string path)
        {
            try
            {
                return GetResolvedProviderPathFromPSPath(path, out _).ToList();
            }
            catch (ItemNotFoundException ex)
            {
                WriteError(new ErrorRecord(ex, "ParquetFileNotFound", ErrorCategory.ObjectNotFound, path));
                return Enumerable.Empty<string>();
            }
        }

        private void ReadFile(string file)
        {
            using (var stream = File.OpenRead(file))
            using (var reader = ParquetReader.CreateAsync(stream).GetAwaiter().GetResult())
            {
                var fields = SelectFields(reader.Schema, file);
                if (fields == null)
                {
                    return;
                }

                var indexes = fields.Select(f => reader.Schema.Fields.ToList().IndexOf(f)).ToArray();

                for (var i = 0; i < reader.RowGroupCount; i++)
                {
                    var table = reader.ReadAsTableAsync(null, i).GetAwaiter().GetResult();
                    foreach (var row in table)
                    {
                        var cells = indexes.Select(ix => row[ix]).ToArray();
                        WriteObject(ParquetRowMapper.ToPSObject(new Row(cells), fields));
                    }
                }
            }
        }

        private Field[] SelectFields(ParquetSchema schema, string file)
        {
            if (Column == null || Column.Length == 0)
            {
                return schema.Fields.ToArray();
            }

            var selected = new List<Field>();
            var ok = true;
            foreach (var name in Column)
            {
                var field = schema.Fields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
                if (field == null)
                {
                    ok = false;
                    WriteError(new ErrorRecord(
                        new ArgumentException($"Column '{name}' not found in '{file}'."),
                        "ParquetColumnNotFound", ErrorCategory.InvalidArgument, name));
                }
                else if (!selected.Contains(field))
                {
                    selected.Add(field);
                }
            }
            return ok ? selected.ToArray() : null;
        }
    }
}
