﻿using Supertext.Base.Configuration;

namespace Supertext.Base.Core.Configuration.Specs.Extensions.ConfigClasses
{
    [ConfigSection("TopHierarchy")]
    public class TopHierarchyConfig : IConfiguration
    {
        public Hierarchy2 Hierarchy2 { get; set; }
    }
}