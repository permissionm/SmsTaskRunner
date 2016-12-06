using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HomeAtCare.TaskRunner
{
    public class BulkSmsClient : ISmsClient
    {
        readonly string clientmessage = ConfigurationManager.AppSettings["smsmessage"].ToString();
        public string BuildSms(string username, string password, SendSms.BookingsReminder data)
        {
            var newmessage = clientmessage.Replace("{{name}}", data.Name).Replace("{{surname}}", data.Surname).Replace("{{nurse}}", data.NurseName).Replace("{{date}}", data.BookingDate).Replace("{{time}}", data.BookingTime);

            return SevenBitMessage(username, password, data.Cell, newmessage);
        }

        public Tuple<bool, string> CheckMsIsdn(string number)
        {
            var firstletter = number.Substring(0,1);
            var newnumber = "";

            if (firstletter == "0")
            {
                newnumber = "27" + number.Remove(0, 1) + number;
            }
            else
            {
                newnumber = number;
            }

            Console.WriteLine(newnumber);

            if (newnumber.Length != 11)
            {
                return new Tuple<bool, string>(false, "");
            }

            return new Tuple<bool, string>(true, newnumber);
        }

        public bool SendSms(string smsmessage, string url)
        {
            return SendSmsToProvider(smsmessage, url);
        }

        private bool SendSmsToProvider(string data, string url)
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

        private bool ProccessSms(string messagepacket, string url)
        {
            var result = SendSms(messagepacket, url);
            return result;
        }
        private string Post(string url, string data)
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
            data += "username=" + HttpUtility.UrlEncode(username, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&password=" + HttpUtility.UrlEncode(password, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&message=" + HttpUtility.UrlEncode(message, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&msisdn=" + msisdn;
            data += "&want_report=1";
            return data;

        }
    }

}



