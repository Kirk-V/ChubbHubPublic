using ChubbHubMVVM.Exceptions;
using ChubbHubMVVM.Stores;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using File = System.IO.File;

namespace ChubbHubMVVM.Model
{
    public class UserReportModel
    {

        #region config
        private string _folderPath = @"W:\SSTSCardAccess\Reports"; //Set to default file location of the Chubb Director Reports
        private string _defaultGroupFile = @"W:\SSTSCardAccess\Authority Groups\List-of-Authority-Groups.xlsx";
        //private string _defaultGroupFile = @"W:\SSTSCardAccess\ChubbHub\AuthorityGroupsForChubbHubb.xlsx"; //Set to the defualt location of the Group Info File
        #endregion

        private FileInfo _groupInfoFileInfo;

        public FileInfo GroupInfoFile
        {
            get
            {
                return _groupInfoFileInfo;
            }
            set
            {
                _groupInfoFileInfo = value;
            }
        }

        private FileInfo _fileInfo;

        public FileInfo FilePathInfo
        { 
            get 
            { 
                return _fileInfo; 
            } 
            set 
            {
                _fileInfo = value;
            } 
        }

        //public FileInfo ReportFileInfo { get; set; }

        private List<string> UserNumbers { get; set; }

        //dictionary of all users in the file
        private Dictionary<string, string> userHash = new Dictionary<string, string>();

        //Dictionary of Authority Groups Key=Authority GroupName
        public Dictionary<string, AuthorityGroup> AuthorityGroups { get; set; }
        //total Count of Users
        public int userCount
        {
            get { return ChubbUsers != null ? this.ChubbUsers.Count : 0; }

        }

        public Dictionary<string, Department> Departments;

        //Department names and a list of authority groups for the department


        public Dictionary<string, List<AuthorityGroup>> DepartmentGroups;

        public int DepartmentCount = 0;

        private List<string> headers = new List<string>(); //list of available headers in file
        public Dictionary<string, ChubbUser> ChubbUsers { get; set; } //Key = student nummber

        public Dictionary<string, ChubbUser> ExpiredUsers;

        public DateTime ExpiryDate { get; set; } //mm/dd/yyyy

        public List<string> getHeaders()
        {
            return this.headers;
        }

        //index and value of current chubb user number iterator
        private Tuple<int, string> ChubbUserNumberCurrentIndex = new(0, "0");

        public Notifications NotificationList { get; set; }

        //When creating a userReport object, we instantiate a new Dictionary to hold all cubb users, this is indexed by the 
        public UserReportModel(Notifications notifications, string? filePath = null)
        {
            // instead of initializing everything in the constructor, use a call to Reinitialize. 
            // This allows us to call reinitialize in the future without having to make a new object. 
            NotificationList = notifications;
            string newFile = filePath ?? this.InferFile();
            GroupInfoFile = new FileInfo(_defaultGroupFile);
            Reinitialize(newFile);
        }


        public void Reinitialize(string newFilePath)
        {
            //clear notifications
            NotificationList.Messages.Clear();
            //Set the report file path
            if (!File.Exists(newFilePath))
            {
                NotificationList.Messages.Add($"could not initialize with All user report: {newFilePath}");
                throw new UserFileException($"{newFilePath} Could not be Found");
            }
            FilePathInfo = new FileInfo(newFilePath);
            
            //Remake Dictionaries
            this.ChubbUsers = new Dictionary<string, ChubbUser>(); //All users in the file
            this.ExpiredUsers = new Dictionary<string, ChubbUser>(); //All expired users in the file
            this.DepartmentGroups = new Dictionary<string, List<AuthorityGroup>>();
            this.AuthorityGroups = new Dictionary<string, AuthorityGroup>(); //All Authority Groups Found in the 
            //Create a list of all possible userNumbers (chubb numbers)
            this.UserNumbers = new();
            for (int i = 1; i < 20000; i++){this.UserNumbers.Add(i.ToString());}

            this.Departments = new();

            try
            {
                if (!this.FilePathInfo.Exists)
                {
                    throw new FileNotFoundException();
                }
                MakeHeaders();
                Validateheaders();

                bool dictBuild = this.BuildUserDict(FilePathInfo.FullName);
                if(dictBuild)
                {
                    InferGroupInfo();
                    System.Diagnostics.Debug.WriteLine("Added users and Infered Group Info");
                }
                else
                {
                    this.ChubbUsers = new Dictionary<string, ChubbUser>(); //All users in the file
                    this.ExpiredUsers = new Dictionary<string, ChubbUser>(); //All expired users in the file
                    this.DepartmentGroups = new Dictionary<string, List<AuthorityGroup>>();
                    this.AuthorityGroups = new Dictionary<string, AuthorityGroup>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return;
            }
        }

        /*
         * Function to update the report with the authority group info generated by the chubbHub groups file. By using this method
         * instead of simply setting the instance variables, we can keep an update of things like department counts, department list
         * etc. 
         */
        public bool AddAuthorityGroupInfo(string groupNumber, string groupName, string groupDepartment, string groupDescription, List<string> groupRooms)
        {
            if (this.AuthorityGroups.ContainsKey(groupName))
            {
                //AuthorityGroup exists
                if (this.AuthorityGroups[groupName].GroupDepartment == "default")
                {
                    //group does not yet have department info, add dept and update the department datastructure
                    if (!this.DepartmentGroups.ContainsKey(groupDepartment))
                    {
                        //add a new department to the list of groupDepartmetns
                        this.DepartmentGroups.Add(groupDepartment, new List<AuthorityGroup>());
                        this.DepartmentCount += 1;
                    }

                    //update group deets
                    this.AuthorityGroups[groupName].GroupDepartment = groupDepartment;
                    this.AuthorityGroups[groupName].GroupDescription = groupDescription;
                    this.AuthorityGroups[groupName].GroupRooms = groupRooms;
                    this.AuthorityGroups[groupName].GroupNumber = groupNumber;


                }
                this.DepartmentGroups[groupDepartment].Add(this.AuthorityGroups[groupName]);
                foreach (ChubbUser usr in this.AuthorityGroups[groupName].GroupUsers.Values)
                {
                    usr.AuthorityNumber = groupNumber;
                }
            }
            return true;
        }



        //Function to strip the first file
        private void MakeHeaders()
        {
            headers.Clear();
            //open file, get first line
            //Open the stream and read it back.
            try
            {
                using (StreamReader sr = new StreamReader(FilePathInfo.FullName))
                {
                    string line = sr.ReadLine() ?? "";
                    this.headers = line.Split(",").ToList();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private void Validateheaders()
        {
            //check count
            if (this.headers == null) { throw new UserFileException("Invalid Number of Headers"); }
            if (!headers.Contains("Card Number")) { throw new UserFileException("Must include card number"); }
            if (!headers.Contains("Last Name")) { throw new UserFileException("Must include Last Name"); }
            if (!headers.Contains("System Authority")) { throw new UserFileException("Must include System Authority"); }
            if (!headers.Contains("Authority Plus")) { throw new UserFileException("Must include Authority Plus"); }
        }

        //    /*
        //     * Passed a department, gets the list of authority group names for that department
        //     */
        //    public List<string> GetDepartmentGroupNames(string department)
        //    {
        //        List<string> rList = new List<string>();
        //        foreach (AuthorityGroup g in this.DepartmentGroups[department])
        //        {
        //            rList.Add(g.GroupName);
        //        }
        //        return rList;
        //    }

        private bool checkHeaders(Dictionary<string, int> headers)
        {
            if (!headers.ContainsKey("First Name"))
            {
                NotificationList.Messages.Add("Invalid headers: First name not included, please check chubb txt file for required headers");
                return false;
            }

            if (!headers.ContainsKey("Last Name"))
            {
                NotificationList.Messages.Add("Invalid headers: Last name not included, please check chubb txt file for required headers");
                return false;
            }
            if (!headers.ContainsKey("System Authority"))
            {
                NotificationList.Messages.Add("Invalid headers: System Authority not included, please check chubb txt file for required headers");
                return false;
            }
            if (!headers.ContainsKey("Card Number"))
            {
                NotificationList.Messages.Add("Invalid headers:Card Number not included, please check chubb txt file for required headers");
                return false;
            }
            if (!headers.ContainsKey("Invalid On"))
            {
                NotificationList.Messages.Add("Invalid headers: Invalid On not included, please check chubb txt file for required headers");
                return false;
            }
            if (!headers.ContainsKey("Authority Plus"))
            {
                NotificationList.Messages.Add("Invalid headers: Authority Plus not included, please check chubb txt file for required headers");
                return false;
            }
            if (!headers.ContainsKey("Authority Plus End"))
            {
                NotificationList.Messages.Add("Invalid headers: Authority Plus End not included, please check chubb txt file for required headers");
                return false;
            }
            return true;
        }

        private bool InferDepartments(string? departmentInfoFile=null)
        {
            return true;
        }


        private bool BuildUserDict(string chubbFilePath)
        {
            try
            {
                using StreamReader chubbReader = new(chubbFilePath);
                {

                    string line;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    Dictionary<string, int> headers = new Dictionary<string, int>();
                    int indexNum = 0;
                    while ((line = chubbReader.ReadLine()) != null)
                    {
                        //a row should [User,First Name,Last Name,System Authority,Card Number,Invalid On,Authority Plus,Authority Plus End]
                        string[] row = line.Split(',');
                        if (row[0].Contains("User"))
                        {
                            //make hash of headers                            
                            foreach (string head in row)
                            {
                                if (head.Length > 1)
                                {
                                    headers.Add(head, indexNum++);
                                }
                            }
                            if (!checkHeaders(headers))
                            {
                                NotificationList.Messages.Add("Invalid headers, please check chubb txt file for required headers");
                                //return false;
                            }
                            continue;
                        }
                        string usrNumber = headers.ContainsKey("User")? row[headers["User"]] : "";
                        string fName = headers.ContainsKey("First Name")?  row[headers["First Name"]]: "";
                        string lName = headers.ContainsKey("Last Name")? row[headers["Last Name"]] : "";
                        string authority = headers.ContainsKey("System Authority") ? row[headers["System Authority"]].Trim(): "";
                        string studentnumber = headers.ContainsKey("Card Number")? row[headers["Card Number"]].Trim(): "";
                        string expiry = headers.ContainsKey("Invalid On") ?  row[headers["Invalid On"]].Trim() : "";
                        string? authPlus = headers.ContainsKey("Authority Plus")? row[headers["Authority Plus"]].Trim() : "";
                        //bool exists = Array.Exists(headers, item => item == "Authority Plus End");
                        
                        string? AuthPlusEnd = headers.ContainsKey("Authority Plus End") ? row[headers["Authority Plus End"]].Trim() : "";
                        if(authPlus == "")
                        {
                            authPlus = null;
                        }
                        if(AuthPlusEnd =="")
                        {
                            AuthPlusEnd = null;
                        }
                        
                        //take usernumber out of available Numbers;
                        this.UserNumbers.Remove(usrNumber);

                        DateTime? expiryDateTime = null;
                        bool expired = false;
                        //Extract expiry info
                        if (expiry != "")
                        {
                            expiry = expiry.Split(" ")[0]; //only need the mm/dd/yyyy, not time.
                            if (expiry != "Never")
                            {
                                //check if user is expired
                                bool parsedDate = false;
                                /*
                                 * Notice: Chubb Directory will export the date in the all user report according to the operating system's
                                 * culture settings. This means the date format could change between (mm/dd/yy) or (dd/mm/yy) for example.
                                 */
                                parsedDate = int.TryParse(expiry.Split(@"/")[0], out int M);
                                parsedDate = int.TryParse(expiry.Split(@"/")[1], out int D);
                                parsedDate = int.TryParse(expiry.Split(@"/")[2], out int Y);
                                //if we have datetime data, make datetimeobject
                                expiryDateTime = parsedDate ? new DateTime(Y+2000, M, D) : null;

                                DateTime today = DateTime.Now;
                                DateTime comparableToday = new DateTime(today.Year, today.Month, today.Day);
                                
                                DateTime expiryDate = parsedDate ? new DateTime(Y, M, D) : comparableToday;
                                if (expiryDate.CompareTo(comparableToday) < 0)
                                {
                                    //user expired
                                    expired = true;
                                }
                            }
                        }
                        string authorityPlus = headers.ContainsKey("Authority Plus") ? row[headers["Authority Plus"]].Trim() : "";
                        //make a chubbUser out of the info
                        //string dCode, string type, string userName, string fName, string lName, string id, string statusCode, string authority = "-1", string chubbNum = "", string authorityNumber = "-1", string pin = "-1"
                        ChubbUser usr;
                        try
                        {
                            //(string firstName, string lastName, string westernID, string chubbUserNumber, DateTime? expiry, string authority, string pin, string ? authorityPlus = null, DateTime ? authPlusExpiry = null
                            usr = new(fName, lName, studentnumber, usrNumber, expiryDateTime, authority, "1234", authPlus, null);
                            //ChubbUser usr = new(fName, lName, studentnumber, usrNumber, expiryDateTime, authority, "1234", authPlus, AuthPlusEnd);
                        }
                        catch (Exception e)
                        {

                            System.Diagnostics.Debug.WriteLine($"Could not properly add chubb usr with data {fName}, {lName}, {studentnumber}, {usrNumber}, {expiry}, {authority}, 1234, {authPlus}, {AuthPlusEnd}: Exception msg: {e.Message}");
                            NotificationList.Messages.Add($"Could not properly add chubb usr with data {fName}, {lName}, {studentnumber}, {usrNumber}, {expiry}, {authority}, 1234, {authPlus}, {AuthPlusEnd}");
                            return false;
                        }
                        //ChubbUser usr = new(fName, lName, studentnumber, usrNumber, expiry, authority, "1234", authorityPlus,  AuthPlusEnd);
                        if (ChubbUsers.ContainsKey(studentnumber))
                        {
                            NotificationList.Messages.Add($@"Duplicate Card number found in Chubb report. #{usrNumber}:{fName} {lName} and #{ChubbUsers[studentnumber].ChubbNumber}:{ChubbUsers[studentnumber].FirstName} {ChubbUsers[studentnumber].FirstName}");
                        }
                        else
                        {
                            this.ChubbUsers.Add(studentnumber, usr);
                            if (expired) { this.ExpiredUsers.Add(studentnumber, usr); }
                        }

                        //check if auth group is already present for this user, if not create a new one and add to list

                        bool addedUser = false;
                        if (AuthorityGroups.ContainsKey(authority))
                        {
                            addedUser = AuthorityGroups[authority].AddMember(usr);
                        }
                        else
                        {
                            AuthorityGroups.Add(authority, new AuthorityGroup(authority));
                            addedUser = AuthorityGroups[authority].AddMember(usr);
                        }
                        if (!addedUser)
                        {
                            NotificationList.Messages.Add($"Could not add User {fName} {lName} id: {studentnumber} to an authority group - No matching group found  for: {authority}");
                        }
                        
                    }
                    
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                System.Diagnostics.Debug.WriteLine("The Chubb Report file could not be read:");
                System.Diagnostics.Debug.WriteLine(e.Message);
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

        /******************************************************
        * Returns a list of all Groups found in Chubb Director
        ******************************************************/
        public List<string> getAuthorityGroupNames()
        {
            List<string> returnList = new();
            foreach (AuthorityGroup g in this.AuthorityGroups.Values)
            {
                returnList.Add(g.GroupName);
            }
            return returnList;
        }

        public List<string> getAuthorityGroupDescriptions()
        {
            List<string> returnList = new();
            foreach (AuthorityGroup g in this.AuthorityGroups.Values)
            {
                returnList.Add(g.GroupDescription);
            }
            return returnList;
        }


        //    /*
        //     * Gets the next free usernumber which can be used with a chubb user and inserted freely into an import file.
        //     * By running this function, the pool of available numbers will be decreased by one, and the returned
        //     * number (as a string) will be removed from availability. It is not recoverable. 
        //     */
        //    public string TakeNextFreeChubbUserNumber(string minNum = "0")
        //    {
        //        string stringMinNum = minNum;
        //        //convert both valus to ints

        //        int currentIndexValue;
        //        int minNumValue;
        //        if (Int32.TryParse(this.ChubbUserNumberCurrentIndex.Item2, out currentIndexValue) && Int32.TryParse(minNum, out minNumValue))
        //        {
        //            if (currentIndexValue <= minNumValue)
        //            {
        //                //The value at the current index is less than the min number, so we must find a new new index corresponding to 
        //                //a new chubb user number. We can then extract numbers in order from that point (until reset occurs or a new min value is passed)
        //                //if string num greater than minNum take. else next...
        //                for (int i = 0; i < this.UserNumbers.Count(); i++)
        //                {
        //                    //if string num greater than minNum take. else next...
        //                    if (Int32.TryParse(this.UserNumbers[i], out currentIndexValue))
        //                    {
        //                        if (currentIndexValue < minNumValue)
        //                        {
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            this.ChubbUserNumberCurrentIndex = new(i, this.UserNumbers[i]);
        //                            break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return "-1";
        //                    }
        //                }
        //            }
        //            if (this.UserNumbers.Count() > this.ChubbUserNumberCurrentIndex.Item1)
        //            {
        //                string returnNumber = this.UserNumbers[this.ChubbUserNumberCurrentIndex.Item1];
        //                this.UserNumbers.RemoveAt(this.ChubbUserNumberCurrentIndex.Item1);
        //                return returnNumber;
        //            }
        //        }

        //        return "-1"; //default return value

        //    }


        public void ResetChubbNumberGenerator()
        {
            this.ChubbUserNumberCurrentIndex = new(0, "0");
        }



        /*
         * Checks if a User already Exists in the Report
         */
        public bool hasUser(string userID)
        {
            if (this.ChubbUsers.ContainsKey(userID.Trim()))
            {
                return true;
            }
            else { return false; }
        }

        public ChubbUser? GetUser(string userID)
        {
            if (this.ChubbUsers.ContainsKey(userID.Trim()))
            {
                return ChubbUsers[userID.Trim()];
            }
            else return null;
        }


        //    /***
        //     * Provides a list of availabe user numbers in chubb directory. This assumes that the provided chubb director report
        //     * is up to date and that no further insertions have been done.
        //     */
        //    public List<string> GetBlankUserNumbers()
        //    {
        //        List<string> userNums = new List<string>();
        //        //parse file to find the available user numbers...or maybe go through the users and find the spaces between?
        //        return userNums;
        //    }

        //    /***
        //     * Provides a list of expired user numbers that can be overwritten in chubb director. This helps to recycle the existing usernumbers that are
        //     * being used up by expired accounts.
        //     */
        //    public List<string> GetExpiredUserNumbers()
        //    {
        //        List<string> userNums = new List<string>();
        //        //parse file to find the available user numbers...or maybe go through the users and find the spaces between?
        //        return userNums;
        //    }


        public bool BuildAllDepartments()
        {
            foreach (string d in this.DepartmentGroups.Keys)
            {
                if (d == "") { continue; }
                this.Departments.TryAdd(d, new Department(d));
                //this.Departments.Add(d, new Department(d));
                foreach (AuthorityGroup a in this.DepartmentGroups[d])
                {
                    this.Departments[d].AddAuthorityGroup(a);
                }
            }
            return true;
        }

        //    public bool ExportAllDepartmentFiles(string RootFolder)
        //    {
        //        if (!BuildAllDepartments())
        //        {
        //            AlertUserOkBox("could not build Department Data", "Department Error");
        //            return false;
        //        }

        //        DirectoryInfo dir = new DirectoryInfo(RootFolder);

        //        foreach (Department d in this.Departments.Values)
        //        {
        //            string fileName = d.Name;
        //            bool foundFolder = false;
        //            foreach (DirectoryInfo oneDir in dir.EnumerateDirectories())
        //            {
        //                if (oneDir.Name == d.Name)
        //                {
        //                    //no need to create new file
        //                    foundFolder = true;
        //                }
        //            }
        //            if (!foundFolder)
        //            {

        //                dir.CreateSubdirectory(d.Name);
        //            }
        //            //now able to make new file
        //            string DateAppend = DateTime.Now.ToString("d").Replace(@"/", "-");
        //            d.GenerateExcelSheet($@"{dir.FullName}\{d.Name}\{d.Name}-{DateAppend}.xlsx");
        //        }
        //        return true;
        //    }

        //    public bool ExportOneDepartmentFiles(string RootFolder, string DepartmentName)
        //    {
        //        this.Departments.Clear();
        //        if (BuildAllDepartments())
        //        {
        //            DirectoryInfo dir = new DirectoryInfo(RootFolder);
        //            Department d = this.Departments[DepartmentName];
        //            string fileName = d.Name;

        //            string DateAppend = DateTime.Now.ToString("d").Replace(@"/", "-");
        //            d.GenerateExcelSheet($@"{dir.FullName}\{d.Name}-{DateAppend}.xlsx");
        //            return true;
        //        }
        //        else
        //        {
        //            AlertUserOkBox("could not build Department Data", "Department Error");
        //            return false;
        //        }
        //    }
        //    public int GetExpiredUsersCount()
        //    {
        //        return ExpiredUsers.Count;
        //    }


        // Function to check for the most recent all users chubb report. This is a report that is exported from chubb director
        // and contains all the present users in the system. In this function, we use the configured (see top of class region)
        // path to the folder which contians the reports. The path to the most recent one is returned as a string.
        private string InferFile()
        {
            // find the latest file in chubb reports folder from configuration at top of file.
            // This has the effect of setting the objects file
            bool foundReport = false;
            string latestFile = "";
            if (Directory.Exists(_folderPath))
            {
                var txtFiles = Directory.EnumerateFiles(_folderPath, "*.txt");

                DateTime latest = DateTime.MinValue;

                //Go through all files in the folder and check for the most recent one with a name that begins with 'AllUsers'
                foreach (var txtFile in txtFiles)
                {
                    FileInfo fileInfo = new FileInfo(txtFile);
                    if (fileInfo.Name.StartsWith("AllUsers") && fileInfo.Length > 0)
                    {

                        if (!foundReport)
                        {
                            latest = fileInfo.CreationTime;
                            latestFile = fileInfo.FullName;
                            foundReport = true;
                        }
                        else
                        {
                            //checks if this file is created after all other seen so far
                            if (latest.CompareTo(fileInfo.CreationTime) < 0)
                            {
                                latest = fileInfo.CreationTime;
                                latestFile = fileInfo.FullName;
                            }
                        }
                    }
                }
            }
            return latestFile;

        }

        /*
         * Gets the next free usernumber which can be used with a chubb user and inserted freely into an import file.
         * By running this function, the pool of available numbers will be decreased by one, and the returned
         * number (as a string) will be removed from availability. It is not recoverable. 
         */
        public string TakeNextFreeChubbUserNumber(string minNum = "0")
        {
            string stringMinNum = minNum;
            //convert both valus to ints

            int currentIndexValue;
            int minNumValue;
            if (Int32.TryParse(this.ChubbUserNumberCurrentIndex.Item2, out currentIndexValue) && Int32.TryParse(minNum, out minNumValue))
            {
                if (currentIndexValue <= minNumValue)
                {
                    //The value at the current index is less than the min number, so we must find a new new index corresponding to 
                    //a new chubb user number. We can then extract numbers in order from that point (until reset occurs or a new min value is passed)
                    //if string num greater than minNum take. else next...
                    for (int i = 0; i < this.UserNumbers.Count(); i++)
                    {
                        //if string num greater than minNum take. else next...
                        if (Int32.TryParse(this.UserNumbers[i], out currentIndexValue))
                        {
                            if (currentIndexValue < minNumValue)
                            {
                                continue;
                            }
                            else
                            {
                                this.ChubbUserNumberCurrentIndex = new(i, this.UserNumbers[i]);
                                break;
                            }
                        }
                        else
                        {
                            return "-1";
                        }
                    }
                }
                if (this.UserNumbers.Count() > this.ChubbUserNumberCurrentIndex.Item1)
                {
                    string returnNumber = this.UserNumbers[this.ChubbUserNumberCurrentIndex.Item1];
                    this.UserNumbers.RemoveAt(this.ChubbUserNumberCurrentIndex.Item1);
                    return returnNumber;
                }
            }
            return "-1"; //default return value
        }

        /*
         * Function to retrieve Authority group information from the xlsx file stored
         * in the default location (determined by config at top of this file).
         * This function iterates over the file line by line to retrieve the rooms
         * description, and department for each authority group. It calls the function
         * AddAuthorityGroupInfo so that the existing dict this.AuthorityGroups can 
         * be updated with the relevant information. 
         * Params:
         *      importFile: path to the authority group info file. This will override
         *      the default configuration.
         * Returns:
         *      True: if info added
         *      false: if something went wrong with reading file and/or adding info to 
         *      a group
         */
        private bool InferGroupInfo(string? importFile=null)
        {
            //open file for parsing
            try
            {
                if (!(File.Exists(this.GroupInfoFile.FullName))) 
                { 
                    System.Diagnostics.Debug.WriteLine("Group Info File Not Found!");
                    NotificationList.Messages.Add($"Could not find Authority Group information file. {this.GroupInfoFile.FullName}");
                    return false; 
                }
                using (ExcelPackage package = new ExcelPackage(this.GroupInfoFile.FullName))
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First(); //file created already so should have a sheet
                    if (worksheet.Dimension == null) { return false; }
                    var start = worksheet.Dimension.Start;
                    var end = worksheet.Dimension.End;
                    for (int row = 1; row <= end.Row; row++) //skip headers
                    { // Row by row...
                        
                        //populate dictionaries with row 
                        //number 
                        string groupNum = worksheet.Cells[row, 1].Value == null ? "" : worksheet.Cells[row, 1].Value.ToString();

                        //If we have reached the end of the user groups continue
                        if (groupNum == "") { continue; }
                        //name    
                        string? groupName = worksheet.Cells[row, 2].Value == null ? "" : worksheet.Cells[row, 2].Value.ToString().Trim();
                        if (groupName == "") { continue; }

                        string? groupDepartment = worksheet.Cells[row, 3].Value == null ? "No Department" : worksheet.Cells[row, 3].Value.ToString().Trim();
                        string? groupDescription = worksheet.Cells[row, 4].Value == null ? "" : worksheet.Cells[row, 4].Value.ToString().Trim();

                        int col = 5;
                        List<string> rooms = new();
                        while (worksheet.Cells[row, col].Value != null)
                        {
                            rooms.Add(worksheet.Cells[row, col].Value.ToString().Trim());
                            col++;
                        }
                        try
                        {
                            if (groupName != null)
                            {

                                if (this.AuthorityGroups.ContainsKey(groupName))
                                {
                                    //found a group with this nmber in chubb report, update the groups information
                                    this.AddAuthorityGroupInfo(groupNum, groupName, groupDepartment, groupDescription, rooms);

                                }
                                else
                                {
                                    if (!(groupDepartment == "No Department"))
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Group {groupNum}: {groupName} had no matches in Chubb Report. Check if this group is expected to have users in it: Group Import Error");
                                        NotificationList.Messages.Add($"Group {groupNum}: {groupName} had no matches in Chubb Report. Check if this group is expected to have users in it: Group Import Error");
                                    }
                                    //return false;
                                }
                            }
                        }
                        catch (Exception e3)
                        {
                            System.Diagnostics.Debug.WriteLine(e3.Message);
                        }

                    }
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message);
                return false;
            }
            return true;

        }
    }
}
