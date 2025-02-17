﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Contracts.v1.Request
{
    public class UserRegistrationRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
