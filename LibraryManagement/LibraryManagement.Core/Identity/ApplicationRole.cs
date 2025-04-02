using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Core.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }


    }
}
