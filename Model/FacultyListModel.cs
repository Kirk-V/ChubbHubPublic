using NPOI.HPSF;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Model
{
    public class FacultyListModel : INotifyPropertyChanged
    {

        //All users in file + Any additional updates done through UI


        #region config
        private const string DefaultFacultyFile = @"C:\Users\kirkv\Desktop\ChubbMVVMRepo\ChubbHubMVVM\TestData\NewFacultyStaff.txt";
        private const string LatestFacultyFile = @"S:\Batch\Logs\NewFacultyStaff.txt";
        private const string DefaultDepartmentCodeFile = @"W:\SSTSCardAccess\Authority Groups\DepartmentCodes.xlsx";
        private const string DefaultStudentAuthorityGroupName = "SSC UGrad";
        #endregion

        public Dictionary<string, string> DepartmentLabels;
        public Dictionary<string, AuthorityGroup> DepartmentCodes;
        /*
         * The obsevable collection View serves as the binding point for the datagrid control and
         * it gets updated to hold the 'view' that the user desires. This can be one of {all users, unmatched, matched}
         */
        public ObservableCollection<FacultyListUser> View { get; set; }
        public Dictionary<string, FacultyListUser> Users { get; set; } //String is the trimmed student ID

        public Dictionary<string, FacultyListUser> MatchedUsers { get; set; } //String is the trimmed student ID

        public Dictionary<string, FacultyListUser> UnmatchedUsers { get; set; } //String is the trimmed student ID

        //ObservableCollection<FacultyListUser> UserList { get; set; }

        // A dictionary of Programs where the program name is the key IE. 'SSAC4' and the value
        // is a list of all users in that program
        Dictionary<string, List<ChubbUserModel>> Programs { get; set; }

        public string ListFile { get; set; }

        private UserReportModel ChubbReport;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public FacultyListModel(UserReportModel report, string fileName=DefaultFacultyFile)
        {
            this.ChubbReport = report;
            Users = new Dictionary<string, FacultyListUser>();
            Programs = new Dictionary<string, List<ChubbUserModel>>();
            MatchedUsers = new Dictionary<string, FacultyListUser>();
            UnmatchedUsers = new Dictionary<string, FacultyListUser>();
            View = new ObservableCollection<FacultyListUser>();
            DepartmentCodes = new Dictionary<string, AuthorityGroup>();
            DepartmentLabels = new();
            InferDepartmentCodes();
            if (ParseFile(fileName))
            {
                this.ListFile = fileName;
                View = new ObservableCollection<FacultyListUser>(Users.Values);
            }
            else
            {
                this.ListFile = "None";
            }

        }

        public void NewFacultyFile(string newFile)
        {
            //reset all props
            Users = new Dictionary<string, FacultyListUser>();
            Programs = new Dictionary<string, List<ChubbUserModel>>();
            MatchedUsers = new Dictionary<string, FacultyListUser>();
            UnmatchedUsers = new Dictionary<string, FacultyListUser>();
            View = new ObservableCollection<FacultyListUser>();
            if (ParseFile(newFile))
            {
                this.ListFile = newFile;
                View = new ObservableCollection<FacultyListUser>(Users.Values);
            }
            else
            {
                this.ListFile = "None";
            }
        }

        private bool ParseFile(string FileName)
        {
            //check file integrity
            //file still exists, file is has contents, file format is okay

            FileInfo fileInfo = new FileInfo(FileName);
            try
            {
                if (fileInfo.Exists)
                {
                    string[] lines = File.ReadAllLines(FileName);

                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] userData = line.Split(',');
                            AddUser(userData);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ChubbReport.NotificationList.Messages.Add("Could not add all users from new faculty file.");
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private void AddUser(string[] userData)
        {
            if (userData.Length == 7)
            {
                string dept = userData[0].Trim();
                string type = userData[1].Trim();
                string usrName = userData[2].Trim();
                string firstName = userData[4].Trim();
                string lastName = userData[3].Trim();
                string WesternId = userData[5].Trim().TrimStart('0');
                string status = userData[6].Trim();
                
                if (this.Users.ContainsKey(WesternId))
                {
                    //already added
                    return;
                }
                else
                {
                    FacultyListUser usr = new(dept, type, usrName, firstName, lastName, WesternId, status);
                    //Add Department Label if exists in dept dict
                    if (DepartmentLabels.TryGetValue(dept, out string? Dl))
                    {
                        usr.DepartmentLabel = Dl;
                    }
                    //matched or unmatched?
                    if (this.ChubbReport.hasUser(WesternId))
                    {
                        //matched
                        ChubbUser tempUser = ChubbReport.GetUser(WesternId);
                        if (tempUser != null)
                        {
                            usr.AuthorityNumber = tempUser.AuthorityNumber;
                            usr.ChubbNumber = tempUser.ChubbNumber;
                            
                            usr.SystemAuthority = tempUser.SystemAuthority;
                            usr.Expiry = tempUser.ExpiryDateTime();
                            usr.Pin = tempUser.Pin;
                            usr.SystemAuthorityPlus = tempUser.SystemAuthorityPlus;
                            
                            AuthorityGroup? UserAuthorityGroup;
                            ChubbReport.AuthorityGroups.TryGetValue(tempUser.SystemAuthority, out UserAuthorityGroup);
                            usr.AuthorityGroup = UserAuthorityGroup;
                        }

                        usr.ChubbStatus = "Matched";
                        this.MatchedUsers.Add(WesternId, usr);

                    }
                    else
                    {
                        usr.ChubbStatus = "Unmatched";
                        //Lookup the dept code if one exists for Faculty
                        if(usr.Type == "F")
                        {
                            if (DepartmentCodes.TryGetValue(dept, out AuthorityGroup? Ag))
                            {
                                usr.AuthorityGroup = Ag;
                            }
                        }
                        else if(usr.Type == "S")
                        {

                            if(ChubbReport.AuthorityGroups.TryGetValue(DefaultStudentAuthorityGroupName, out AuthorityGroup? Ag))
                            {
                                usr.AuthorityGroup = Ag;
                            }
                        }
                        
                        this.UnmatchedUsers.Add(WesternId, usr);

                    }
                    this.Users.Add(WesternId, usr);
                }
            }
            else
            {
                return;
            }

        }

        public void UpdateAuthorityGroupForView(string newAuthority)
        {
            if (this.ChubbReport.AuthorityGroups.ContainsKey(newAuthority))
            {
                string AuthorityNumber = this.ChubbReport.AuthorityGroups[newAuthority].GroupNumber;
                foreach (FacultyListUser usr in this.View)
                {
                    usr.SystemAuthority = newAuthority;
                    usr.AuthorityNumber = AuthorityNumber;
                }
            }
            else
            {
                throw new Exception("AuthorityGroup not found, cannot be added to user");
            }

        }

        public void InferDepartmentCodes()
        {
            //Open file
            if(!File.Exists(DefaultDepartmentCodeFile))
            {
                this.ChubbReport.NotificationList.Messages.Add($"DepartmentCodeAuthorities file not found. Cannot infer authority groups for new faculty/staff.");
            }
            FileInfo CodeFileInfo = new FileInfo(DefaultDepartmentCodeFile);
            try
            {
                using (ExcelPackage package = new ExcelPackage(CodeFileInfo))
                {
                    //get the worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    int colCount = worksheet.Dimension.End.Column;  //get Column Count
                    int rowCount = worksheet.Dimension.End.Row;     //get row count
                    for (int row = 2; row <= rowCount; row++) // start on second row, extract the dpt code and authority group number
                    {
                        for (int col = 1; col <= colCount; col++)
                        {
                            string? DepartmentCode = worksheet.Cells[row, 1].Value.ToString();
                            string? DepartmentLabel = worksheet.Cells[row, 2].Value.ToString();
                            string? AuthorityNumber = worksheet.Cells[row, 4].Value.ToString()?.Trim();
                            if (AuthorityNumber != null && DepartmentCode != null)
                            {
                                //find the authority group
                                foreach (AuthorityGroup Ag in ChubbReport.AuthorityGroups.Values.ToList())
                                {
                                    if (Ag.GroupNumber == AuthorityNumber)
                                    {
                                        //match found
                                        DepartmentCodes.TryAdd(DepartmentCode, Ag);
                                    }
                                }
                            }
                            if(DepartmentCode!=null && DepartmentLabel!=null)
                            {
                                DepartmentLabels.TryAdd(DepartmentCode, DepartmentLabel);

                            }
                            //DepartmentCodes.Add(worksheet.Cells[row, 1].Value.ToString(), this.ChubbReport.AuthorityGroups[]);
                        }

                    }
                    //made it through the file
                }
            }
            catch(Exception e)
            {
                ChubbReport.NotificationList.Messages.Add($"Could not infer all Department Codes. Check file and restart program");
            }   
        }
        /*
         * Use config setting at top of file to search for the latest faculty file
         */
        public void InferFacultyFile()
        {
            if(File.Exists(DefaultFacultyFile))
            {
                //we have a faculty file
                if(new FileInfo(DefaultFacultyFile).Length > 0)
                {

                }

            }
        }
    }

    public class FaculyListCollection : ObservableCollection<FacultyListUser>
    {
        List<FacultyListUser> users = new List<FacultyListUser>();
    }

    public class FacultyListUser : IEditableObject, INotifyPropertyChanged
    {

        struct UserInfo
        {
            internal string ChubbUserNumber;
            internal string DeptCode;
            internal string Type; //faculty or staff
            internal string UserName;
            internal string FirstName;
            internal string LastName;
            internal string WesternId;
            internal string StatusCode;
            internal string SystemAuthority;
            internal string SystemAuthorityNumber;
            internal DateTime? Expiry;
            internal string chubbStatus;
            internal string Pin;
            internal string AuthorityPlus;
            internal string AuthorityPlusNumber;
            internal DateTime? ExpiryPlus;
            internal string DepartmentLabel;
        }

        private UserInfo OldInfo;
        private UserInfo CurrentInfo;

        public string AuthorityNumber
        {
            get { return this.CurrentInfo.SystemAuthorityNumber; }
            set
            {
                this.CurrentInfo.SystemAuthorityNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string AuthorityPlusNumber
        {
            get { return this.CurrentInfo.AuthorityPlusNumber; }
            set
            {
                this.CurrentInfo.AuthorityPlusNumber = value;
                NotifyPropertyChanged();
            }
        }

        //This is the department label from the DepartmenCodeAuthorities file NOT the department of
        //the users authority group. 
        public string DepartmentLabel
        {
            get { return this.CurrentInfo.DepartmentLabel; }
            set
            {
                this.CurrentInfo.DepartmentLabel = value;
                NotifyPropertyChanged();
            }
        }
        public string ChubbNumber
        {
            get { return this.CurrentInfo.ChubbUserNumber; }
            set
            {
                this.CurrentInfo.ChubbUserNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string ChubbStatus
        {
            get { return this.CurrentInfo.chubbStatus; }
            set
            {
                this.CurrentInfo.chubbStatus = value;
                NotifyPropertyChanged();
            }
        }
        public string DeptCode
        {
            get { return this.CurrentInfo.DeptCode; }
            set
            {
                this.CurrentInfo.DeptCode = value;
                NotifyPropertyChanged();
            }
        }

        public string Type
        {
            get { return this.CurrentInfo.Type; }
            set
            {
                this.CurrentInfo.Type = value;
                NotifyPropertyChanged();
            }
        }

        public string UserName
        {
            get { return this.CurrentInfo.UserName; }
            set
            {
                this.CurrentInfo.UserName = value;
                NotifyPropertyChanged();
            }
        }

        public string FirstName
        {
            get { return this.CurrentInfo.FirstName; }
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
            get { return this.CurrentInfo.LastName; }
        }

        public string WesternId
        {
            get { return this.CurrentInfo.WesternId; }
            set
            {
                this.CurrentInfo.WesternId = value;
                NotifyPropertyChanged();
            }

        }

        public string Pin
        {
            get { return this.CurrentInfo.Pin; }
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
            get { return this.CurrentInfo.StatusCode; }
        }

        [Display(AutoGenerateField = false)]
        public String SystemAuthority
        {
            get { return this.CurrentInfo.SystemAuthority; }
            set
            {
                this.CurrentInfo.SystemAuthority = value;
                NotifyPropertyChanged();
            }
        }

        public string SystemAuthorityPlus
        {
            get { return this.CurrentInfo.AuthorityPlus; }
            set
            {
                this.CurrentInfo.AuthorityPlus = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime? Expiry
        {
            get { return this.CurrentInfo.Expiry; }
            set
            {
                this.CurrentInfo.Expiry = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime? ExpiryPlus
        {
            get { return this.CurrentInfo.ExpiryPlus; }
            set
            {
                this.CurrentInfo.ExpiryPlus = value;
                NotifyPropertyChanged();
            }
        }
        private bool inTxn = false;

        private AuthorityGroup? _authorityGroupPlus;
        public AuthorityGroup? AuthorityGroupPlus
        {
            get => _authorityGroupPlus;
            set
            {
                _authorityGroupPlus = value;
                if (value != null)
                {
                    AuthorityPlusNumber = value.GroupNumber;
                    SystemAuthorityPlus = value.GroupName;
                }
                NotifyPropertyChanged();
            }
        }

        private AuthorityGroup? _authorityGroup;
        public AuthorityGroup? AuthorityGroup
        {
            get => _authorityGroup;
            set
            {
                _authorityGroup = value;
                if(value != null)
                {
                    AuthorityNumber = value.GroupNumber;
                    SystemAuthority = value.GroupName;
                }
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public FacultyListUser(string dCode, string type, string userName, string fName, string lName, string id, string statusCode, AuthorityGroup? authorityGroup=null, string authority = "-1", string chubbNum = "", string authorityNumber = "-1", string pin = "-1")
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
            this.Expiry = null;
            this.AuthorityGroup = authorityGroup;
            this.DepartmentLabel = string.Empty;
        }

        public string? AuthorityExpiryAsString()
        {
            return this.Expiry?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }


        public string? AuthorityPlusExpiryAsString()
        {
            return this.ExpiryPlus?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
        
        /*
         * Convets a Faculty user to a 
         */
        public ChubbUser GetUserAsChubbUser()
        {
            ChubbUser returnUser = new(FirstName, LastName, WesternId, ChubbNumber, Expiry, SystemAuthority, Pin, SystemAuthorityPlus, ExpiryPlus);
            return returnUser;
        }
        /*
         * Edit state saving
         */
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
    }
    
}
