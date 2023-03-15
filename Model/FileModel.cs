using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChubbHubMVVM.Model
{
    public class FileModel
    {
        public static FileInfo? SelectFile(string fileName, string defaultExtension, string filter, string defaultFileName, string initialDirectory)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = fileName; // Default file name
            dialog.DefaultExt = defaultExtension; // Default file extension
            dialog.Filter = filter; // Filter files by extension
            dialog.InitialDirectory = initialDirectory;
            //Accept file from user
            bool? result = dialog.ShowDialog();
            string SelectedFileName = string.Empty;

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                SelectedFileName = dialog.FileName;
                try
                {
                    FileInfo returnFileInfo = new FileInfo(SelectedFileName);
                    return new FileInfo(SelectedFileName);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Unable to get file, try again.");
                    return SelectFile(fileName, defaultExtension, filter, defaultFileName, initialDirectory);
                }
            }
            else return null;

        }
    
        
    }
}
