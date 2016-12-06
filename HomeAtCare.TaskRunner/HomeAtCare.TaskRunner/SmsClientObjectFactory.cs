using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAtCare.TaskRunner
{
    public class SmsClientObjectFactory
    {
        public static ISmsClient GetSmsClient(string smsclient)
        {
            switch (smsclient)
            {
                case "bulksms":
                    return new BulkSmsClient();
            }
            return null;
        }
    }
}

