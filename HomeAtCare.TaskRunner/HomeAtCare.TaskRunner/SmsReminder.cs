using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using Dapper;
using HIS.Utilities.SQL.Dapper;

namespace HomeAtCare.TaskRunner
{
    public class SmsReminder
    {
        public void Process()
        {
            while (true)
            {
                Console.WriteLine("Checking Booking Reminders");
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["HIS.Occufit.HomeBasedCare"].ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        connection.Execute("CreateBookingReminder", commandType: CommandType.StoredProcedure);
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    Thread.Sleep(20000);
                }
            }
        }
    }
}
