using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FillWords.Phone._8._0.DataAccess
{
    public class DataManager
    {
        public List<string> ReadLinesFromTextFile(string path, Encoding encoding)
        {
            var fileUri = new Uri(path, UriKind.Relative);
            using (var stream = Application.GetResourceStream(fileUri).Stream)
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    List<string> lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        lines.Add(reader.ReadLine());
                    }
                    reader.Close();
                    return lines;
                }
            }
        }
    }
}
