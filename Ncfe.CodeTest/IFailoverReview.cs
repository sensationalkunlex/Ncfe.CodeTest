﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncfe.CodeTest
{
    public interface IFailoverReview
    {
        bool DetermineFailover(List<FailoverEntry> failoverEntries);
    }
}
