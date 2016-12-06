using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HIS.Utilities.SQL.Dapper;

namespace HomeAtCare.TaskRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().StartApplication("smsReminder");
        }

        public void StartApplication(string app)
        {
            switch (app)
            {
                case  "sendsms":
                    new SendSms().Execute();
                    break;

                case  "smsReminder": 
                    new SmsReminder().Process();
                    break;
            }
        }
    }
}
