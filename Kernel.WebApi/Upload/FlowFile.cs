using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Upload
{
    public class FlowFile
    {
        public string originalFilename { get; set; }
        public string Identifier { get; set; }
        public string flowFilename { get; set; }
        public string path { get; set; }
    }
}