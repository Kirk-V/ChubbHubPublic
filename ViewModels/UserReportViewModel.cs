using ChubbHubMVVM.Commands;
using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChubbHubMVVM.ViewModels
{
    /*
     * This class serves the bindings from within the UserRportView 
     */
    internal class UserReportViewModel: ViewModelBase
    {
        #region Configuration
        static private string _ReportFolder = @"W:\SSTSCardAccess\Reports";
        #endregion


        private UserReportModel? _userReport;

        private FileInfo? _fileName; //path to file
        public string FileName
        {
            get
            {
                if (_fileName == null) return string.Empty;
                return _fileName.Name;
            }
            set
            {
                _fileName = File.Exists(value) ? new FileInfo(value) : null;
                OnPropertyChanged(nameof(FileName));
            }
        }

        

        public UserReportViewModel(UserReportModel report, NavigationStore navStore, string? fileName = null): base(report, navStore )
        {
            _userReport = base._userReportModel;
            if (fileName == null)
            {
                //try to infer filename
                this.FileName = InferChubbReport();
            }
            else
            {
                this.FileName = fileName;
            }

            
        }

        
        

        // Function to check for the most recent all users chubb report. This is a report that is exported from chubb director
        // and contains all the present users in the system. In this function, we use the configured (see top of class region)
        // path to the folder which contians the reports. The path to the most recent one is returned as a string.
        private static string InferChubbReport()
        {
            //find the latest file in chubb reports
            string reportFolder = _ReportFolder;
            bool foundReport = false;
            string latestFile = "";
            if (Directory.Exists(reportFolder))
            {
                var txtFiles = Directory.EnumerateFiles(reportFolder, "*.txt");
                
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

    }
}
