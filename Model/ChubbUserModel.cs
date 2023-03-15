using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Model
{
    public class ChubbUserModel : UserModelBase
    {
        public string ChubbUserNumber { get; set; }
        public string ExpiryDate { get; set; } //mm/dd/yyyy
        public string AuthorityGroupName { get; set; }
        public string? AuthorityPlusName { get; set; }
        public string? AuthorityPlusExpiry { get; set; }
        public string Pin { get; set; }
        public string? AuthorityGroupNumber { get; set; }


        //Chubb users must have certain properties in order to be added into chubb director.
        public ChubbUserModel(string firstNme, string lastName, string westernID, string chubbUserNumber,  
            string expiryDate, string authorityName, string pin="1234",string? authorityPlusName=null, string? authorityPlusExpiry = null): base(firstNme, lastName, westernID)
        {
            ChubbUserNumber = chubbUserNumber;

            ExpiryDate = expiryDate;

            AuthorityGroupName = authorityName;

            AuthorityPlusName = authorityPlusName;

            AuthorityPlusExpiry = authorityPlusExpiry;

            Pin = pin;

            //AuthorityGroupNumber = authorityGroupNumber;

        }

        public bool IsExpired()
        {
            
            //year comes as 8/30/23 when passed as string..split on /
            string[] dateArray = this.ExpiryDate.Split("/");
            int year;
            int month;
            int day;
            if (dateArray.Length > 1) //Otherwise we can assume expired
            {
                year = Int32.Parse(dateArray[2]);
                month = Int32.Parse(dateArray[0]);
                day = Int32.Parse(dateArray[1]);
                DateTime expiryDateTime = new DateTime(year, month, day);
                DateTime dateTime = DateTime.Now;
                if (dateTime > expiryDateTime)
                {
                    return true;
                }
            }
            return false;
        }

        public DateTime ExpiryDateTime()
        {
            //split
            string[] DateComponents = this.ExpiryDate.Split("/");
            return new DateTime(Int32.Parse(DateComponents[2]), Int32.Parse(DateComponents[0]), Int32.Parse(DateComponents[1]));
        }
    }
}
