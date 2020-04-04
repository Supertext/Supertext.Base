namespace Supertext.Base.IO.FileHandling
{
    public interface IFileHelper
    {
        int GetNumberOfLines(string originalFilePath);

        string ReadAllText(string path);

        void WriteAllText(string path, string content);
    }
}