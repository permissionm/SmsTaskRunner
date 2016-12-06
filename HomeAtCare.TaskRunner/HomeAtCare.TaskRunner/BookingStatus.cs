﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAtCare.TaskRunner
{
    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Completed = 4,
        Closed = 5,
        Unsuccsesful = 6,
        IncorrectFormat = 7
    }
}
