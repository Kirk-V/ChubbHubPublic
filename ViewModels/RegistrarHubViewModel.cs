using ChubbHubMVVM.Commands;
using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static ChubbHubMVVM.ViewModels.FacultyHubViewModel;

namespace ChubbHubMVVM.ViewModels
{
    public class RegistrarHubViewModel : ViewModelBase
    {
        private RegistrarList RegistrarList;

        private ObservableCollection<RegistrarUser>? _gridView;

        public ObservableCollection<RegistrarUser>? GridView
        {
            get { return _gridView; }
            set
            {
                _gridView = value;
                this.GridViewWrapper = CollectionViewSource.GetDefaultView(_gridView);
                OnPropertyChanged(nameof(GridView));
            }
        }

        private List<AuthorityGroup>? _authorityGroups;
        public List<AuthorityGroup> AuthorityGroups
        {
            get
            {
                return _authorityGroups;
            }
            set
            {
                _authorityGroups = value;
                _authorityGroups.Sort((x, y) => int.Parse(x.GroupNumber) - int.Parse(y.GroupNumber));
                OnPropertyChanged(nameof(AuthorityGroups));
            }
        }
        
        private ICollectionView _gridViewWrapper;

        public ICollectionView GridViewWrapper
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
        public Dictionary<string, bool> AllViews { get; }
        
        private string? _registrarFileName;

        public string RegistrarFileName
        {
            get
            {
                if(_registrarFilePath == null) { return string.Empty; }
                if (_registrarFilePath != string.Empty)
                {
                    return new FileInfo(_registrarFilePath).Name;
                }
                else return string.Empty;
            }
        }
        private string? _registrarFilePath;

        public string RegistrarFilePath
        {
            get => _registrarFilePath ?? string.Empty;
            set
            {
                _registrarFilePath = value;
                OnPropertyChanged(nameof(RegistrarFilePath));
                OnPropertyChanged(nameof(RegistrarFileName));
            }
        }
        //public List<AuthorityGroup> AuthorityGroups { get; set; }

        private AuthorityGroup? _selectedAuthorityGroup;
        public AuthorityGroup? SelectedAuthorityGroup
        {
            get { return _selectedAuthorityGroup; }
            set
            {
                _selectedAuthorityGroup = value;
                if (value != null)
                {
                    this.UpdateAuthorityGroup(value);

                }
                else
                {
                    //default to null if improper value type is provided/also serves to reset the shown auth group date.
                    _selectedAuthorityGroup = null;
                }
                OnPropertyChanged(nameof(SelectedAuthorityGroup));
            }
        }

        private DateTime? _selectedExpiry;
        public DateTime? SelectedExpiry
        {
            get { return this._selectedExpiry; }
            set
            {
                //We want nullable values to be allowed here incase we need to reset the UI to show nothing selected (ie. when a filter changes)
                if (value is DateTime)
                {
                    _selectedExpiry = value;
                    UpdateExpiry((DateTime)value);
                }
                else
                {
                    //default to null if improper value type is provided/also serves to reset the shown expirty date.
                    _selectedExpiry = null;
                    OnPropertyChanged(nameof(SelectedExpiry));
                }
                
                
            }
        }

        private string? _selectedPin;

        public string? SelectedPin
        {
            get { return this._selectedPin; }
            set
            {
                if(value is string)
                {
                    this._selectedPin = value;
                    UpdatePin(value);
                }
                else
                {
                    this._selectedPin = null;
                }
                OnPropertyChanged(nameof(SelectedPin));
                
            }
        }
        public ICommand SelectRegistrarFileCommand { get; }

        public ICommand UpdateUserNumbersCommand { get; }
        public string MinChubbNumber { get; set; }

        private List<string> _acadGroups;
        public List<string> AcadGroups
        {
            get => _acadGroups;
            set
            {
                _acadGroups = value;
                OnPropertyChanged(nameof(AcadGroups));
            }
        }

        private string _selectedAcadGroup;

        public string SelectedAcadGroup
        {
            get => _selectedAcadGroup;
            set
            {
                _selectedAcadGroup = value;
                if(value != null)
                {
                    UpdateGridViewWithFilters();
                }
                
            }
        }


        public List<string> _primaryPrograms;

        public List<string> PrimaryPrograms 
        {
            get => _primaryPrograms;
            set
            {
                _primaryPrograms = value;
                OnPropertyChanged(nameof(PrimaryPrograms));
            }
        }

        private string _selectedPrimaryProgram;

        public string SelectedPrimaryProgram
        {
            get => _selectedPrimaryProgram;
            set
            {
                _selectedPrimaryProgram = value;
                if (value != null)
                {
                    UpdateGridViewWithFilters();
                }
            }
        }

        private Dictionary<string, bool> _yearsFilter;

        public Dictionary<string, bool> YearsFilter
        {
            get => _yearsFilter;
            set
            {
                _yearsFilter = value;
                OnPropertyChanged(nameof(YearsFilter));
                UpdateGridViewWithFilters();
            }
        }


        private List<string> _startLevels;
        public List<string> StartLevels
        {
            get => _startLevels;
            set
            {
                _startLevels = value;
                OnPropertyChanged(nameof(StartLevels));
            }
        }

        //private string _selectedStartValue;

        //public string SelectedStartLevel
        //{
        //    get => _selectedStartValue;
        //    set
        //    {
        //        _selectedStartValue = value;
        //        if (value != null)
        //        {
        //            UpdateGridViewWithFilters();
        //        }
        //    }
        //}
        public ICommand ToggleYear { get; }
        public ICommand ImportRegistrarFileCommand { get; }

        //use this list as the currently selected filters for years.
        public List<string> Years { get; set; }

        public RegistrarHubViewModel(UserReportModel userReportModel, NavigationStore navigationStore ) :base(userReportModel, navigationStore)
        {
            RegistrarList = new RegistrarList(base._userReportModel);
            RegistrarFilePath = RegistrarList.RegistrarFile;

            GridView = new ObservableCollection<RegistrarUser>(RegistrarList.AllUsers.Values.ToList());
            SelectRegistrarFileCommand = new SelectClassFileCommand(SelectNewRegistrarFile);
            AllViews = new Dictionary<string, bool>() { { "All Users", false }, { "Matched", false }, { "Unmatched", false } };
            AuthorityGroups = base._userReportModel.AuthorityGroups.Values.ToList();
            AuthorityGroups.Sort((x, y) => int.Parse(x.GroupNumber) - int.Parse(y.GroupNumber));
            UpdateUserNumbersCommand = new AddChubbNumbersCommand(UpdateChubbNumbers);
            MinChubbNumber = "0";
            AcadGroups = RegistrarList.AcadGroups.Keys.ToList();
            AcadGroups.Insert(0, "ALL");
            PrimaryPrograms = RegistrarList.PrimaryProgarms.Keys.ToList();
            PrimaryPrograms.Insert(0, "ALL");
            StartLevels = RegistrarList.StartLevels.Keys.ToList();
            StartLevels.Insert(0, "ALL");
            _selectedAcadGroup = _selectedPrimaryProgram = "ALL";
            YearsFilter = StartLevels.ToDictionary(x => x, x=>(x=="ALL") ? true:false);
            
            ImportRegistrarFileCommand = new CreateClassImportFileCommand(CreateImportFile);
            ChubbReportFile = base._userReportModel.FilePathInfo.Name;
            ToggleYear = new ToggleYearFilter(ToggleYearFilter);
        }

        public void ToggleYearFilter(string year)
        {
            if(year == "ALL")
            {
                foreach (KeyValuePair<string, bool> entry in YearsFilter)
                {
                    if (entry.Key == "ALL")
                    {
                        YearsFilter["ALL"] = !YearsFilter["ALL"];
                    }
                    else
                    {
                        YearsFilter[entry.Key] = false;
                    }
                }
            }
            else
            {
                YearsFilter[year] = !YearsFilter[year];
                foreach(KeyValuePair<string, bool> entry in YearsFilter)
                {
                    if(entry.Value == true)
                    {
                        if(entry.Key!="All")
                        {
                            //we have at least one year selected (that is not all) so remove All from filter
                            YearsFilter["ALL"] = false;
                        }
                    }
                }
            }
            
            YearsFilter = new Dictionary<string, bool>(YearsFilter);
            //OnPropertyChanged(nameof(YearsFilter));
        }

        /*
         * The wrapper for the GridView is used when outputting the data. This will maintain any sorting that has been 
         * done in the view layer.ie. if the gridview has been sorted by Chubb User Number, the same order will appear 
         * in the output File.
         */
        public void CreateImportFile(bool isCSV = false)
        {
            if (this.GridViewWrapper == null)
            {
                MessageBox.Show("No Data visible for output", "ChubbHub", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = isCSV ? "CSV (*.csv)|*.csv" : "Text file (*.txt)|*txt";
            saveFileDialog.InitialDirectory = @"W:\SSTSCardAccess\Import Lists";
            saveFileDialog.FileName = $"{DateTime.Now.ToString("ddMMMyyyy")}";
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
        public void SelectNewRegistrarFile()
        {
            FileInfo? RegistrarFileInfo = FileModel.SelectFile("Registrar File", ".xls", "Microsoft Excel 97-2003 Worksheet (.xls)|*.xls", "", @"W:\SSTSCardAccess\Registrar lists");
            if (RegistrarFileInfo != null)
            {
                //First copy the file contents 
                if(RegistrarList.InitializeRegistrarList(false, RegistrarFileInfo.FullName))
                {
                    RegistrarFilePath = RegistrarList.RegistrarFile;
                    GridView = new ObservableCollection<RegistrarUser>(RegistrarList.AllUsers.Values.ToList());
                    
                    AcadGroups.Clear();
                    PrimaryPrograms.Clear();
                    StartLevels.Clear();
                    AcadGroups = RegistrarList.AcadGroups.Keys.ToList();
                    AcadGroups.Insert(0, "ALL");
                    PrimaryPrograms = RegistrarList.PrimaryProgarms.Keys.ToList();
                    PrimaryPrograms.Insert(0, "ALL");
                    StartLevels = RegistrarList.StartLevels.Keys.ToList();
                    StartLevels.Insert(0, "ALL");
                    _selectedAcadGroup = _selectedPrimaryProgram = "ALL";
                    YearsFilter = StartLevels.ToDictionary(x => x, x => false);
                    ChangeView("All Users");
                }
            }
        }

        public void AddRegistrarFile()
        {
            //opens a dialog box to select a new file
        }

        private void ChangeView(string value)
        {
            if (RegistrarList == null) return;
            if (value == "Matched")
            {
                this.GridView = new ObservableCollection<RegistrarUser>(RegistrarList.Matched.Values.ToList());
                this.AllViews["Matched"] = true;
                this.AllViews["Unmatched"] = false;
                this.AllViews["All Users"] = false;
                OnPropertyChanged(nameof(AllViews));
            }
            else if (value == "Unmatched")
            {
                this.GridView = new ObservableCollection<RegistrarUser>(RegistrarList.Unmatched.Values.ToList());
                this.AllViews["Matched"] = false;
                this.AllViews["Unmatched"] = true;
                this.AllViews["All Users"] = false;
                OnPropertyChanged(nameof(AllViews));
            }
            else
            {
                this.GridView = new ObservableCollection<RegistrarUser>(RegistrarList.AllUsers.Values.ToList());
                this.AllViews["Matched"] = false;
                this.AllViews["Unmatched"] = false;
                this.AllViews["All Users"] = true;
                OnPropertyChanged(nameof(AllViews));
            }
        }




        #region View Updating Functions

        /*
         * Updater functions will act on whatever current view is visible.  
         */
        private void UpdateAuthorityGroup(AuthorityGroup newAuthortityGroup)
        {
            foreach (RegistrarUser usr in this.GridViewWrapper)
            {
                usr.AuthorityNumber = newAuthortityGroup.GroupNumber;
                usr.SystemAuthority = newAuthortityGroup.GroupName;
            }
        }


        private void UpdateExpiry(DateTime newDate)
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

        #endregion


        
        #region Filters

        private void UpdateGridViewWithFilters()
        {
            if (this.GridViewWrapper == null) return;
            this.GridViewWrapper.Filter = new Predicate<object>(Filter);
            SelectedExpiry = null;
            SelectedAuthorityGroup = null;
            SelectedPin = null;

        }

        //private bool MultiYearFilter(RegistrarUser usr)
        //{
        //    foreach()
        //}
        private bool Filter(object usrObj)
        {
            RegistrarUser? usr = usrObj as RegistrarUser;
            if (usr == null) return false;
            return (FilterShowAllUsers(usr) && FilterStartLevels(usr) && FilterPrimaryProgram(usr));
        }


        public bool FilterShowAllUsers(object usrObj)
        {
            return true;
        }

        public bool FilterStartLevels(RegistrarUser usr)
        {
            if (YearsFilter["ALL"]) return true;
            bool foundYear;
            if (YearsFilter.TryGetValue(usr.StartLevel ?? "", out foundYear))
            {
                return foundYear;
            }
            else
            {
                this._userReportModel.NotificationList.Messages.Add($"When filtering users for years, the user :{usr.FirstName} {usr.LastName}: {usr.WesternId} had a year that was not seen in the initial registrar file. Could not filter user");
                return false;
            }
            //return usr.StartLevel == this.SelectedStartLevel;
        }

        private bool FilterAcadGroup(RegistrarUser usr)
        {
            if (this.SelectedAcadGroup.ToLower() == "all") return true;
           
            if (usr == null) return false;
            return usr.AcadGroup == this.SelectedAcadGroup;
        }

        private bool FilterPrimaryProgram(RegistrarUser usr)
        {
            if (this.SelectedPrimaryProgram.ToLower() == "all") return true;
            if (usr == null) return false;
            else
            {
                return usr.PrimaryProgram == this.SelectedPrimaryProgram;
            }
        }
        #endregion

    }
}
