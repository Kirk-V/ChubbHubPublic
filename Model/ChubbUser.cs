using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ChubbHubMVVM.Model
{

    //A chubb user is an object with all the components necessary to insert into chubb director;
    public class ChubbUser : IEditableObject, INotifyPropertyChanged
    {
        struct UserInfo
        {
            internal string? ChubbUserNumber;
            internal string? DeptCode;
            internal string? Type; //faculty or staff
            internal string? UserName;
            internal string? FirstName;
            internal string? LastName;
            internal string? WesternId;
            internal string? StatusCode;
            internal string? SystemAuthority;
            internal string? SystemAuthorityNumber;
            internal string? ExpiryString;
            internal DateTime? Expiry;
            internal string? SystemAuthorityPlus;
            internal string? AuthorityPlusNumber;
            internal string? ExpiryPlusString;
            internal DateTime? ExpiryPlus;
            internal string? chubbStatus;
            internal string? Pin;
            internal DateTime? ValidOn;
        }



        private UserInfo OldInfo;
        private UserInfo CurrentInfo;

        public DateTime? ValidOn
        {
            get => this.CurrentInfo.ValidOn;
            set
            {
                this.CurrentInfo.ValidOn = value ?? DateTime.Today;
                //this.CurrentInfo.Expiry = value;
                //Added a datetime, now extract string from datetime object in MM/DD/YY format
                //this.CurrentInfo.ExpiryString = this.AuthorityExpiryAsString();
                NotifyPropertyChanged();               
            }
        }
        public string AuthorityNumber
        {
            get { return this.CurrentInfo.SystemAuthorityNumber ?? string.Empty; }
            set
            {
                this.CurrentInfo.SystemAuthorityNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string AuthorityPlusNumber
        {
            get { return this.CurrentInfo.AuthorityPlusNumber ?? string.Empty; }
            set
            {
                this.CurrentInfo.AuthorityPlusNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string ChubbNumber
        {
            get { return this.CurrentInfo.ChubbUserNumber ?? String.Empty; }
            set
            {
                this.CurrentInfo.ChubbUserNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string ChubbStatus
        {
            get { return this.CurrentInfo.chubbStatus ?? String.Empty; }
            set
            {
                this.CurrentInfo.chubbStatus = value;
                NotifyPropertyChanged();
            }
        }
        public string DeptCode
        {
            get { return this.CurrentInfo.DeptCode ?? String.Empty; }
            set
            {
                this.CurrentInfo.DeptCode = value;
                NotifyPropertyChanged();
            }
        }

        public string Type
        {
            get { return this.CurrentInfo.Type ?? String.Empty; }
            set
            {
                this.CurrentInfo.Type = value;
                NotifyPropertyChanged();
            }
        }

        public string UserName
        {
            get { return this.CurrentInfo.UserName ?? String.Empty; }
            set
            {
                this.CurrentInfo.UserName = value;
                NotifyPropertyChanged();
            }
        }

        public string FirstName
        {
            get { return this.CurrentInfo.FirstName ?? String.Empty; }
            set
            {
                this.CurrentInfo.FirstName = value;
                NotifyPropertyChanged();
            }
        }

        public string LastName
        {
            set
            {
                this.CurrentInfo.LastName = value;
                NotifyPropertyChanged();
            }
            get { return this.CurrentInfo.LastName ?? String.Empty; }
        }

        public string WesternId
        {
            get { return this.CurrentInfo.WesternId ?? string.Empty; }
            set
            {
                this.CurrentInfo.WesternId = value;
                NotifyPropertyChanged();
            }

        }

        public string Pin
        {
            get { return this.CurrentInfo.Pin ?? String.Empty; }
            set
            {
                this.CurrentInfo.Pin = value;
                NotifyPropertyChanged();
            }
        }

        public string StatusCode
        {
            set
            {
                this.CurrentInfo.StatusCode = value;
                NotifyPropertyChanged();
            }
            get { return this.CurrentInfo.StatusCode ?? String.Empty; }
        }

        //[Display(AutoGenerateField = false)]
        public string SystemAuthority
        {
            get { return this.CurrentInfo.SystemAuthority ?? string.Empty; }
            set
            {
                this.CurrentInfo.SystemAuthority = value;
                NotifyPropertyChanged();
            }
        }

        public string SystemAuthorityPlus
        {
            get { return this.CurrentInfo.SystemAuthorityPlus ?? string.Empty; }
            set
            {
                this.CurrentInfo.SystemAuthorityPlus = value;
                NotifyPropertyChanged();
            }
        }

        public string ExpiryString
        {
            get { return this.CurrentInfo.ExpiryString ?? string.Empty; }
            set
            {
                this.CurrentInfo.ExpiryString = value;
                this.CurrentInfo.Expiry = StringToDateTime(value);
                NotifyPropertyChanged();
            }
        }

        /*
         * When we pass an Expiry Date, we also populate the string representation of the date
         * This must be MM/DD/YYYY for chubb import
         */
        public DateTime? Expiry
        {
            get { return this.CurrentInfo.Expiry; }
            set
            {
                this.CurrentInfo.Expiry = value;
                //Added a datetime, now extract string from datetime object in MM/DD/YY format
                this.CurrentInfo.ExpiryString = this.AuthorityExpiryAsString();
                NotifyPropertyChanged();
            }
        }

        public string ExpiryPlusString
        {
            get { return this.CurrentInfo.ExpiryPlusString ?? string.Empty; }
            set
            {
                this.CurrentInfo.ExpiryPlusString = value;
                this.CurrentInfo.ExpiryPlus = StringToDateTime(value);
                NotifyPropertyChanged();
            }
        }
        public DateTime? ExpiryPlus
        {
            get { return this.CurrentInfo.ExpiryPlus; }
            set
            {
                this.CurrentInfo.ExpiryPlus = value;
                this.CurrentInfo.ExpiryPlusString = AuthorityPlusExpiryAsString();
                //this.ExpiryPlusString = AuthorityPlusExpiryAsString();
                NotifyPropertyChanged();
            }
        }
             
        private bool inTxn = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ReadyToImport;

        /*
         * Constructor to make a starter chubb user without the necessary info to insert into a file
         */
        public ChubbUser(string firstName, string lastName, string westernID)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.WesternId = westernID;
            this.ValidOn = null; //setting to null will default validon to today.
            this.ReadyToImport = false;
        }

        /*
         * This constructor is useful for creating new chubb users with most of the information already present. It is now legacy as we do not want to take in strings for expiry dates.
         */
        //public ChubbUser(string firstName, string lastName, string westernID, string chubbUserNumber, string expiry, string authority, string pin, string? authorityPlus=null, string? authPlusExpiry=null)
        //{
        //    this.FirstName = firstName;
        //    this.LastName = lastName;
        //    this.WesternId = westernID;
        //    this.ChubbNumber = chubbUserNumber;
        //    this.ExpiryString = expiry;
        //    this.SystemAuthority = authority;
        //    this.Pin = pin;
        //    if(authorityPlus != null)
        //    {
        //        this.SystemAuthorityPlus = authorityPlus;
        //    }   
        //    if(authPlusExpiry != null)
        //    {
        //        this.ExpiryPlusString = authPlusExpiry;
        //    }

        //    this.ReadyToImport = this.IsReadyToImport();
        //}

        /*
         * Constructor which uses a datetime object for the expiry dates. This should be used when possible as it allows
         * The string version of the expiry to be consistend (MM/DD/YY). This format is required by input files and is preferred.
         */
        public ChubbUser(string firstName, string lastName, string westernID, string chubbUserNumber, DateTime? expiry, string authority, string pin, string? authorityPlus = null, DateTime? authPlusExpiry = null)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.WesternId = westernID;
            this.ChubbNumber = chubbUserNumber;
            this.Expiry = expiry;
            this.SystemAuthority = authority;
            this.Pin = pin;
            this.ValidOn = null; //setting to null will default validon to today.
            if (authorityPlus != null)
            {
                this.SystemAuthorityPlus = authorityPlus;
            }
            if (authPlusExpiry != null)
            {
                this.ExpiryPlus = authPlusExpiry;
            }
            this.ReadyToImport = this.IsReadyToImport();
            
        }
        public ChubbUser(string dCode, string type, string userName, string fName, string lName, string id, string statusCode, string authority = "-1", string chubbNum = "", string authorityNumber = "-1", string pin = "-1")
        {
            this.ChubbNumber = chubbNum;
            this.DeptCode = dCode;
            this.Type = type;
            this.UserName = userName;
            this.FirstName = fName;
            this.LastName = lName;
            this.WesternId = id;
            this.StatusCode = statusCode;
            this.SystemAuthority = authority;
            this.AuthorityNumber = authorityNumber;
            this.Pin = pin;
            this.Expiry = DateTime.MinValue;
            this.ValidOn = null; //setting to null will default validon to today.
        }

        public string? AuthorityExpiryAsString()
        {
            return this.Expiry?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }

        public string? AuthorityPlusExpiryAsString()
        {
            return this.ExpiryPlus?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }

        public void BeginEdit()
        {
            if (!inTxn)
            {
                this.OldInfo = this.CurrentInfo;
                inTxn = true;

            }

        }

        public void CancelEdit()
        {
            if (inTxn)
            {
                this.CurrentInfo = this.OldInfo;
                inTxn = false;
            }
        }

        public void EndEdit()
        {
            if (inTxn)
            {
                this.OldInfo = new UserInfo();
                inTxn = false;
            }
        }

        public DateTime? StringToDateTime(string expiry)
        {
            //only need the mm/dd/yyyy, not time.
            if (string.IsNullOrEmpty(expiry) || expiry.Contains("Never")) { return null; }
            
            if(expiry.Contains("Now"))
            {
                return DateTime.Now;
            }
            //check if user is expired
            
            bool parsedDate = false;
            parsedDate = int.TryParse(expiry.Split(@"/")[0], out int M);
            parsedDate = int.TryParse(expiry.Split(@"/")[1], out int D);
            parsedDate = int.TryParse(expiry.Split(@"/")[2], out int Y);
            //DateTime r = new DateTime(Y, M)
            return parsedDate ? new DateTime(Y, M, D) : null;
            
        }


        public DateTime? ExpiryDateTime()
        {
            //split
            if(this.CurrentInfo.Expiry != null)
            {
                return this.CurrentInfo.Expiry;
            }
            string[] DateComponents = this.ExpiryString.Split("/");
            if(DateComponents.Length == 3)
            {
                return new DateTime(Int32.Parse(DateComponents[2]), Int32.Parse(DateComponents[1]), Int32.Parse(DateComponents[0]));
            }
            return null;
        }

        /*
         * Perform a check on the object to check if it has the necessary info to inser into chubb director
        */
        public bool IsReadyToImport()
        {
            if (this.CurrentInfo.SystemAuthorityNumber == null) return false;
            if (this.CurrentInfo.SystemAuthority == null) return false;
            if (this.CurrentInfo.ChubbUserNumber == null) return false;
            if (this.CurrentInfo.FirstName == null) return false;
            if (this.CurrentInfo.LastName == null) return false;
            if (this.CurrentInfo.Pin == null) return false;
            if (this.CurrentInfo.WesternId == null) return false;
            if (this.CurrentInfo.Expiry == null) return false;
            if (this.CurrentInfo.SystemAuthorityPlus != null)
            {
                if (this.CurrentInfo.ExpiryPlus == null) return false;
                if (this.CurrentInfo.SystemAuthorityNumber == null) return false;
            }
            this.ReadyToImport = true;
            return true;
        }
    }

    
}