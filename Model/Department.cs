using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;


namespace ChubbHubMVVM.Model
{

    /*
     * Model of a department. Each department has a name and description and can have many authority groups. 
     */
    public class Department
    {

        public string Name { get; set; }
        public string Description { get; set; }

        public Dictionary<string, AuthorityGroup> AuthorityGroups;

        public Department(string name)
        {
            this.Name = name;
            this.AuthorityGroups = new();
        }

        //public bool GenerateExcelSheet(string SaveLocation)
        //{
        //    //get the department for the group, open the departments xlxs file
        //    // add in the first group, passed by groupName
        //    // for every member of groupName (in passed param authority group), we check for
        //    // the authority plus to see if any other groups should be added to the file. 
        //    FileInfo depFile = new FileInfo(SaveLocation);
        //    if (depFile.Exists)
        //    {
        //        depFile.Delete();
        //    }

        //    string headers = "User, First Name, Last Name, System Authority, Card Number, Invalid On, Authority Plus, Authority Plus End";

        //    using (ExcelPackage package = new ExcelPackage(depFile))
        //    {
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Card Access Groups");
        //        int row = 1;
        //        foreach (AuthorityGroup g in AuthorityGroups.Values)
        //        {
        //            worksheet.Cells[row, 1].Value = $"Group {g.GroupNumber}";
        //            worksheet.Cells[row, 1].Style.Font.Bold = true;
        //            worksheet.Cells[row, 2].Value = g.GroupName;
        //            row++;
        //            worksheet.Cells[row, 1].Value = $"Rooms";
        //            worksheet.Cells[row, 2].Value = $"{g.GroupDescription}";
        //            worksheet.Cells[row, 2].Style.Font.Bold = true;
        //            row++;
        //            worksheet.Cells[row, 1].LoadFromText(headers);
        //            worksheet.Cells[row, 1, row, 8].Style.Font.Bold = true;
        //            worksheet.Cells[row, 1, row, 8].Style.Font.UnderLine = true;
        //            row++;

        //            foreach (ChubbUser user in g.GroupUsers.Values)
        //            {
        //                //get authplus info to check if we need to include more groups in this file
        //                string UserNumber = user.chubbUserNumber;
        //                worksheet.Cells[row, 1].Value = UserNumber;

        //                string UserName = user.FirstName;
        //                worksheet.Cells[row, 2].Value = UserName;

        //                string LastName = user.LastName;
        //                worksheet.Cells[row, 3].Value = LastName;

        //                string SystemAuthority = user.authority;
        //                worksheet.Cells[row, 4].Value = SystemAuthority;

        //                string CardNumber = user.StudentId;
        //                worksheet.Cells[row, 5].Value = CardNumber;

        //                string InvalidOn = user.ExpiryDate.ToString("d");
        //                worksheet.Cells[row, 6].Value = InvalidOn;

        //                string AuthorityPlus = user.authorityPlus;
        //                worksheet.Cells[row, 7].Value = AuthorityPlus;

        //                string AuthorityPlusEnd = user.authorityPlusExpiry;
        //                worksheet.Cells[row, 8].Value = AuthorityPlusEnd;
        //                row++;
        //            }
        //            row += 2;
        //        }

        //        package.Save();
        //    }

        //    return true;
        //}

        public bool AddAuthorityGroup(AuthorityGroup group)
        {
            this.AuthorityGroups.TryAdd(group.GroupName, group);
            return true;
        }

        public bool RemoveAuthorityGroup(string groupName)
        {
            return true;
        }

    }
}
