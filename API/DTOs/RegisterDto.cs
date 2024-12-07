using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}