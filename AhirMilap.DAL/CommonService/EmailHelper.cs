using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using AhirMilap.DTO;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace AhirMilap.DAL.CommonService
{
    public class EmailHelper
    {
        private static string _host;
        private static int _port; 
        private static string _from;
        private string _userName;
        private string _password;

        private readonly string _accountSid; // for the SMS
        private readonly string _authToken;
        private readonly string _Fromphone; 

        public EmailHelper(IConfiguration configuration)
        {
            _host = configuration.GetSection("SMTP").GetSection("host").Value;
            _port = Convert.ToInt32(configuration.GetSection("SMTP").GetSection("port").Value);
            _from = configuration.GetSection("SMTP").GetSection("from").Value;
            _userName = configuration.GetSection("SMTP").GetSection("username").Value;
            _password = configuration.GetSection("SMTP").GetSection("password").Value;

            _accountSid = configuration.GetSection("SMSSettings:AccountSid").Value;
            _authToken = configuration.GetSection("SMSSettings:AuthToken").Value;
            _Fromphone = configuration.GetSection("SMSSettings:Fromphone").Value;
        }


        public bool SendEmail(MaileModelHelper emailModel)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_host))
                {
                    ServicePointManager.SecurityProtocol =
                                                        SecurityProtocolType.Tls11
                                                        | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(_from);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.To.Add(emailModel.To);
                    if (!string.IsNullOrEmpty(emailModel.CC))
                    {
                        mailMessage.CC.Add(emailModel.CC);
                    }
                    mailMessage.Body = emailModel.Message;
                    mailMessage.Subject = emailModel.Subject;
                    mailMessage.IsBodyHtml = emailModel.IsBodyHtml;
                    client.Port = _port;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_userName, _password);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


        public bool SendSMS(string phoneNo, string MsgBody)
        {
            try
            {
                phoneNo = phoneNo.Contains("+1") ? phoneNo : "+1" + phoneNo; // +1 is USA country code
                TwilioClient.Init(_accountSid, _authToken);

                var message = MessageResource.Create(
                    body: MsgBody,
                    from: new Twilio.Types.PhoneNumber(_Fromphone),
                    to: new Twilio.Types.PhoneNumber(phoneNo)  //use fix TO number for testing (phoneNo="+919712521577 - old in use")
                );

                string sid = message.Sid;
                if (sid != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
