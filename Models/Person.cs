using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba1_vote.Models
{
    public class Person
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public bool Voted { get; set; } = false;

        public string Id { get; set; }

        public Role Role { get; set; } = Role.voter;

        public bool Permission { get; set; } = true;

        public Person(string id,string name, string surname, Role role)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Role = role;

            if (Role == Role.applicant || Voted)
                Permission = false;
        }
    }
}
