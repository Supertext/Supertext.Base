namespace Supertext.Base.IO.FileHandling
{
    public interface IFileHelper
    {
        int GetNumberOfLines(string originalFilePath);

        void WriteAllText(string path, string content);
    }
}