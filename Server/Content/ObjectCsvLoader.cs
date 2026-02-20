using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public sealed class CSVObjectData
    {
        public int id;
        public string name;
    }

    public class ObjectCsvLoader
    {
        public static List<CSVObjectData> LoadIdNameTable(string filePath)
        {
            List<CSVObjectData> tableById = new List<CSVObjectData>();

            string[] lines = File.ReadAllLines(filePath);

            // 0번째 라인은 헤더(ID,Name)라고 가정
            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex].Trim();
                if (line.Length == 0)
                    continue;

                string[] tokens = line.Split(',');
                if (tokens.Length < 2)
                    continue;

                string idToken = tokens[0].Trim();
                string nameToken = tokens[1].Trim();

                int id = int.Parse(idToken);

                CSVObjectData row = new CSVObjectData();
                row.id = id;
                row.name = nameToken;

                tableById.Add(row);
            }

            return tableById;
        }
    }
}
