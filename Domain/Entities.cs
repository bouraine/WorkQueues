using System.Collections.Generic;

namespace Domain
{

    public class EntityBase
    {
        public string Id { get; set; }
    }
    
    public class People: EntityBase
    {
        public string Name { get; set; }
    }

    public class PartialPeople: EntityBase
    {
        public Adress Adress { get; set; }
    }

    public class Adress
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class UserPreference
    {
        public string Id { get; set; }
        public IEnumerable<string> HiddenPeople { get; set; }
        public IEnumerable<string> HiddenCity { get; set; }
    }
}