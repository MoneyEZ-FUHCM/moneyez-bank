﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Repositories.Commons.Filter
{
    public class AccountFilter : FilterBase
    {
        public Guid? UserId { get; set; }
    }
}
