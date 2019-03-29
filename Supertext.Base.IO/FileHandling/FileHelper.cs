using System.IO;
using System.Linq;

namespace Supertext.Base.IO.FileHandling
{
    internal class FileHelper : IFileHelper
    {
        public int GetNumberOfLines(string path)
        {
            return File.ReadLines(path).Count();
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}
