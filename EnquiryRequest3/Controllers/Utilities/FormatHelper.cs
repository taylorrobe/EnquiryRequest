using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnquiryRequest3.Controllers.Utilities
{
    public class FormatHelper
    {
        public string ReFormatDbDateTime(DateTime dateTime)
        {
            string reformattedDate= "";
            //reformattedDate = dateTime.toISOString().substring(0, 16);
            return reformattedDate;
        }

        public string GetEnquiryCodePrefix(int enquiryId)
        {
            //set Code
            DateTime date = DateTime.UtcNow;
            DateTime endOfFinancialYear = new DateTime(date.Year, 3, 30);
            var prefix = "";
            int year = date.Year;
            if (date < endOfFinancialYear)
            {
                year = year - 1;
            }

            string yearStr = year.ToString();
            prefix = "3" + yearStr.Substring(2);
            string enquiryIdPrefixedZeros = enquiryId.ToString();
            int totalLength = 5;
            if(enquiryIdPrefixedZeros.Length > totalLength)
            {
                totalLength = enquiryIdPrefixedZeros.Length;
            }
            enquiryIdPrefixedZeros = enquiryIdPrefixedZeros.PadLeft(totalLength, '0');
            return prefix + enquiryIdPrefixedZeros;
        }
    }
}