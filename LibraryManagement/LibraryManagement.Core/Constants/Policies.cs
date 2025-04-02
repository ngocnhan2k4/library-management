using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Core.Constants
{
    public static class Policies
    {
        public const string RequireAdmin = "RequireAdmin";
        public const string RequireLibrarian = "RequireLibrarian";
        public const string ManageBooks = "ManageBooks";
        public const string ManageUsers = "ManageUsers";
        public const string BorrowBooks = "BorrowBooks";
    }
}
