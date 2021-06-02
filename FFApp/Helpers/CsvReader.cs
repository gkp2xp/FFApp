using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFApp.Helpers
{
    public class CsvReader {
        public static List<string[]> Read(string filename, bool hasHeader) {
            
            using var reader = new StreamReader(filename);
            var csvList = new List<string[]>();

            //Skip first line if header is present
            if (hasHeader) reader.ReadLine();

            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(',');

                csvList.Add(values);
            }

            return csvList;
        }

        public static IEnumerable<string[]> ReadFromBytes(byte[] csvBytes) {
            
            var converted = new List<string[]>();
            var lines = Encoding.UTF8.GetString(csvBytes).Split('\n');

            foreach (var line in lines)  {
                var values = line.Split(',');
                converted.Add(values);
            }

            return converted;
        }
    }
}
