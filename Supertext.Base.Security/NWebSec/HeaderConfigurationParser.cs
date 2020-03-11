using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

[assembly: InternalsVisibleTo("Supertext.Base.Security.Specs")]
namespace Supertext.Base.Security.NWebSec
{
    internal class HeaderConfigurationParser : IHeaderConfigurationParser
    {
        private readonly string _configPath;
        private readonly string _xmlNamespace;

        public HeaderConfigurationParser(NWebSecConfig nWebSecConfig)
        {
            _configPath = nWebSecConfig.NWebSecConfigFilePath;
            _xmlNamespace = nWebSecConfig.NWebSecConfigNamespace;
        }

        public IEnumerable<string> Parse(string source)
        {
            var nwebsecConfig = XElement.Load(_configPath);
            var securityHttpHeaders = nwebsecConfig.Element(_xmlNamespace + "securityHttpHeaders");
            var contentSecurityHeaders = securityHttpHeaders.Element(_xmlNamespace + "content-Security-Policy");

            var currentElement = contentSecurityHeaders.Element(_xmlNamespace + source);

            var sourceStrings = currentElement
                                        .Elements(_xmlNamespace + "add")
                                        .Select(item => item.Attribute("source").Value);

            return sourceStrings;
        }
    }
}
