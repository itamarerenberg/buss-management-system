﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    /// <summary>
    /// udentity property = Name
    /// </summary>
    public class User
    {
        public string Name { get; set; }
        public string Password{ get; set; }
        public bool Admin { get; set; }
        public bool IsActive { get; set; }

    }
}
