using System.Collections.Generic;
using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses
{
    [ConfigSection("TestConfig")]
    public class DummyConfig : IConfiguration
    {
        public DummyConfig()
        {
            Clients = new List<Client>();
        }

        public int SomeInt { get; set; }

        public string Value { get; set; }

        public double DoubleValue { get; set; }

        [KeyVaultSecret]
        public string ConnectionString { get; set; }

        [KeyVaultSecret(usePropertyValueAsSecretName: true)]
        public string Secret { get; set; }

        [KeyVaultSecret("AuthenticationConnectionString")]
        public string AnotherString { get; set; }

        public ICollection<Client> Clients { get; set; }

        [DictionaryPrefix("storageConn")]
        public IDictionary<string, string> ConnStrings { get; set; } = new Dictionary<string, string>();

        public StorageStrings StorageStrings { get; set; }
    }

    public class StorageStrings
    {
        [DictionaryPrefix("storageConn")]
        public IDictionary<string, string> ConnStrings { get; set; } = new Dictionary<string, string>();
    }
}