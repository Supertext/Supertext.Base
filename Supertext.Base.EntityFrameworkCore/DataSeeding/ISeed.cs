using Microsoft.EntityFrameworkCore;

namespace Supertext.Base.EntityFrameworkCore.DataSeeding
{
    public interface ISeed
    {
        void Seed(ModelBuilder modelBuilder);
    }
}