using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Entities
{
    public class MedicalResultUserPermission : EntityBase<MedicalResultUserPermission>
    {
        public int MedicalResultId { get; set; }
        public int UserInfoId { get; set; }
        public MedicalResult MedicalResult { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}