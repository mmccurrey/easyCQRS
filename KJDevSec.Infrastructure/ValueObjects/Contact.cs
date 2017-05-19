using System;

namespace EasyCQRS
{
    public class Contact
    {
        protected Contact() { }
        public Contact(string organization, string firstName, string lastName, PhoneNumber phone, EmailAddress emailAddress)
        {
            this.Organization = organization;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PhoneNumber = phone;
            this.EmailAddress = emailAddress;
        }

        public Contact(string organization, string firstName, string lastName, string phoneNumber, string emailAddress)
        {
            this.Organization = organization;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PhoneNumber = new PhoneNumber(phoneNumber);
            this.EmailAddress = new EmailAddress(emailAddress);
        }

        public string Organization { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public PhoneNumber PhoneNumber { get; protected set; }
        public EmailAddress EmailAddress { get; protected set; }

        public Contact Clone()
        {
            Contact clone = (Contact) this.MemberwiseClone();
            clone.PhoneNumber = this.PhoneNumber.Clone();
            clone.EmailAddress = this.EmailAddress.Clone();

            return clone;
        }
    }
}