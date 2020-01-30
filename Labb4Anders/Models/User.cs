using System;

namespace Labb4Anders
{
    public class User
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NotApprovedPhoto { get; set; }
        public string ApprovedPhoto { get; set; }
        public bool IsAdmin { get; set; }
        public string Password { get; set; }

    }
}