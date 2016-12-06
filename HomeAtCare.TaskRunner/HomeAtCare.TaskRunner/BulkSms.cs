using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections;

namespace HomeAtCare.TaskRunner
{
    public class BulkSMS
    {
        private readonly string m_username;
        private readonly string m_password;
        private readonly string m_url;
        private readonly string m_msisdn;
        private readonly string m_message;
        public BulkSMS(string smsNumber, string smsMessage)
        {
            m_username = ConfigurationManager.AppSettings["smsuname"].ToString();
            m_password = ConfigurationManager.AppSettings["smspass"].ToString();
            m_url = ConfigurationManager.AppSettings["url"].ToString();
            m_msisdn = smsNumber;
            m_message = smsMessage;

        }


        public string BuildMessagePacket()
        {
            return SevenBitMessage(m_username, m_password, m_msisdn, m_message);
        }

        public bool ProccessSms(string messagepacket)
        {
            var result = SendSms(messagepacket, m_url);
            return result;
        }

        private bool SendSms(string data, string url)
        {
            string smsResult = Post(url, data);
            string[] parts = smsResult.Split('|');
            
            string statusCode = parts[0];

            if (statusCode.Equals("0"))
            {
                Console.WriteLine("Sucessfully sent");
                return true;
            }

            return false;
        }


        public string Post(string url, string data)
        {

            string result = null;
            try
            {
                var buffer = Encoding.Default.GetBytes(data);

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);
                WebReq.Method = "POST";
                WebReq.ContentType = "application/x-www-form-urlencoded";
                WebReq.ContentLength = buffer.Length;
                Stream PostData = WebReq.GetRequestStream();

                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                Console.WriteLine(WebResp.StatusCode);

                Stream Response = WebResp.GetResponseStream();
                StreamReader _Response = new StreamReader(Response);
                result = _Response.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result.Trim() + "\n";
        }

        private string SevenBitMessage(string username, string password, string msisdn, string message)
        {
            string data = "";
            data += "username=" + HttpUtility.UrlEncode(m_username, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&password=" + HttpUtility.UrlEncode(m_password, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&message=" + HttpUtility.UrlEncode(m_message, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&msisdn=" + m_msisdn;
            data += "&want_report=1";

            return data;
        }
    }
}
