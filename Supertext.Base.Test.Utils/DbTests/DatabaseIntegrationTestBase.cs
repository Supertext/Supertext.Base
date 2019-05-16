using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Modules;
using Supertext.Base.Test.Utils.Migration;
using Module = Autofac.Module;

namespace Supertext.Base.Test.Utils.DbTests
{
    [TestClass]
    public abstract class DatabaseIntegrationTestBase : IDisposable
    {
        private readonly Lazy<IContainer> _lazyTestContainer;
        private readonly ICollection<IModule> _customModules;
        private static Assembly _migrationAssembly;
        private IMigrationPerformer _migrationPerformer;

        protected DatabaseIntegrationTestBase()
        {
            _lazyTestContainer = new Lazy<IContainer>(InitializeTestContainer);
            _customModules = new List<IModule>();
        }

        public IContainer Container => _lazyTestContainer.Value;

        public void Dispose()
        {
            Container.Dispose();
        }

        [AssemblyInitialize]
        protected static void AssemblyInit(TestContext context)
        {
            var databaseType = context.Properties["DatabaseType"].ToString();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var name = referencedPaths.Single(file => file.EndsWith(databaseType + ".dll", StringComparison.Ordinal));

            _migrationAssembly = Assembly.LoadFile(name);
        }

        protected virtual void RegisterModule<TModule>() where TModule : Module, new()
        {
            _customModules.Add(new TModule());
        }

        protected void CleanUp()
        {
            _migrationPerformer.CleanUp();
        }

        private IContainer InitializeTestContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(_migrationAssembly);
            builder.RegisterModule<BaseModule>();

            foreach (var customModule in _customModules)
            {
                builder.RegisterModule(customModule);
            }

            var container = builder.Build();

            _migrationPerformer = container.Resolve<IMigrationPerformer>();
            _migrationPerformer.Migrate();

            return container;
        }
    }
}