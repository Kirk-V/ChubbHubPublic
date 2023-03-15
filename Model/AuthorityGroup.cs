using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChubbHubMVVM.Model
{
    public class AuthorityGroup
    {

        //Dict to hold group name a key, and group members
        //Group members is a list of key values where the ID number is the key and the user info is the value
        public Dictionary<string, ChubbUser> GroupUsers;

        public string Pin { get; set; }
        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string GroupNumber { get; set; }

        public List<string> GroupRooms;

        public String GroupDepartment { get; set; }

        public bool active { get; set; }

        public AuthorityGroup(string groupName, string groupNumber = "-1", string groupDescription = "")
        {
            this.GroupUsers = new Dictionary<string, ChubbUser>();
            this.GroupNumber = groupNumber;
            this.GroupName = groupName;
            this.GroupDescription = groupDescription;
            this.GroupRooms = new List<string>();
            this.GroupDepartment = "default";
            active = false; //only active when members are added
            this.Pin = "4321";
        }

        public bool AddMember(string userNumber, string fName, string lName, string SystemAuthorityName, string systemAuthorityNumber, string cardNum, string invalidOn, string authorityPlusName = "", string AuthorityPlusExpiry = "")
        {
            ChubbUser usr = new(fName, lName, cardNum, userNumber, invalidOn, SystemAuthorityName, systemAuthorityNumber, "1234", authorityPlusName, AuthorityPlusExpiry);
            //ChubbUser usr = new(fName, lName, cardNum, userNumber, invalidOn, SystemAuthorityName, systemAuthorityNumber);
            this.GroupUsers.Add(cardNum, usr);
            if (!active)
            {
                active = true;
            }
            return true;
        }

        public int getUserCount()
        {
            return this.GroupUsers.Count();
        }
        public bool AddMember(ChubbUser usr)
        {
            string cardNum = usr.WesternId;
            if (GroupUsers.ContainsKey(cardNum))
            {
                AlertUserOkBox($"Could not add user with ID: {usr.WesternId} Name: {usr.FirstName} {usr.LastName}", "Adding to Group Error");
                return false;
            }
            GroupUsers.Add(cardNum, usr);
            if (!active)
            {
                active = true;
            }
            return true;
        }

        static void AlertUserOkBox(string message, string caption)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;
            result = MessageBox.Show(message, caption, button, icon, MessageBoxResult.Yes);
            return;
        }


        public bool HasMember(string studentId)
        {
            if (GroupUsers.ContainsKey(studentId))
            {
                return true;
            }
            return false;
        }
    }
}
