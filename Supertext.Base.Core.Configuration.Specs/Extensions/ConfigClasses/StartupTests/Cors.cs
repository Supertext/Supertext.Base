using System.Collections.Generic;

namespace Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses.StartupTests;

public class Cors
{
    public Cors()
    {
        AllowedOrigins = new List<string>();
    }

    public ICollection<string> AllowedOrigins { get; set; }
}