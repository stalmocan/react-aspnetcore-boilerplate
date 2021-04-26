using System.Collections.Generic;

namespace Library.Security
{
    public class CreateAuthorizationTokenDto
    {
        public int UserId { get; set; }
        public List<Permissions> Permissions { get; set; }
    }
}
