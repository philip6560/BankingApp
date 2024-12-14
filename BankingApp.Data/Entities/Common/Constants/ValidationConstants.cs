﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Data.Entities.Common.Constants
{
    public class ValidationConstants
    {
        public const int MinNameLength = 1;
        public const int MaxNameLength = 120;

        public const int MinAddressLength = 0;
        public const int MaxAddressLength = 256;

        public const int MinEmailLength = 1;
        public const int MaxEmailLength = 120;

        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 30;

        public const int AccountNumberLength = 10;
        
        public const int ReferenceNumberLength = 17;
    }
}