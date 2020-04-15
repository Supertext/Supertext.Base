using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Supertext.Base.EntityFrameworkCore.DataSeeding
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Configures data seeding
        /// </summary>
        /// <typeparam name="TSeed">Represents type of an assembly which implements ISeed. All types of that assembly which implement ISeed are being taken.</typeparam>
        /// <param name="modelBuilder"></param>
        public static void Seed<TSeed>(this ModelBuilder modelBuilder) where TSeed : ISeed
        {
            var seedingTypes = typeof(TSeed).Assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(ISeed)));
            foreach (var seedType in seedingTypes)
            {
                var seed = Activator.CreateInstance(seedType) as ISeed;
                seed?.Seed(modelBuilder);
            }
        }
    }
}