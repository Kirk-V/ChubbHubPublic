using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Windows.Input;
using ChubbHubMVVM.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using System.Reflection;
using SixLabors.ImageSharp.ColorSpaces;
using File = System.IO.File;

namespace ChubbHubMVVM.ViewModels
{
    public class FacultyHubViewModel : ViewModelBase
    {
        /*
         * The obsevable collection View serves as the binding point for the datagrid control and
         * it gets updated to hold the 'view' that the user desires. This can be one of {all users, unmatched, matched}
         * depending on what the user has selected in the view dropdown.
         */

        private ObservableCollection<FacultyListUser> _gridView;

        public ObservableCollection<FacultyListUser> GridView
        {
            get { return _gridView; }
            set
            {
                _gridView = value;
                this.GridViewWrapper = CollectionViewSource.GetDefaultView(_gridView);
                OnPropertyChanged(nameof(GridView));
            }
        }

        private ICollectionView _gridViewWrapper;

        public ICollectionView GridViewWrapper
        {
            get { return _gridViewWrapper; }
            set 
            { 
                _gridViewWrapper = value;
                
                OnPropertyChanged(nameof(GridViewWrapper));
                ResetSettings();
            }
        }

        private void ResetSettings()
        {
            _minChubbNumber = "0";
            OnPropertyChanged(nameof(MinChubbNumber));
            _selectedAuthorityGroup = null;
            OnPropertyChanged(nameof(SelectedAuthorityGroup));
            _selectedPin = "0";
            OnPropertyChanged(nameof(SelectedPin));
            _selectedExpiry = DateTime.Now;
            OnPropertyChanged(nameof(SelectedExpiry));
            //SelectedFilter = FilterTypes.none;
        }

        private string _selectedView;

        public string SelectedView
        {
            get
            {
                return this._selectedView;
            }
            set
            {
                ChangeView(value);
                this._selectedView = value;
                OnPropertyChanged(nameof(SelectedView));
                //this.FacultyListModel.ChangeView(value);
                OnPropertyChanged(nameof(_gridView));
                ApplyFilter(SelectedFilter);
                ResetSettings();
            }
        }

        public string ChubbReportFile { get;}

        private string _minChubbNumber = "0";

        public string MinChubbNumber
        {
            get => _minChubbNumber;
            set
            {
                _minChubbNumber = value;
                OnPropertyChanged(nameof(MinChubbNumber));
            }
        }

        public ICommand AddChubbNumbers { get; }

        private Dictionary<string, bool> _viewComboSelection;

        public Dictionary<string, bool> ViewComboSelection
        {
            get { return _viewComboSelection; }
            set {
                _viewComboSelection = value;
                OnPropertyChanged(nameof(ViewComboSelection));
            }
        }

        private string _matchedSettingsVisibility;
        public string MatchedSettingsVisibility 
        {
            get => _matchedSettingsVisibility; 
            set
            {
                _matchedSettingsVisibility = value;
                OnPropertyChanged(nameof(MatchedSettingsVisibility));
            }
        }
        
        private string _unmatchedSettingsVisibility;
        public string UnmatchedSettingsVisibility
        {
            get => _unmatchedSettingsVisibility;
            set
            {
                _unmatchedSettingsVisibility = value;
                OnPropertyChanged(nameof(UnmatchedSettingsVisibility));
            }
        }

        private string _allUsersSettingsVisibility;

        public string AllUsersSettingsVisibility
        {
            get => _allUsersSettingsVisibility;
            set
            {
                _allUsersSettingsVisibility = value;
                OnPropertyChanged(nameof(AllUsersSettingsVisibility));
            }
        }

        public List<string> ComboViewItems { get; set; }
        public CollectionView GridData { get; set; }

        private AuthorityGroup? _selectedAuthorityGroup;

        public AuthorityGroup? SelectedAuthorityGroup
        {
            get { return _selectedAuthorityGroup; }
            set
            {
                _selectedAuthorityGroup = value;
                if(value != null)
                {
                    this.UpdateAuthorityGroup(value);

                }
                OnPropertyChanged(nameof(SelectedAuthorityGroup));
            }
        }

        private DateTime? _selectedExpiry;
        public DateTime? SelectedExpiry
        {
            get { return this._selectedExpiry ?? DateTime.Today; }
            set
            {
                if(value != null)
                {
                    _selectedExpiry = value;
                    UpdateExpiry(value);
                    OnPropertyChanged(nameof(SelectedExpiry));
                }
                
            }
        }

        private string _selectedPin = "0";

        public string SelectedPin 
        {
            get { return this._selectedPin; }
            set
            {
                this._selectedPin = value;
                UpdatePin(value);
                OnPropertyChanged(nameof(SelectedPin));
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

        private FileInfo? _facultyFileInfo;

        public ICommand SelectFacultyFileCommand { get; }

        public string FacultyFileName
        {
            get
            {
                return _facultyFileInfo == null ? string.Empty : _facultyFileInfo.Name;
            }

            set 
            {
                if (value != null)
                {
                    FacultyListModel.ListFile = value;
                    _facultyFileInfo = new FileInfo(value);
                    OnPropertyChanged(nameof(FacultyFileName));
                }
                 
            }
        }

        public FacultyListModel FacultyListModel{ get; set; }

        public enum FilterTypes
        {
            none,
            Students,
            Staff,
        }

        private FilterTypes? _selectedFilter;

        public List<String> Filters
        {
            get => Enum.GetNames(typeof(FilterTypes)).ToList();
        }
        public FilterTypes SelectedFilter
        {
            get =>  _selectedFilter ?? 0;
            set
            {
                _selectedFilter = value;
                ApplyFilter(value);
                ResetSettings();
            }
        }
        public ICommand CreateFacultyFileCommand { get; }

        public ICommand CreateCSVFileCommand { get; }

        public ICommand ExportMatchedCSVCommand { get; }

        public ICommand CreateMatchedUnmatched { get; }

        private bool _madeMatchedAndUnmatched = false;
        public bool MadeMatchedAndUnmatched 
        {
            get => _madeMatchedAndUnmatched; 
            set
            {
                _madeMatchedAndUnmatched = value;
                OnPropertyChanged(nameof(MadeMatchedAndUnmatched));
            }
        }

        /*
         * To instantiate the viewmodel, we create an instance of the FacultyList model. Using this object we populate the variables
         * necessary to display to the user. FacultyFileName - For the file selection, AuthorityGroups - for the drop downs,
         * SelectedFacultyFileCommand responds to the user changing the faculty file. GridView - holds the current view of the datagrid (allusers, matched, unmatched)
         * ViewComboSelection is hardcoded as a dictionary representing each of the possible views and if they are currently displayed. 
         * This dictionary also helps to dynamically show the appropriate settings for a view. For example, we don't edit the all users, but we
         * can edit the unmatched users (to update their info). 
         */
        public FacultyHubViewModel(UserReportModel userReportModel, NavigationStore navigationStore) : base(userReportModel, navigationStore)
        {
            FacultyListModel = new FacultyListModel(base._userReportModel);
            FacultyFileName = FacultyListModel.ListFile;
            AuthorityGroups = base._userReportModel.AuthorityGroups.Values.ToList();
            SelectFacultyFileCommand = new SelectFacultyFileCommand(this);
            GridView = new ObservableCollection<FacultyListUser>(FacultyListModel.Users.Values);
            ViewComboSelection = new Dictionary<string, bool>() {{"All Users", true }, {"Matched", false },{"Unmatched", false } };
            ComboViewItems = ViewComboSelection.Keys.ToList();
            SelectedView = "All Users";
            AddChubbNumbers = new AddChubbNumbersCommand(UpdateChubbNumbers);
            CreateFacultyFileCommand = new CreateFacultyImportFileCommand(OutputList);
            UnmatchedSettingsVisibility = "Collapsed";
            MatchedSettingsVisibility = "Collapsed";
            ExportMatchedCSVCommand = new ExportMatchedCommand(ExportMatchedCSV);
            ChubbReportFile = base._userReportModel.FilePathInfo.Name;
            CreateMatchedUnmatched = new CallBackCommand(CreateMatchedUnmatchedFiles);
        }

        private void ExportMatchedCSV()
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "CSV (*.csv)|*.csv";
            saveFileDialog.InitialDirectory = @"W:\SSTSCardAccess\ChubbHub";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                FileImporter FI = new(fileName, true);
                foreach (FacultyListUser usr in this.GridViewWrapper)
                {
                    //convert to chubb user and add to export file
                    FI.AddMatchedUser(usr.ChubbNumber, usr.FirstName, usr.LastName, usr.Pin, usr.SystemAuthority, usr.AuthorityNumber, usr.AuthorityExpiryAsString(), usr.SystemAuthorityPlus, usr.AuthorityPlusNumber, usr.AuthorityPlusExpiryAsString());
                }
                FI.ExportTableToCSV(fileName);
            }
        }

        public void ApplyFilter(FilterTypes filter)
        {
            switch (filter)
            {
                case FilterTypes.none:
                    GridViewWrapper.Filter = new Predicate<object>(FilterShowAllUsers);
                    break;

                case FilterTypes.Students:
                    GridViewWrapper.Filter = new Predicate<object>(FilterShowStudents);
                    break;

                case FilterTypes.Staff:
                    GridViewWrapper.Filter = new Predicate<object>(FilterShowFacyltyStaff);
                    break;
                default:
                    GridViewWrapper.Filter = new Predicate<object>(FilterShowAllUsers);
                    break;

            }
        }


        public bool FilterShowAllUsers(object usrObj)
        {
            return true;
        }

        public bool FilterShowFacyltyStaff(object usrObj)
        {
            FacultyListUser? usr = usrObj as FacultyListUser;
            if (usr == null) return false;
            return !FilterShowStudents(usrObj);
        }

        public bool FilterShowStudents(object usrObj)
        {
            FacultyListUser? usr = usrObj as FacultyListUser;
            if (usr == null) return false;
            return usr.Type.ToLower() == "s";
        }
        private Predicate<object> CollectionViewSource_Filter()
        {
            throw new NotImplementedException();
        }

        public void UpdateChubbNumbers()
        {
            string MinNumber = this.MinChubbNumber;
            base._userReportModel.ResetChubbNumberGenerator();
            foreach (FacultyListUser usr in this.GridView)
            {
                usr.ChubbNumber = base._userReportModel.TakeNextFreeChubbUserNumber(MinNumber);
            }
        }

        public void FacultyHubView_SelectionChanged()
        {

        }
        /*
         * This function will start the process of loading a new faculty file for the view to display.
         * First, the file is selected by the user, this updates the ListFile property of the FacultyListModel object
         * through the setter on this objects property 'FacultyFileName'. 
         */
        public bool NewFacultyFile()
        {
            FileInfo? FacultyFileInfo = FileModel.SelectFile("Faculty File", ".txt", "Text documents (.txt)|*.txt", "", @"\\juno.ssc.uwo.ca\snap$\Batch\Logs");
            if(FacultyFileInfo != null)
            {
                //First copy the file contents 
                string CopyFile = $@"{FacultyFileInfo.DirectoryName}\NewFacultyStaff-{DateTime.Now.ToString("MMM")}{DateTime.Now.Day}-{DateTime.Now.Year}.txt";
                int i = 0;
                while (File.Exists(CopyFile))
                {
                    i++;
                    base._userReportModel.NotificationList.Messages.Add($"Faculty File already copied to {CopyFile}, Appending {i.ToString()} to name.");
                    CopyFile = $"{CopyFile}{i.ToString()}";
                }
                File.Copy(FacultyFileInfo.FullName, CopyFile);

                this.FacultyFileName = CopyFile;
                FacultyListModel = new FacultyListModel(base._userReportModel, FacultyFileInfo.FullName);
                GridView = new ObservableCollection<FacultyListUser>(FacultyListModel.Users.Values);
                return true;
            }
            else { return false; }

            //string FileName = FileModel.SelectFile("Faculty File", ".txt", "Text documents (.txt)|*.txt", "W:\\SSTSCardAccess\\Reports", "W:\\SSTSCardAccess\\Reports");
            //FileModel.SelectFile("Faculty File", ".txt", "Text documents (.txt)|*.txt", "W:\\SSTSCardAccess\\Reports", "W:\\SSTSCardAccess\\Reports");
        }

        public void CreateMatchedUnmatchedFiles()
        {
            string term = "";
            if(DateTime.Now.Month <= 4)
            {
                term = "Winter-Jan-Apr";
            }
            else if(DateTime.Now.Month <=8)
            {
                term = "Summer-May-Aug";
            }
            else
            {
                term = "Fall-Sept-Dec";
            }
            string saveFolder = $@"W:/SSTSCardAccess/Matched & Unmatched files";
            string saveFileMatched = $@"{saveFolder}/NewFacStaff-{DateTime.Now.Day}{DateTime.Now.ToString("MMM")}{DateTime.Now.Year}-Matched.csv";
            string saveFileUnmatched = $@"{saveFolder}/NewFacStaff-{DateTime.Now.Day}{DateTime.Now.ToString("MMM")}{DateTime.Now.Year}-Unmatched.csv";
            if (!Directory.Exists(saveFolder)) { Directory.CreateDirectory(saveFolder); }
            FileImporter FI = new FileImporter(saveFileMatched);
            foreach (FacultyListUser usr in FacultyListModel.MatchedUsers.Values)
            {
                FI.AddUser(usr.GetUserAsChubbUser(), usr.AuthorityNumber);
            }
            FI.ExportTableToCSV(saveFileMatched);
            FI = new FileImporter(saveFileUnmatched);
            foreach (FacultyListUser usr in FacultyListModel.UnmatchedUsers.Values)
            {
                FI.AddUser(usr.GetUserAsChubbUser(), usr.AuthorityNumber);
            }
            MadeMatchedAndUnmatched = FI.ExportTableToCSV(saveFileUnmatched);
        }

        public void ChangeView(string ViewType)
        {
            if (ViewType == "Matched")
            {
                MatchedSettingsVisibility = "Visible";
                UnmatchedSettingsVisibility = "Collapsed";
                AllUsersSettingsVisibility = "Collapsed";
                ViewComboSelection["Matched"] = true;
                ViewComboSelection["Unmatched"] = false;
                ViewComboSelection["All Users"] = false;
                this._gridView.Clear();
                GridView = new ObservableCollection<FacultyListUser>(FacultyListModel.MatchedUsers.Values);

            }
            else if (ViewType == "Unmatched")
            {
                MatchedSettingsVisibility = "Collapsed";
                UnmatchedSettingsVisibility = "Visible";
                AllUsersSettingsVisibility = "Collapsed";
                ViewComboSelection["Matched"] = false;
                ViewComboSelection["Unmatched"] = true;
                ViewComboSelection["All Users"] = false;
                this._gridView.Clear();
                GridView = new ObservableCollection<FacultyListUser>(FacultyListModel.UnmatchedUsers.Values);
            }
            else
            {
                MatchedSettingsVisibility = "Collapsed";
                UnmatchedSettingsVisibility = "Collapsed";
                AllUsersSettingsVisibility = "Visible";
                ViewComboSelection["Matched"] = false;
                ViewComboSelection["Unmatched"] = false;
                ViewComboSelection["true"] = false;
                this._gridView.Clear();
                GridView = new ObservableCollection<FacultyListUser>(FacultyListModel.Users.Values);
            }
            ViewComboSelection = ViewComboSelection;
        }

        /*
         * Function to update the authority group name and number for every user selected in the current view.
         */
        private void UpdateAuthorityGroup(AuthorityGroup newAuthortityGroup)
        {
            foreach (FacultyListUser usr in this.GridViewWrapper)
            {
                usr.AuthorityNumber = newAuthortityGroup.GroupNumber;
                usr.SystemAuthority = newAuthortityGroup.GroupName;
            }
        }

        private void UpdatePin(string newPin)
        {
            if(newPin == null) { return; }
            foreach(FacultyListUser usr in this.GridViewWrapper)
            {
                usr.Pin = newPin;
            }
        }
        private void UpdateExpiry(DateTime? newExpiry)
        {
            if(newExpiry == null) { return; }
            foreach (FacultyListUser usr in this.GridViewWrapper)
            {
                usr.Expiry = (DateTime)newExpiry;
            }
        }


        /*
         * Output the current datagrid view in its form to a file. This can be either a CSV or ChubbDirector 
         * Import File.
         */
        public void OutputList(bool csv=false)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            string directoryPath = $@"W:\SSTSCardAccess\Import Lists\{year}-import";
            string saveDirectory = Directory.CreateDirectory(directoryPath).FullName;
            saveFileDialog.Filter = csv ? "CSV (*.csv)|*.csv" : "Text file (*.txt)|*txt";
            saveFileDialog.InitialDirectory = saveDirectory;
            saveFileDialog.FileName = csv? $"Import New FacStaff-{day}{DateTime.Now.ToString("MMM")}{year}.csv": $"Import New FacStaff-{day}{DateTime.Now.ToString("MMM")}{year}.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                FileImporter FI = new(fileName);
                foreach (FacultyListUser usr in this.GridViewWrapper)
                {
                    //convert to chubb user and add to export file
                    ChubbUser chubbUser = new(usr.FirstName, usr.LastName, usr.WesternId, usr.ChubbNumber, usr.Expiry, usr.SystemAuthority, usr.Pin, usr.SystemAuthorityPlus ?? null, usr.ExpiryPlus ?? null);
                    //ChubbUserModel chubbUser = new(usr.FirstName, usr.LastName, usr.WesternId, usr.ChubbNumber, usr.AuthorityExpiryAsString(), usr.SystemAuthority, usr.Pin, usr.SystemAuthorityPlus ?? null);
                    FI.AddUser(chubbUser, usr.AuthorityNumber);
                }
                if (csv)
                {
                    FI.ExportTableToCSV(fileName);
                }
                else
                {
                    FI.ExportTableToImportFile(fileName);
                }
            }
            
        }



    }
}
