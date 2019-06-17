namespace Supertext.Base.Test.Utils.Migration
{
    public interface IMigrationPerformer
    {
        void Migrate();
        void CleanUp();
    }
}