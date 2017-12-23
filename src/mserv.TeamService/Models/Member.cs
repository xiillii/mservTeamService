using System;

namespace mserv.TeamService.Models
{
    public class Member
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Member()
        {
            
        }

        public Member(Guid id) : this()
        {
            ID = id;
        }

        public Member(string firstName, string lastName, Guid id) : this(id)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString()
        {
            return LastName;
        }
    }
}