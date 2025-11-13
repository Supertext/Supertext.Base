using Supertext.Base.Configuration;

namespace Supertext.Base.Net.Http
{
    [ConfigSection("TokenSettings")]
    public  class TokenConfig : IConfiguration
    {
        public bool EnableTokenCaching { get; set; }
    }
}
