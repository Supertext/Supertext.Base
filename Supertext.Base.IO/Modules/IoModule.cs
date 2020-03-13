using Autofac;
using Supertext.Base.IO.FileHandling;
using Supertext.Base.IO.StreamHandling;

namespace Supertext.Base.IO.Modules
{
    public class IoModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileHelper>().As<IFileHelper>();
            builder.RegisterType<StreamFactory>().As<IStreamFactory>();
            builder.RegisterType<StreamReaderWrapper>().As<IStreamReader>();
            builder.RegisterType<StreamWriterWrapper>().As<IStreamWriter>();
            builder.RegisterType<XmlFileHelper>().As<IXmlFileHelper>();
        }
    }
}
