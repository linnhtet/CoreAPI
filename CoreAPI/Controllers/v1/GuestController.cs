using CoreAPI.Contracts.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Controllers.v1
{
    public class GuestController:Controller
    {
        [HttpGet(ApiRoutes.Guest.GuestEndPoint)]
        public IActionResult Guest()
        {
            return Ok(new { Guest = "True" });
        }
    }
}
