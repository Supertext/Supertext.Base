
using System.Runtime.CompilerServices;
using Autofac;
using Module = Autofac.Module;

[assembly:InternalsVisibleTo("Supertext.Base.Dal.Specs")]
namespace Supertext.Base.Dal
{
    public class DalModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
        }
    }
}