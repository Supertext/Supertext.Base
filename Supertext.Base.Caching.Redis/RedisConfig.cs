using Supertext.Base.Configuration;

namespace Supertext.Base.Caching.Redis;

[ConfigSection("Redis")]
public class RedisConfig : IConfiguration
{
    [KeyVaultSecret("redisConnection")]
    public string ConnectionString { get; set; }

    public int Timeout { get; set; }

    public int ConnectRetry { get; set; }

    [KeyVaultSecret("redisPassword")]
    public string Password { get; set; }

    public bool UseSsl { get; set; }
}