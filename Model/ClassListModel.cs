using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Model
{
    public class ClassListModel
    {


        private string? _RCLFile;
        public string RCLFile 
        { 
            get
            {
                return _RCLFile ?? string.Empty;
            }
            set => _RCLFile = value; 
        }


        public string? ClassName { get; set; }
        
        public string? ClassYear { get; set; }
        public string? Department { get; set; }

        public string? ClassNumber { get; set; }

        public Dictionary<string, ChubbUser> AllUsers { get; set; }

        public Dictionary<string, ChubbUser> Matched { get; set; }

        public Dictionary<string, ChubbUser> Unmatched { get; set; }

        public UserReportModel ChubbReport { get; set; }
        public ClassListModel(UserReportModel ChubbReport)
        {
            
            this.AllUsers = new Dictionary<string, ChubbUser>();
            this.Matched = new Dictionary<string, ChubbUser>();
            this.Unmatched = new Dictionary<string, ChubbUser>();
            this.ChubbReport = ChubbReport;
        }

        private void ParseFile()
        {
            try
            {
                bool NameSet = false;
                string[] lines = System.IO.File.ReadAllLines(this.RCLFile);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(",").Select(x => x.Trim('"')).ToArray();

                    //RCL File has only four columns worth saving
                   
                    if (parts.Length > 0)
                    {
                        if (!NameSet)
                        {
                            this.ClassName = parts[6];
                            NameSet = true;
                            this.ClassYear = parts[6][..1];
                            this.Department = parts[5];
                            this.ClassNumber = parts[2];
                        }

                        string cardNumber = parts[8].Trim().TrimStart('0');

                        //string cardNumber = parts[8];

                        string FirstName = parts[10];
                        string LastName = parts[9];

                        ChubbUser usr = new(FirstName, LastName, cardNumber);

                        if (this.ChubbReport.hasUser(cardNumber.Trim()))
                        {

                            //ChubbUser MatchedUser = this.ChubbReport.ChubbUsers[cardNumber];

                            ChubbUser MatchedUser = this.ChubbReport.ChubbUsers[cardNumber.Trim()];

                            this.Matched.Add(cardNumber, MatchedUser);
                        }
                        else
                        {
                            this.Unmatched.Add(cardNumber, usr);
                        }
                        this.AllUsers.Add(cardNumber, usr);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }



        /*
         * This class will reinitialize the object with a new class file.
         */
        public void NewRCLFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                //found file, lets try and import its data
                RCLFile = filePath;
                AllUsers?.Clear();
                Matched?.Clear();
                Unmatched?.Clear();
                ParseFile();
            }
            
        }
    }
}
