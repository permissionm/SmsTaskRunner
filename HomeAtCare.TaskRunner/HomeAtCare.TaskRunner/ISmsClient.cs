using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAtCare.TaskRunner
{
    public interface ISmsClient
    {
        string BuildSms(string username,string password, SendSms.BookingsReminder data);
        bool SendSms(string smsmessage,string url);
        Tuple<bool, string> CheckMsIsdn(string number);
    }
}
