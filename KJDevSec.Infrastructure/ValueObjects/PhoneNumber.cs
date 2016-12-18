using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KJDevSec
{
    public class PhoneNumber
    {
        public static PhoneNumber Empty = new PhoneNumber();

        protected PhoneNumber() { }

        public PhoneNumber(string rawValue, string region = "VE")
        {
            /*
            var phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            var _pn = phoneUtil.Parse(rawValue, region);

            this.Value = rawValue;
            this.NationalNumber = _pn.NationalNumber.ToString();
            this.CountryCode = _pn.CountryCode.ToString();
            this.Extension = _pn.Extension;
            this.FormattedValue = phoneUtil.Format(_pn, PhoneNumbers.PhoneNumberFormat.NATIONAL);
            */
        }

        public PhoneNumber(string rawValue, string nationalNumber, string extension, string countryCode, string region = "VE")
        {
            /*
            var phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            var _pn = phoneUtil.Parse(rawValue, region);

            this.Value = rawValue;
            this.NationalNumber = nationalNumber;
            this.Extension = extension;
            this.CountryCode = countryCode;
            this.FormattedValue = phoneUtil.Format(_pn, PhoneNumbers.PhoneNumberFormat.NATIONAL);
            */
        }

        public string Value { get; protected set; }
        public string NationalNumber { get; protected set; }
        public string CountryCode { get; protected set; }
        public string Extension { get; protected set; }
        public string FormattedValue { get; protected set; }       

        public static bool Valid(string phoneNumber, string region = "CA")
        {
            /*
            var phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            try
            {
                PhoneNumbers.PhoneNumber number = phoneUtil.ParseAndKeepRawInput(phoneNumber, region);
                return phoneUtil.IsValidNumber(number);
            }
            catch
            {
                return false;
            }*/

            return false;
        }

        public PhoneNumber Clone()
        {
            return (PhoneNumber)this.MemberwiseClone();
        }
    }
}
