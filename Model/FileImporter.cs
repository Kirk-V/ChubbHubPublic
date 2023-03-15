using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ChubbHubMVVM.Model
{
    public class FileImporter
    {

        //Dictionary where the Key is the student ID 
        //Dictionary<string, Dictionary<string, string>> ImportData = new Dictionary<string, Dictionary<string, string>>();

        //Dataframe for users
        DataTable ImportDataTable;
        //Dictionary<string, ChubbUser> Users;
        private string saveFile;

        public FileImporter(string FileName)
        {
            this.ImportDataTable = new DataTable();
            this.MakeTable();
            this.saveFile = FileName;
            //this.Users = Users;
        }

        public FileImporter(string FileName, bool MatchedFile)
        {
            this.ImportDataTable = new DataTable();
            this.MakeMatchedTable();
            //this.Users = Users;
        }

        private void MakeMatchedTable()
        {
            this.ImportDataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("ChubbID", typeof(string)),
                new DataColumn("FirstName", typeof(string)),
                new DataColumn("LastName", typeof(string)),
                new DataColumn("Pin", typeof(string)),
                new DataColumn("AuthorityName", typeof(string)),
                new DataColumn("AuthorityNumber", typeof(string)),
                new DataColumn("Expiry", typeof(string)),
                new DataColumn("AuthorityPlusName", typeof(string)),
                new DataColumn("AuthorityPlusNumber", typeof(string)),
                new DataColumn("ExpiryPlus", typeof(string)),
            });
        }


        public bool AddMatchedUser(string chubbId, string fname, string lname, string pin, string authorityName, string authorityNumber, string? expiry, string? authorityPlusName, string? authorityPlusNumber, string? expiryPlus)
        {
            DataRow row;
            row = this.ImportDataTable.NewRow();
            row["ChubbID"] = chubbId;
            row["FirstName"] = fname;
            row["LastName"] = lname;
            row["Pin"] = pin;
            row["AuthorityName"] = authorityName;
            row["AuthorityNumber"] = authorityNumber;
            row["Expiry"] = expiry;
            row["AuthorityPlusName"] = authorityPlusName;
            row["AuthorityPlusNumber"] = authorityPlusNumber;
            row["ExpiryPlus"] = expiryPlus;
            this.ImportDataTable.Rows.Add(row);
            return true;
        }
        private void MakeTable()
        {

            this.ImportDataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("ChubbID", typeof(string)),
                new DataColumn("Language", typeof(string)),
                new DataColumn("FirstName", typeof(string)),
                new DataColumn("LastName", typeof(string)),
                new DataColumn("Pin", typeof(string)),
                new DataColumn("AuthorityNumber", typeof(string)),
                new DataColumn("chall", typeof(string)),
                new DataColumn("Card#", typeof(string)),
                new DataColumn("CardVersion", typeof(string)),
                new DataColumn("Custom Fields 1", typeof(string)),
                new DataColumn("Custom Fields 2", typeof(string)),
                new DataColumn("Custom Fields 3", typeof(string)),
                new DataColumn("Custom Fields 4", typeof(string)),
                new DataColumn("Custom Fields 5", typeof(string)),
                new DataColumn("Custom Fields 6", typeof(string)),
                new DataColumn("Custom Fields 7", typeof(string)),
                new DataColumn("Custom Fields 8", typeof(string)),
                new DataColumn("Custom Fields 9", typeof(string)),
                new DataColumn("Custom Fields 10", typeof(string)),
                new DataColumn("Custom Fields 11", typeof(string)),
                new DataColumn("Custom Fields 12", typeof(string)),
                new DataColumn("Custom Fields 13", typeof(string)),
                new DataColumn("Custom Fields 14", typeof(string)),
                new DataColumn("Custom Fields 15", typeof(string)),
                new DataColumn("Custom Fields 16", typeof(string)),
                new DataColumn("Custom Fields 17", typeof(string)),
                new DataColumn("Custom Fields 18", typeof(string)),
                new DataColumn("Custom Fields 19", typeof(string)),
                new DataColumn("Custom Fields 20", typeof(string)),
                new DataColumn("suiteauthority", typeof(string)),
                new DataColumn("Zero", typeof(string)),
                new DataColumn("ValidOn", typeof(string)),
                new DataColumn("Expiry", typeof(string)),
                new DataColumn("Photofile", typeof(string)),
                new DataColumn("email", typeof(string)),
                new DataColumn("phone", typeof(string)),
                new DataColumn("Carrier", typeof(string))
            });
        }

        /*Convert a chubb user to the row format and insert into the table*/
        public bool AddUser(ChubbUserModel usr, string AuthorityNumber)
        {
            DataRow row;
            row = this.ImportDataTable.NewRow();
            row["ChubbID"] = usr.ChubbUserNumber;
            row["Language"] = "0";
            row["FirstName"] = usr.FirstName;
            row["LastName"] = usr.LastName;
            row["Pin"] = usr.Pin;
            row["AuthorityNumber"] = AuthorityNumber;
            row["chall"] = "0";
            row["Card#"] = usr.WesternID;
            row["CardVersion"] = "0";
            row["Custom Fields 1"] = ""; row["Custom Fields 2"] = ""; row["Custom Fields 3"] = ""; row["Custom Fields 4"] = ""; row["Custom Fields 5"] = "";
            row["Custom Fields 6"] = ""; row["Custom Fields 7"] = ""; row["Custom Fields 8"] = ""; row["Custom Fields 9"] = ""; row["Custom Fields 10"] = "";
            row["Custom Fields 11"] = ""; row["Custom Fields 12"] = ""; row["Custom Fields 13"] = ""; row["Custom Fields 14"] = ""; row["Custom Fields 15"] = "";
            row["Custom Fields 16"] = ""; row["Custom Fields 17"] = ""; row["Custom Fields 18"] = ""; row["Custom Fields 19"] = ""; row["Custom Fields 20"] = "";
            row["suiteauthority"] = "0";
            row["Zero"] = "0";
            
            row["ValidOn"] = "0";
            row["Expiry"] = usr.ExpiryDate;
            row["Photofile"] = "";
            row["email"] = "";
            row["phone"] = "";
            row["Carrier"] = "";

            this.ImportDataTable.Rows.Add(row);
            return true;
        }

        /*OVerload of AddUser to take a ChubbUser (not the old chubb user modle)*/
        public bool AddUser(ChubbUser usr, string AuthorityNumber)
        {
            DataRow row;
            row = this.ImportDataTable.NewRow();
            row["ChubbID"] = usr.ChubbNumber;
            row["Language"] = "0";
            row["FirstName"] = usr.FirstName;
            row["LastName"] = usr.LastName;
            row["Pin"] = usr.Pin;
            row["AuthorityNumber"] = AuthorityNumber;
            row["chall"] = "0";
            row["Card#"] = usr.WesternId;
            row["CardVersion"] = "0";
            row["Custom Fields 1"] = ""; row["Custom Fields 2"] = ""; row["Custom Fields 3"] = ""; row["Custom Fields 4"] = ""; row["Custom Fields 5"] = "";
            row["Custom Fields 6"] = ""; row["Custom Fields 7"] = ""; row["Custom Fields 8"] = ""; row["Custom Fields 9"] = ""; row["Custom Fields 10"] = "";
            row["Custom Fields 11"] = ""; row["Custom Fields 12"] = ""; row["Custom Fields 13"] = ""; row["Custom Fields 14"] = ""; row["Custom Fields 15"] = "";
            row["Custom Fields 16"] = ""; row["Custom Fields 17"] = ""; row["Custom Fields 18"] = ""; row["Custom Fields 19"] = ""; row["Custom Fields 20"] = "";
            row["suiteauthority"] = "0";
            row["Zero"] = "0";
            row["ValidOn"] = usr.ValidOn?.ToString("MM/dd/yy") ?? "0";
            //row["ValidOn"] = "0";
            row["Expiry"] = usr.ExpiryString;
            row["Photofile"] = "";
            row["email"] = "";
            row["phone"] = "";
            row["Carrier"] = "";

            this.ImportDataTable.Rows.Add(row);
            return true;
        }

        /*
         * Add a user to the table. Once added the user will be included in the output data (either csv or txt file)
         */
        public bool AddUser(ChubbUser usr)
        {
            DataRow row;
            row = this.ImportDataTable.NewRow();
            row["ChubbID"] = usr.ChubbNumber;
            row["Language"] = "0";
            row["FirstName"] = usr.FirstName;
            row["LastName"] = usr.LastName;
            row["Pin"] = usr.Pin;
            row["AuthorityNumber"] = usr.AuthorityNumber;
            row["chall"] = "0";
            row["Card#"] = usr.WesternId;
            row["CardVersion"] = "0";
            row["Custom Fields 1"] = ""; row["Custom Fields 2"] = ""; row["Custom Fields 3"] = ""; row["Custom Fields 4"] = ""; row["Custom Fields 5"] = "";
            row["Custom Fields 6"] = ""; row["Custom Fields 7"] = ""; row["Custom Fields 8"] = ""; row["Custom Fields 9"] = ""; row["Custom Fields 10"] = "";
            row["Custom Fields 11"] = ""; row["Custom Fields 12"] = ""; row["Custom Fields 13"] = ""; row["Custom Fields 14"] = ""; row["Custom Fields 15"] = "";
            row["Custom Fields 16"] = ""; row["Custom Fields 17"] = ""; row["Custom Fields 18"] = ""; row["Custom Fields 19"] = ""; row["Custom Fields 20"] = "";
            row["suiteauthority"] = "0";
            row["Zero"] = "0";
            row["ValidOn"] = usr.ValidOn?.ToString("MM/dd/yy") ?? "0";
            //row["ValidOn"] = "0";
            row["Expiry"] = usr.ExpiryString;
            row["Photofile"] = "";
            row["email"] = "";
            row["phone"] = "";
            row["Carrier"] = "";

            this.ImportDataTable.Rows.Add(row);
            return true;
        }


        //Overload with only chubbusr for data source, Chubb user must have authority number information 
        public bool AddUser(ChubbUserModel usr)
        {
            DataRow row;
            row = this.ImportDataTable.NewRow();
            row["ChubbID"] = usr.ChubbUserNumber;
            row["Language"] = "0";
            row["FirstName"] = usr.FirstName;
            row["LastName"] = usr.LastName;
            row["Pin"] = usr.Pin;
            row["AuthorityNumber"] = usr.AuthorityGroupNumber;
            row["chall"] = "0";
            row["Card#"] = usr.WesternID;
            row["CardVersion"] = "0";
            row["Custom Fields 1"] = ""; row["Custom Fields 2"] = ""; row["Custom Fields 3"] = ""; row["Custom Fields 4"] = ""; row["Custom Fields 5"] = "";
            row["Custom Fields 6"] = ""; row["Custom Fields 7"] = ""; row["Custom Fields 8"] = ""; row["Custom Fields 9"] = ""; row["Custom Fields 10"] = "";
            row["Custom Fields 11"] = ""; row["Custom Fields 12"] = ""; row["Custom Fields 13"] = ""; row["Custom Fields 14"] = ""; row["Custom Fields 15"] = "";
            row["Custom Fields 16"] = ""; row["Custom Fields 17"] = ""; row["Custom Fields 18"] = ""; row["Custom Fields 19"] = ""; row["Custom Fields 20"] = "";
            row["suiteauthority"] = "0";
            row["Zero"] = "0";
            row["ValidOn"] = "0";
            row["Expiry"] = usr.ExpiryDate;
            row["Photofile"] = "";
            row["email"] = "";
            row["phone"] = "";
            row["Carrier"] = "";

            this.ImportDataTable.Rows.Add(row);
            return true;
        }
        public bool ExportTableToImportFile(string FileSaveLocation)
        {
            StreamWriter w = new StreamWriter(FileSaveLocation, false);
            int index = 0;
            foreach (DataRow r in this.ImportDataTable.Rows)
            {
                index++;

                string[] arr = r.ItemArray.Select(x => x.ToString()).ToArray();
                //(string[])r.ItemArray;
                foreach (string str in arr)
                {
                    w.Write(str.ToString() + "\t");
                }
                w.Write("\n");
            }
            if (!(index == this.ImportDataTable.Rows.Count))
            {
                Debug.WriteLine("Unmatched number of rows inserted vs rows in table");
                return false;
            }
            w.Close();
            return true;
        }

        public bool ExportTableToCSV(string FileSaveLocation)
        {
            try
            {
                using (var writer = new StreamWriter(FileSaveLocation, false))
                {
                    /*
                     * Write the headers seperated by columns. Add newLine to end of column line and then write all the user data line by line
                     */
                    for (int i = 0; i < this.ImportDataTable.Columns.Count; i++)
                    {
                        writer.Write(this.ImportDataTable.Columns[i]);
                        if (i < this.ImportDataTable.Columns.Count - 1)
                        {
                            writer.Write(",");
                        }
                    }
                    writer.Write("\n");
                    foreach (DataRow row in this.ImportDataTable.Rows)
                    {
                        writer.WriteLine(string.Join(",", row.ItemArray));
                    }
                }
            }
            catch(Exception e)
            {
                //add alert here
                MessageBox.Show("Could not Save to File, Please close the file if it is opened or select a different save location.", "ChubbHubb", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            return true;
        }

        public bool MakeImportFile(string FileSaveLocation)
        {
            return true;
        }




    }
}
