using System;
using System.Collections.Generic;
using System.IO;
using Parquet;
using Parquet.Data;
using Parquet.Rows;
using Parquet.Schema;

var outDir = args.Length > 0 ? args[0] : Path.Combine("PSParquetTools", "IntegrationTests");
Directory.CreateDirectory(outDir);

var flatFields = new Field[]
{
    new DataField<int>("id"),
    new DataField<string>("name"),
    new DataField<double>("score"),
    new DataField<DateTime>("ts"),
};

var flat = new Table(flatFields);
flat.Add(1, "alice", 1.5, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
flat.Add(2, null, 2.5, new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc));
flat.Add(3, "carol", 3.5, new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc));
await flat.WriteAsync(Path.Combine(outDir, "flat.parquet"));

var nestedFields = new Field[]
{
    new DataField<int>("id"),
    new StructField("addr", new DataField<string>("city"), new DataField<string>("zip")),
    new ListField("tags", new DataField<string>("element")),
    new MapField("attrs", new DataField<string>("key", false), new DataField<string>("value")),
};

var nested = new Table(nestedFields);
nested.Add(1, new Row("paris", "75001"), new[] { "a", "b" }, new[] { new Row("k1", "v1") });
nested.Add(2, new Row("rome", "00100"), new[] { "c" }, new[] { new Row("k2", "v2") });
await nested.WriteAsync(Path.Combine(outDir, "nested.parquet"));

var multiSchema = new ParquetSchema(new DataField<int>("id"), new DataField<string>("name"));
using (var stream = File.Create(Path.Combine(outDir, "multi.parquet")))
using (var writer = await ParquetWriter.CreateAsync(multiSchema, stream))
{
    for (var g = 0; g < 3; g++)
    {
        var t = new Table(multiSchema);
        t.Add(g * 2 + 1, "n" + (g * 2 + 1));
        t.Add(g * 2 + 2, "n" + (g * 2 + 2));
        using var rg = writer.CreateRowGroup();
        await rg.WriteAsync(t);
    }
}

Console.WriteLine("Wrote samples to " + outDir);
