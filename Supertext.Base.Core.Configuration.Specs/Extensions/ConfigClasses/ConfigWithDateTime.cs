using System;
using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses
{
    [ConfigSection("TestConfig")]
    public class ConfigWithDateTime : IConfiguration
    {
        public DateTime Date { get; set; }
    }
}