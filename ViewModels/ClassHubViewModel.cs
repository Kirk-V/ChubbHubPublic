using ChubbHubMVVM.Commands;
using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace ChubbHubMVVM.ViewModels
{
    public class ClassHubViewModel : ViewModelBase
    {

        private ObservableCollection<ChubbUser>? _gridView;

        public ObservableCollection<ChubbUser>? GridView
        {
            get { return _gridView; }
            set
            {
                _gridView = value;
                this.GridViewWrapper = CollectionViewSource.GetDefaultView(_gridView);
                OnPropertyChanged(nameof(GridView));
            }
        }


        private ICollectionView? _gridViewWrapper;

        public ICollectionView? GridViewWrapper
        {
            get { return _gridViewWrapper; }
            set { _gridViewWrapper = value; OnPropertyChanged(nameof(GridViewWrapper)); }
        }


        private string? _selectedView;

        public string SelectedView
        {
            get
            {
                return _selectedView ?? string.Empty;
            }
            set
            {
                _selectedView = value;
                ChangeView(value);
                OnPropertyChanged(SelectedView);
            }
        }
        
        public string ChubbReportFile { get; }

        private string? _classYear;

        public string? ClassYear
        {
            get => _classYear ?? string.Empty;
            set { _classYear = value; OnPropertyChanged(nameof(ClassYear)); }
        }

        private string? _classFileName;

        public string ClassFileName
        {
            get { return _classFileName ?? String.Empty; }
            set
            {
                _classFileName = value;
                OnPropertyChanged(nameof(ClassFileName));
            }
        }

        private string? _classDepartment;

        private string? _selectedPin;

        public string? SelectedPin
        {
            get => _selectedPin ?? string.Empty;
            set
            {
                if(value == null)
                {
                    _selectedPin = null;
                }
                else
                {
                    _selectedPin = value;
                    UpdatePin(value);
                }                
                OnPropertyChanged(SelectedPin);
            }
        }

        private string _minChubbNumber = "0";

        public string MinChubbNumber
        {
            get => _minChubbNumber ?? string.Empty;
            set
            {
                _minChubbNumber = value;
                OnPropertyChanged(MinChubbNumber);
            }
        }
        
        public string? ClassDepartment

        {
            get => _classDepartment ?? string.Empty;
            set{ _classDepartment = value; OnPropertyChanged(nameof(ClassDepartment));}
        }

        private string? _classNumber;

        public string? ClassNumber
        {
            get => _classNumber ?? string.Empty;
            set { _classNumber = value; OnPropertyChanged(nameof(ClassNumber)); }
        }

        private string? _className;

        public string? ClassName
        {
            get { return _className ?? string.Empty; }
            set { _className = value; OnPropertyChanged(nameof(ClassName)); }
        }
        public List<AuthorityGroup> AuthorityGroups { get; }

        private AuthorityGroup? _selectedAuthorityGroup;

        public AuthorityGroup? SelectedAuthorityGroup
        {
            get => _selectedAuthorityGroup ?? AuthorityGroups[0]; //default to perimeter
            set
            {
                _selectedAuthorityGroup = value;
                if (_selectedAuthorityGroup != null)
                {
                    UpdateSystemAuthority(value);
                }
                OnPropertyChanged(nameof(AuthorityGroups));
            }
        }

        private DateTime? _selectedExpiryDate;
        public DateTime? SelectedExpiry 
        { 
            get { return _selectedExpiryDate ?? DateTime.Today; } 
            set
            { 
                _selectedExpiryDate = value;
                if (_selectedPin != null)
                {
                    UpdateExpiryDate(value);
                }
                OnPropertyChanged(nameof(SelectedExpiry)); 
            }
        }

        private ClassListModel ClassModel;

        public ICommand SelectClassFileCommand { get; }

        public ICommand UpdateUserNumbersCommand { get; }

        public ICommand ImportClassFileCommand { get; } 

        public Dictionary<string, bool> _allViews;

        public Dictionary<string, bool> AllViews
        {
            get
            {
                return _allViews;
            }
            set
            {
                _allViews = value;
                OnPropertyChanged(nameof(AllViews));
            }
        }
        
        public ClassHubViewModel(UserReportModel userReportModel, NavigationStore navigationStore) : base(userReportModel, navigationStore)
        {
            SelectClassFileCommand = new SelectClassFileCommand(SelectNewClassFile);
            ClassModel = new ClassListModel(base._userReportModel);
            AllViews = new Dictionary<string, bool>() { { "All Users", true }, { "Matched", false }, { "Unmatched", false } };
            AuthorityGroups = base._userReportModel.AuthorityGroups.Values.ToList();
            AuthorityGroups.Sort((x, y) => int.Parse(x.GroupNumber) - int.Parse(y.GroupNumber));
            UpdateUserNumbersCommand = new AddChubbNumbersCommand(UpdateChubbNumbers);
            ImportClassFileCommand = new CreateClassImportFileCommand(CreateImportFile);
            ChubbReportFile = base._userReportModel.FilePathInfo.Name;
            ClassName = ClassModel.ClassName;
            ClassDepartment = ClassModel.Department;
            ClassYear = ClassModel.ClassYear;
            ClassNumber = ClassModel.ClassNumber;
        }
        /*
         * The wrapper for the GridView is used when outputting the data. This will maintain any sorting that has been 
         * done in the view layer.ie. if the gridview has been sorted by Chubb User Number, the same order will appear 
         * in the output File.
         */
        public void CreateImportFile(bool isCSV=false)
        {
            if (this.GridViewWrapper == null)
            {
                MessageBox.Show("No Data visible for output", "ChubbHub", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = isCSV ? "CSV (*.csv)|*.csv" : "Text file (*.txt)|*txt";
            saveFileDialog.InitialDirectory = @"W:\SSTSCardAccess\Import Lists";
            saveFileDialog.FileName = $"Import {ClassDepartment}-{ClassName}";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                FileImporter FI = new(fileName);
                foreach (ChubbUser usr in this.GridViewWrapper)
                {
                    FI.AddUser(usr);
                }
                if (isCSV)
                {
                    FI.ExportTableToCSV(fileName);
                }
                else
                {
                    FI.ExportTableToImportFile(fileName);
                }
            }
        }
        

        public void SelectNewClassFile()
        {
            FileInfo? ClassFileInfo = FileModel.SelectFile("RCL Class File", ".rcl", "Registrar Class List (.rcl)|*.rcl", "", @"W:\SSTSCardAccess\Classes");
            if (ClassFileInfo != null)
            {
                this.ClassFileName = ClassFileInfo.Name;
                //Now use model to parse the selected file.
                ClassModel.NewRCLFile(ClassFileInfo.FullName);
                this.ClassName = ClassModel.ClassName;
                this.ClassDepartment = ClassModel.Department;
                this.ClassNumber = ClassModel.ClassNumber;
                this.ClassYear = ClassModel.ClassYear;
                this.GridView = new ObservableCollection<ChubbUser>(ClassModel.AllUsers.Values.ToList());
                this.SelectedView = "All Users";
            }
        }

        private void ChangeView(string value)
        {
            if (ClassModel == null) return;
            if(value =="Matched")
            {
                this.GridView = new ObservableCollection<ChubbUser>(ClassModel.Matched.Values.ToList());
                this.AllViews["Matched"] = true;
                this.AllViews["Unmatched"] = false;
                this.AllViews["All Users"] = false;
                //this.SelectedView = "Matched";
                OnPropertyChanged(nameof(AllViews));
            }
            else if (value == "Unmatched")
            {
                this.GridView = new ObservableCollection<ChubbUser>(ClassModel.Unmatched.Values.ToList());
                this.AllViews["Matched"] = false;
                this.AllViews["Unmatched"] = true;
                this.AllViews["All Users"] = false;
                //this.SelectedView = "Unatched";
                OnPropertyChanged(nameof(AllViews));
            }
            else
            {
                this.GridView = new ObservableCollection<ChubbUser>(ClassModel.AllUsers.Values.ToList());
                this.AllViews["Matched"] = false;
                this.AllViews["Unmatched"] = false;
                this.AllViews["All Users"] = true;
                //this.SelectedView = "All Users";
                OnPropertyChanged(nameof(AllViews));
            }
            SelectedAuthorityGroup = null;
            SelectedPin = null;
            SelectedExpiry = null;
        }


        #region View Updating Functions

        /*
         * Updater functions will act on whatever current view is visible.  
         */
        private void UpdateExpiryDate(DateTime? newDate)
        {
            if (this.GridViewWrapper == null) return;
            //setting the string datetime
            foreach (ChubbUser usr in this.GridViewWrapper)
            {
                usr.Expiry = newDate;
            }
        }
        public void UpdateChubbNumbers()
        {
            if (this.GridViewWrapper == null) return;
            base._userReportModel.ResetChubbNumberGenerator();
            foreach (ChubbUser usr in this.GridViewWrapper)
            {
                usr.ChubbNumber = base._userReportModel.TakeNextFreeChubbUserNumber(this.MinChubbNumber);
            }
        }
        private void UpdatePin(string newPin)
        {
            if (this.GridViewWrapper == null) return;
            //setting the string datetime
            foreach (ChubbUser usr in this.GridViewWrapper)
            {
                usr.Pin = newPin;
            }
        }
        private void UpdateSystemAuthority(AuthorityGroup newAuthority)
        {
            if (this.GridViewWrapper == null) return;
            foreach (ChubbUser usr in this.GridViewWrapper)
            {
                usr.AuthorityNumber = newAuthority.GroupNumber;
                usr.SystemAuthority = newAuthority.GroupName;
            }
        }
        #endregion
    }
}
