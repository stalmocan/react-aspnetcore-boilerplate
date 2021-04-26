using System;
using System.Collections.Generic;

namespace Library.Security
{
    public class SecurityDetails
    {
        public int UserId { get; set; }
        public DateTime ExpiresOn { get; set; }
        public List<Permissions> Permissions { get; set; }
    }
}
