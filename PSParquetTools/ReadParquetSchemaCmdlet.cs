using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Parquet;
using Parquet.Schema;

namespace PSParquetTools
{
    [Cmdlet(VerbsCommunications.Read, "ParquetSchema")]
    [OutputType(typeof(PSObject))]
    public class ReadParquetSchemaCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("FullName")]
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            var resolved = SessionState.Path.GetUnresolvedProviderPathFromPSPath(Path);
            using (var stream = File.OpenRead(resolved))
            using (var reader = ParquetReader.CreateAsync(stream).GetAwaiter().GetResult())
            {
                foreach (var field in reader.Schema.Fields)
                {
                    WriteObject(Describe(field));
                }
            }
        }

        private static PSObject Describe(Field field)
        {
            var result = new PSObject();
            result.Properties.Add(new PSNoteProperty("Name", field.Name));
            result.Properties.Add(new PSNoteProperty("Type", TypeName(field)));
            result.Properties.Add(new PSNoteProperty("IsNullable", field is DataField df && df.IsNullable));
            result.Properties.Add(new PSNoteProperty("Children", ChildrenOf(field).Select(Describe).ToArray()));
            return result;
        }

        private static string TypeName(Field field)
        {
            switch (field)
            {
                case DataField df: return df.ClrType.Name;
                case StructField _: return "Struct";
                case ListField _: return "List";
                case MapField _: return "Map";
                default: return field.SchemaType.ToString();
            }
        }

        private static IEnumerable<Field> ChildrenOf(Field field)
        {
            switch (field)
            {
                case StructField sf: return sf.Fields;
                case ListField lf: return new[] { lf.Item };
                case MapField mf: return new[] { mf.Key, mf.Value };
                default: return Enumerable.Empty<Field>();
            }
        }
    }
}
