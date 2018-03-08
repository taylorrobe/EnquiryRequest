using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnquiryRequest.Models
{
    public class AddressMetaData
    {
        // Allow up to 40 uppercase and lowercase 
        // characters. Use standard error.
        [RegularExpression(@"[0-9a-zA-Z''-'-\s,]{1,40}$",
         ErrorMessage = "Some Special Characters are not allowed.")]
        public object Address1;

        // Allow up to 40 uppercase and lowercase 
        // characters. Use standard error.
        [RegularExpression(@"[0-9a-zA-Z''-'-\s,]{1,40}$",
         ErrorMessage = "Some Special Characters are not allowed.")]
        public object Address2;

        // Allow up to 40 uppercase and lowercase 
        // characters. Use standard error.
        [RegularExpression(@"[0-9a-zA-Z''-'-\s,]{1,40}$",
         ErrorMessage = "Some Special Characters are not allowed.")]
        public object Address3;

        // Allow up to 40 uppercase and lowercase 
        // characters. Use standard error.
        [RegularExpression(@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))\s?[0-9][A-Za-z]{2})",
         ErrorMessage = "Invalid postcode.")]
        public object PostCode;
    }

    public class ContactMetaData:AddressMetaData
    {
        // Allow up to 40 uppercase and lowercase 
        // characters. Use custom error.
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Special Characters are not allowed.")]
        public object Forename;

        // Allow up to 40 uppercase and lowercase 
        // characters. Use standard error.
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Special Characters are not allowed.")]
        public object Surname;

    }


}