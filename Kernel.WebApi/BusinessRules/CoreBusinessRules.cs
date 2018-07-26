using Kernel.WebApi.Entities;
using Kernel.WebApi.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using ChinhDo.Transactions;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Kernel.WebApi.Exceptions;
using System.Transactions;
using System.Data;
using OpenHtmlToPdf;
using System.Net.Mail;

namespace Kernel.WebApi.BusinessRules
{
    public class CoreBusinessRules
    {
        #region Private Properties
        private CoreProvider Provider { get; set; }
        #endregion

        #region ctor
        public CoreBusinessRules()
        {
            Provider = new CoreProvider();
        }
        #endregion

        #region Authentication

        public AuthenticationResult Authenticate(string username, string password)
        {

            AuthenticationResult authResult = Provider.Authenticate(username, password);

            return authResult;
        }

        public AuthenticationResult AuthenticateToken(string token)
        {

            AuthenticationResult returnValue = new AuthenticationResult();
            returnValue.Result = ResultType.Failed;

            returnValue.Result = ResultType.Success;

            return returnValue;
        }


        #endregion

        #region User

        public UserInfo GetUserInfoPersonLogged(int PersonId)
        {
            return Provider.GetUserInfoPersonLogged(PersonId);
        }

        public bool SaveUserInfo(UserInfo entity)
        {

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {

                entity.ExecuteValidation();

                Patient Patient = new Patient();

                Patient.Email = entity.Email;
                Patient.Name = entity.Name;
                Patient.PhoneNumber = entity.PhoneNumber;
                Patient.RG = entity.RG;
                Patient.Gender = entity.Gender;
                entity.PatientId = this.SavePatient(Patient);

                this.Provider.SaveUserInfo(entity);

                scope.Complete();
            }

            return true;
        }

        public bool VerifyPatientByEmail(string email)
        {
            return this.Provider.VerifyPatientByEmail(email);
        }


        public bool VerifyPatientByRG(string RG)
        {
            return this.Provider.VerifyPatientByRG(RG);
        }



        #endregion

        #region MedicalResult 
        
        public bool SendMedicalResultSMSEmail(MedicalResult Entity, int PersonIdRequester)
        {
            var medicalResult = this.Provider.GetMedicalResult(Entity.Id);
            var html = GetPdfData(PersonIdRequester, Entity.Id);

            string pdfdir = ConfigurationManager.AppSettings["pdfs"];
            var pdfPath = pdfdir + @"\" + medicalResult.Uid.ToString() + ".pdf";
            if (!File.Exists(pdfPath))
            {
                if (!Directory.Exists(pdfdir))
                    Directory.CreateDirectory(pdfdir);

                var pdf = Pdf.From(html)
                            .OfSize(PaperSize.A4)
                            .WithTitle("AC1 NOW - RESUMO DO EXAME")
                            .Content();


                File.WriteAllBytes(pdfPath, pdf);
            }
            try
            {
                var userInfo = this.GetUserInfoPersonLogged(PersonIdRequester);
                var patient = this.Provider.GetPatientById(Entity.PatientId);

                SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtpHost"], Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]));
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["email"], ConfigurationManager.AppSettings["password"]);
                MailMessage message = new MailMessage();

                MailAddress mailAddress = new MailAddress(ConfigurationManager.AppSettings["email"], "A1C Now +");

                //Endereço do remetente
                message.From = mailAddress;

                MailAddress toMailAddres = new MailAddress(userInfo.Email);
                MailAddress toMailAddres2 = new MailAddress("f.moruzzi@nldiagnostica.com.br");

                // Adiciona os destinos
                message.To.Add(toMailAddres);
                message.Bcc.Add(toMailAddres2);
                //Assunto
                message.Subject = "AC1 Now - Resumo de Exame: " + patient.Name;

                //Corpo do Email
                message.Body = "";

                //Anexos
                Attachment attachment = new Attachment(pdfPath);
                attachment.Name = Path.GetFileName(pdfPath);
                attachment.ContentType = new System.Net.Mime.ContentType(MimeMapping.GetMimeMapping(attachment.Name));

                message.Attachments.Add(attachment);
                //Indica se o corpo é Html
                message.IsBodyHtml = true;
                //Envio de email
                client.Send(message);
            }
            catch (Exception emailex)
            { }

            return true;

        }
              
        public MedicalResult SaveMedicalResult(MedicalResult Entity, int PersonIdRequester)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                Entity.CreateDate = DateTime.Now;
                Entity.ResultDate = DateTime.Now;
                Entity.ExecuteValidation();
                MedicalResultUserPermission MedicalResultUserPermission = new MedicalResultUserPermission();
                IList<MedicalResultUserPermission> ListMedicalResultUserPermission = new List<MedicalResultUserPermission>();
                
                Entity.Id = this.Provider.SaveMedicalResult(Entity);
                MedicalResultUserPermission.MedicalResultId = Entity.Id;
                MedicalResultUserPermission.UserInfoId = PersonIdRequester;
                ListMedicalResultUserPermission.Add(MedicalResultUserPermission);

                this.SetMedicalResultUserPermissions(ListMedicalResultUserPermission);

                var html = GetPdfData(PersonIdRequester, Entity.Id);

                string pdfdir = ConfigurationManager.AppSettings["pdfs"];
                if (!Directory.Exists(pdfdir))
                    Directory.CreateDirectory(pdfdir);

                var pdf = Pdf.From(html)
                            .OfSize(PaperSize.A4)
                            .WithTitle("AC1 NOW - RESUMO DO EXAME")
                            .Content();

                var pdfPath = pdfdir + @"\" + Entity.Uid.ToString() + ".pdf";
                File.WriteAllBytes(pdfPath, pdf);


                if (Entity.SendEmailSMS)
                {
                    try
                    {
                        var userInfo = this.GetUserInfoPersonLogged(PersonIdRequester);
                        var patient = this.Provider.GetPatientById(Entity.PatientId);

                        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtpHost"], Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]));
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["email"], ConfigurationManager.AppSettings["password"]);
                        MailMessage message = new MailMessage();

                        MailAddress mailAddress = new MailAddress(ConfigurationManager.AppSettings["email"], "A1C Now +");

                        //Endereço do remetente
                        message.From = mailAddress;

                        MailAddress toMailAddres = new MailAddress(userInfo.Email);
                        MailAddress toMailAddres2 = new MailAddress("f.moruzzi@nldiagnostica.com.br");

                        // Adiciona os destinos
                        message.To.Add(toMailAddres);
                        
                        message.Bcc.Add(toMailAddres2);

                        //Assunto
                        message.Subject = "AC1 Now - Resumo de Exame: " + patient.Name;

                        //Corpo do Email
                        message.Body = "";

                        //Anexos
                        Attachment attachment = new Attachment(pdfPath);
                        attachment.Name = Path.GetFileName(pdfPath);
                        attachment.ContentType = new System.Net.Mime.ContentType(MimeMapping.GetMimeMapping(attachment.Name));

                        message.Attachments.Add(attachment);
                        //Indica se o corpo é Html
                        message.IsBodyHtml = true;
                        //Envio de email
                        client.Send(message);
                    }
                    catch(Exception emailex)
                    { }
                }

                scope.Complete();
            }

            return Entity;
        }
        

        public void SetMedicalResultUserPermissions(IList<MedicalResultUserPermission> MedicalResultUserPermissions)
        {

            if (MedicalResultUserPermissions.Count() > 0)
            {
                foreach (var medicalResult in MedicalResultUserPermissions)
                {

                    this.Provider.SetMedicalResultUserPermission(medicalResult);
                }
            }

        }

        public IList<MedicalResult> GetMedicalResults(int PersonIdRequester)
        {
            return this.Provider.GetMedicalResults(PersonIdRequester);
        }

        #endregion

        #region Patient

        public int SavePatient(Patient entity)
        {
            var HasPatientSameRG = this.VerifyPatientByRG(entity.RG);
            var HasPatientSameEmail = this.VerifyPatientByEmail(entity.Email);
            entity.ExecuteValidation();

            if (HasPatientSameRG)
            {
                throw new CommonValidationException("O RG informado já possuí cadastro");
            }

            if (HasPatientSameEmail)
            {
                throw new CommonValidationException("O Email informado já possuí cadastro");
            }

            return this.Provider.SavePatient(entity);

        }

        public Patient GetPatientByRG(string RG)
        {
            return this.Provider.GetPatientByRG(RG);
        }

        public bool ImportMedicalResults(int PersonIdRequester, string RG)
        {

            this.Provider.ImportMedicalResults(PersonIdRequester, RG);
            return true;
        }

        #endregion

        #region TextConfig

        public bool SaveTextConfig(IList<UserInfoTextConfig> lstTextConfigs, int PersonIdRequester)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach(var textConfig in lstTextConfigs)
                {
                    textConfig.UserInfoId = PersonIdRequester;
                    if (textConfig.Id > 0)
                    {
                        this.Provider.UpdateTextConfig(textConfig);
                    } else
                    {
                        this.Provider.InsertTextConfig(textConfig);
                    }
                }

                scope.Complete();
            }

            return true;
            
        }

        public bool ResetTextConfig(int PersonIdRequester)
        {
            this.Provider.ResetTextConfig(PersonIdRequester);
            return true;
        }

        public IList<UserInfoTextConfig> GetUserTextConfig(int PersonIdRequester)
        {
            return this.Provider.GetUserTextConfig(PersonIdRequester);
        }

        #endregion

        #region PDF

        public string GetPdfData(int PersonIdRequester, int MedicalResultId)
        {
            var MedicalExaminationResult = this.Provider.GetPdfData(PersonIdRequester, MedicalResultId);

            //[PointerTop]
            var resultadoDevice = (double)MedicalExaminationResult.GlicosePercentual;
            var PointerTop = 0.0;
            if (resultadoDevice <= 4.0)
            {
                PointerTop = -60;
            }
            else if (resultadoDevice > 4.0 && resultadoDevice <= 5.7)
            {
                PointerTop = ((104 * resultadoDevice) / 5.7) * -1;
            }
            else if (resultadoDevice > 5.7 && resultadoDevice <= 6.5)
            {
                PointerTop = ((125 * resultadoDevice) / 6.5) * -1;
            }
            else if (resultadoDevice > 6.5 && resultadoDevice < 8)
            {
                PointerTop = ((160 * resultadoDevice) / 8) * -1;
            }
            else
            {
                PointerTop = -160;
            }

            string html_content = System.IO.File.ReadAllText(ConfigurationManager.AppSettings["HTMLTemplatesPath"] + @"\medical-result.html");

            html_content = html_content.Replace("[Nome]", MedicalExaminationResult.Nome);
            html_content = html_content.Replace("[Email]", MedicalExaminationResult.Email);
            html_content = html_content.Replace("[DataNascimento]", MedicalExaminationResult.DataNascimento);
            html_content = html_content.Replace("[DataExame]", MedicalExaminationResult.DataExame);
            html_content = html_content.Replace("[RG]", MedicalExaminationResult.RG);
            html_content = html_content.Replace("[Celular]", MedicalExaminationResult.Celular);
            html_content = html_content.Replace("[Sexo]", MedicalExaminationResult.Sexo);
            html_content = html_content.Replace("[GlicoseMedia]", MedicalExaminationResult.GlicoseMedia.ToString());
           
            html_content = html_content.Replace("[GlicosePercentual]", Math.Round(MedicalExaminationResult.GlicosePercentual, 1).ToString());
            html_content = html_content.Replace("[Texto1]", MedicalExaminationResult.Texto1);
            html_content = html_content.Replace("[Texto2]", MedicalExaminationResult.Texto2);
            html_content = html_content.Replace("[Texto3]", MedicalExaminationResult.Texto3);
            html_content = html_content.Replace("[PointerTop]", PointerTop.ToString().Replace(",","."));
            return html_content;



        }

        #endregion
    }
}