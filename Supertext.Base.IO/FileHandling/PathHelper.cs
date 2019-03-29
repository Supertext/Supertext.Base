using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Supertext.Base.IO.FileHandling
{
    public class PathHelper : IPathHelper
    {
        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }
    }
}
