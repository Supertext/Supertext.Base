using System.Collections.Generic;
using System.Linq;
using Supertext.Base.Common;
using Supertext.Base.Configuration;

namespace Supertext.Base.Authentication
{
    [ConfigSection("Identity")]
    public class Identity : IConfiguration
    {
        public Identity()
        {
            ApiResourceDefinitions = new List<ApiResourceDefinition>();
        }

        public string Authority { get; set; }

        public ICollection<ApiResourceDefinition> ApiResourceDefinitions { get; set;}

        public Option<ApiResourceDefinition> GetApiResourceDefinition(string clientId)
        {
            return ApiResourceDefinitions.Where(definition => definition.ClientId == clientId)
                                         .Select(Option<ApiResourceDefinition>.Some)
                                         .DefaultIfEmpty(Option<ApiResourceDefinition>.None())
                                         .SingleOrDefault();
        }
    }
}