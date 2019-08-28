using Autofac;
using Supertext.Base.Conversion.Json;

namespace Supertext.Base.Conversion
{
    public class ConversionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonConverter>().As<IJsonConverter>();
        }
    }
}