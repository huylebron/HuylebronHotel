﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelProject . Domain . Model .Users
{
    public class ChangePasswordViewModel
    {
        public string CurrentPassword  { get; set; }
        public string NewPassword  { get; set; }
    }
}
