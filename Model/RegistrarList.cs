using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Model
{
    public class RegistrarList
    {
        #region config
        static string DefaultRegistrarFolderPath = @"W:\SSTSCardAccess\Registrar lists";
        #endregion

        private string? _registrarFile;
        public string RegistrarFile
        {
            get
            {
                return _registrarFile ?? string.Empty;
            }
            set
            {
                _registrarFile = value;
            }

        }
        public Dictionary<string, RegistrarUser> AllUsers { get; set; }

        public Dictionary<string, RegistrarUser> Matched { get; set; }

        public Dictionary<string, RegistrarUser> Unmatched { get; set; }

        public UserReportModel ChubbReport { get; set; }

        public Dictionary<string, int> AcadGroups;

        public Dictionary<string, int> StartLevels;

        public Dictionary<string, int> PrimaryProgarms;

        public RegistrarList(UserReportModel userReport)
        {
            ChubbReport = userReport;
            this.InitializeRegistrarList(true);
        }

        /*
         * This function reinitializes the object while still using the original
         * userReportModel.It will reset all collections and data in the object.
         * If the parameter is set to true, the registrars list will be inferred.
         * If false, the second paramter will be used as the registrar file.
         * 
         * Returns true if file found and parsed. Fase otherwise.
         */
        public bool InitializeRegistrarList(bool inferFile, string? newRegistrarFile=null)
        {
            AllUsers = new Dictionary<string, RegistrarUser>();
            Matched = new Dictionary<string, RegistrarUser>();
            Unmatched = new Dictionary<string, RegistrarUser>();
            AcadGroups = new Dictionary<string, int>();
            StartLevels = new Dictionary<string, int>();
            PrimaryProgarms = new Dictionary<string, int>();
            if(inferFile)
            {
                if (!InferRegistrarFile())
                {
                    ChubbReport.NotificationList.Messages.Add("Could not infer a Registrar File. Please add one manually.");
                    return false;
                }
            }
            else
            {
                //using passed file
                if(newRegistrarFile == null) 
                {
                    ChubbReport.NotificationList.Messages.Add("Null Registrar File passed. Cannot process registrar list");
                    return false; 
                }
                if(!File.Exists(newRegistrarFile))
                {
                    ChubbReport.NotificationList.Messages.Add("Cannot find Registrar File. Please try another file and/or make sure your network drives are connected");
                    return false;
                }
                RegistrarFile = newRegistrarFile;
            }
            if(!ParseRegistrarFile())
            {
                ChubbReport.NotificationList.Messages.Add("Cannot Parse the Registrar File. Please check that file is valid.");
                return false;
            }
            return true;
        }

        public bool InferRegistrarFile()
        {
            string year = DateTime.Now.Year.ToString();
            string registrarDirectory = DefaultRegistrarFolderPath;
            string yearlySubDir = $"{DefaultRegistrarFolderPath}\\{year}";

            if (Directory.Exists(DefaultRegistrarFolderPath))
            {
                DateTime latestFileDate = DateTime.MinValue;
                FileInfo? latestFile = null;
                //Get into this years directory if exists:
                foreach(string directoryName in Directory.GetDirectories(DefaultRegistrarFolderPath))
                {
                    if(directoryName.StartsWith(yearlySubDir))
                    {
                        registrarDirectory = directoryName;
                    }
                }
                //search directory for most recent file
                foreach (string currentFile in Directory.GetFiles(registrarDirectory))
                {
                    FileInfo info = new FileInfo(currentFile);
                    if (info.CreationTime > latestFileDate)
                    {
                        latestFileDate = info.CreationTime;
                        latestFile = info;
                    }
                }
                if (latestFile != null)
                {
                    _registrarFile = latestFile?.FullName;
                    return true;
                }
                else
                {
                    ChubbReport.NotificationList.Messages.Add("Could not infer the Registrar file. No file found in registrar list directory");
                }
            }
            ChubbReport.NotificationList.Messages.Add("Default directory for registrar file can not be found. Please check that this directory is connected.");
            return false;
        }

        public bool ParseRegistrarFile()
        {
            if (!File.Exists(RegistrarFile)) return false;
            try
            {
                IWorkbook workbook;
                FileStream fs = new FileStream(RegistrarFile, FileMode.Open, FileAccess.Read);
                if (RegistrarFile.IndexOf(".xlsx") > 0)
                    workbook = new XSSFWorkbook(fs);
                else workbook = new HSSFWorkbook(fs); //default to xls as this is the file format from registrar

                ISheet sheet = workbook.GetSheetAt(0);

                if (sheet != null)
                {
                    int rowCount = sheet.LastRowNum; // This may not be valid row count.
                                                     // If first row is table head, i starts from 1
                    for (int i = 1; i <= rowCount; i++)
                    {
                        IRow curRow = sheet.GetRow(i);
                        string? westernId;
                        string? firstName;
                        string? lastName;
                        string? startLevel;
                        string? primaryProgram;
                        string? acadGroup;
                        // Works for consecutive data. Use continue otherwise 
                        if (curRow == null)
                        {
                            // Valid row count
                            rowCount = i - 1;
                            break;
                        }
                        // Get data from the 4th column (4th cell of each row)
                        //var cellValue = curRow.GetCell(0).StringCellValue.Trim();
                        westernId = curRow.GetCell(0).StringCellValue.Trim().TrimStart('0');
                        lastName = curRow.GetCell(1).StringCellValue.Trim();
                        firstName = curRow.GetCell(2).StringCellValue.Trim();
                        primaryProgram = curRow.GetCell(3).StringCellValue.Trim();
                        startLevel = curRow.GetCell(4).StringCellValue.Trim();
                        acadGroup = curRow.GetCell(5).StringCellValue.Trim();

                        //Gather counts for start level, program, acadgroup

                        if (!this.AcadGroups.ContainsKey(acadGroup))
                        {
                            this.AcadGroups.Add(acadGroup, 0);
                        }
                        this.AcadGroups[acadGroup] += 1;


                        if (!this.PrimaryProgarms.ContainsKey(primaryProgram))
                        {
                            this.PrimaryProgarms.Add(primaryProgram, 0);
                        }
                        this.PrimaryProgarms[primaryProgram] += 1;


                        if (!this.StartLevels.ContainsKey(startLevel))
                        {
                            this.StartLevels.Add(startLevel, 0);
                        }
                        this.StartLevels[startLevel] += 1;

                        //Make new registrar user with row data
                        RegistrarUser usr = new(firstName, lastName, westernId, primaryProgram, startLevel, acadGroup);
                        if (ChubbReport.hasUser(usr.WesternId))
                        {
                            //matched user
                            if (!usr.UpdateMatchedUser(this.ChubbReport.GetUser(westernId)))
                            {
                                Debug.WriteLine("Warning found duplicate user that already exists in chubb report.");
                            }
                            if (!this.Matched.TryAdd(usr.WesternId, usr)) 
                            { 
                                ChubbReport.NotificationList.Messages.Add($"Found duplicate Western Card Number in registrar file: {usr.WesternId}");
                                continue;
                            }
                        }
                        else
                        {
                            //Default date to the next may
                            int currentMonth = DateTime.Now.Month;
                            DateTime defaultExpiry = currentMonth < 5 ? new DateTime(DateTime.Now.Year, 05, 01) : new DateTime(DateTime.Now.Year+1, 05,01);
                            usr.Expiry = defaultExpiry;
                            //unmatched user
                            if (!this.Unmatched.TryAdd(usr.WesternId, usr)) 
                            { 
                                ChubbReport.NotificationList.Messages.Add($"Found duplicate Western Card Number in registrar file: {usr.WesternId}");
                                continue;
                            }
                            
                        }
                        //Either way we add the user to all Users dict
                        //this.AllUsers.Add(westernId, usr);
                        if (!this.AllUsers.TryAdd(usr.WesternId, usr)) 
                        { 
                            ChubbReport.NotificationList.Messages.Add($"Found duplicate Western Card Number in registrar file: {usr.WesternId}");
                            continue;
                        }
                        //Console.WriteLine(cellValue);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
            return false;
        }
    } 
}
