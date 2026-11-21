using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Parquet.Rows;
using Parquet.Schema;

namespace PSParquetTools
{
    internal static class ParquetRowMapper
    {
        public static PSObject ToPSObject(Row row, IReadOnlyList<Field> fields)
        {
            var result = new PSObject();
            for (var i = 0; i < fields.Count; i++)
            {
                result.Properties.Add(new PSNoteProperty(fields[i].Name, Convert(fields[i], row[i])));
            }
            return result;
        }

        private static object Convert(Field field, object value)
        {
            if (value == null || value is System.DBNull)
            {
                return null;
            }

            switch (field)
            {
                case StructField sf when value is Row structRow:
                    return ToPSObject(structRow, sf.Fields);

                case MapField mf:
                    return ConvertMap(mf, value);

                case ListField lf when value is IEnumerable listItems && !(value is string):
                    return ConvertList(lf.Item, listItems);
            }

            if (value is IEnumerable seq && !(value is string) && !(value is byte[]))
            {
                return ConvertList(field, seq);
            }

            return value;
        }

        private static object[] ConvertList(Field itemField, IEnumerable items)
        {
            var list = new List<object>();
            foreach (var item in items)
            {
                list.Add(Convert(itemField, item));
            }
            return list.ToArray();
        }

        private static Hashtable ConvertMap(MapField mf, object value)
        {
            var table = new Hashtable();
            if (value is IDictionary dict)
            {
                foreach (DictionaryEntry e in dict)
                {
                    table[Convert(mf.Key, e.Key)] = Convert(mf.Value, e.Value);
                }
            }
            else if (value is IEnumerable entries)
            {
                foreach (var entry in entries)
                {
                    if (entry is Row kv)
                    {
                        table[Convert(mf.Key, kv[0])] = Convert(mf.Value, kv[1]);
                    }
                }
            }
            return table;
        }
    }
}
