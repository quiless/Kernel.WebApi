using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Models
{
    public class UploadModel<T>
    {
        public IList<String> UniqueIdentifiers { get; set; }
        public T Data { get; set; }
    }
}