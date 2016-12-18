using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec
{
    public class Address
    {
        public static Address Empty = new Address();

        protected Address() { }
        public Address(string postalCode, string line1, string line2, string city, string province, string country)
        {
            this.PostalCode = postalCode;
            this.Line1 = line1;
            this.Line2 = line2;
            this.City = city;
            this.Province = province;
            this.Country = country;
        }

        public string City { get; protected set; }
        public string Country { get; protected set; }
        public string Line1 { get; protected set; }
        public string Line2 { get; protected set; }
        public string PostalCode { get; protected set; }
        public string Province { get; protected set; }

        public Address Clone()
        {
            return (Address) this.MemberwiseClone();
        }
    }
}
