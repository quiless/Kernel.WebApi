using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Entities
{
    public class UserInfoTextConfig
    {
        public int Id { get; set; }
        public int UserInfoId { get; set; }
        public string TextConfig { get; set; }
        public int TextType { get; set; }
        public bool WasReset { get; set;  }

        /// <summary>
        /// TextType =  2 (Texto 2)
        /// TextType =  3 (Texto 3)
        /// WasReset = Caso o texto tenha sido resetado
        /// </summary>
        /// <returns></returns>
    }
}