using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace ChubbHubMVVM.Model
{
    public class UserModelBase
    {

        //public string chubbUserNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string WesternID { get; set; }
        //public DateTime ExpiryDate { get; set; } //mm/dd/yyyy

        //public string authority { get; set; }
        //public string authorityPlus { get; set; }

        //public string authorityPlusExpiry { get; set; }

        //public string Pin { get; set; }

        //public string AuthorityGroupNumber { get; set; }


        //Base class for a user in the chubb/ssc world. Eachuser will have a First/last name, Western ID#
        public UserModelBase(string firstName, string lastName, string westernID)
        {
            FirstName = firstName;
            LastName = lastName;
            WesternID = westernID;
        }

        

    }
}
