using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Security.Policy;
using System.Threading;
using System.Web.UI.WebControls;
using Dapper;
using HIS.Utilities.SQL.Dapper;


namespace HomeAtCare.TaskRunner
{
    public class SendSms
    {
        readonly string _db = ConfigurationManager.ConnectionStrings["HIS.Occufit.HomeBasedCare"].ConnectionString;
        readonly ISqlConnector _connector = new SqlConnector();
        readonly string username = ConfigurationManager.AppSettings["smsuname"].ToString();
        readonly string password = ConfigurationManager.AppSettings["smspass"].ToString();
        readonly string url = ConfigurationManager.AppSettings["url"].ToString();
        public SendSms()
        {
        }

        public void Execute()
        {
            while (true)
            {
             
                var data = GetData().ToList();

                if (data.Any())
                {
                    var smsclient = SmsClientObjectFactory.GetSmsClient("bulksms");

                    foreach (var item in data)
                    {

                        var msisdncheck = smsclient.CheckMsIsdn(item.Cell);

                        if (!msisdncheck.Item1)
                        {
                            SetBookingReminderStatus(item.Id, BookingStatus.IncorrectFormat);
                            continue;
                        }

                        var messagetosend = smsclient.BuildSms(username , password, item);
                        var completed = smsclient.SendSms(messagetosend, url);
                        var test = smsclient.CheckMsIsdn(item.Cell);

                        if (completed)
                        {
                            SetBookingReminderStatus(item.Id, BookingStatus.Completed);
                            Console.WriteLine("Sent SuccessFully", true);


                        }
                        else
                        {
                            SetBookingReminderStatus(item.Id, BookingStatus.Unsuccsesful);
                            Console.WriteLine("Not Sent", false);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10000);
                }
            }
        }

        public class BookingsReminder
        {
            public string Name;
            public string Surname;
            public string Cell;
            public int Id;
            public string NurseName;
            public string BookingDate;
            public string BookingTime;

        }

        private IEnumerable<BookingsReminder> GetData()
        {

            var query = new QueryDb<BookingsReminder>(_db, _connector);
            return query.GetDataWithStoredProc("SelectSmsReminderToSend", null);

        }

        public void SetBookingReminderStatus(long id, BookingStatus status)
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["HIS.Occufit.HomeBasedCare"].ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("[UpdateReminder]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();

                    var parameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@Id", id),
                        new SqlParameter("@Status", (int) status),
                    };

                    command.Parameters.AddRange(parameters.ToArray());
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
