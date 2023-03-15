using ChubbHubMVVM.Commands;
using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChubbHubMVVM.ViewModels
{
    /*
     * This class serves the bindings from within the UserReport view. This also serves as the landing page logic
     */
    internal class ChubbReportViewModel: ViewModelBase
    {
        
        private string? _file; //path to file

        public string? FileName
        {
            get
            {
                if (_file == null) return string.Empty;
                return _file;
            }
            set
            {
                _file = value;
                OnPropertyChanged(nameof(FileName));
            }
        }
        public ICommand SelectFileCommand { get; }

        private List<AuthorityGroup> _chubbGroups;

        private AuthorityGroup _selectedAuthorityGroup;

        public AuthorityGroup SelectedAuthorityGroup
        {
            get { return _selectedAuthorityGroup; }
            set
            {
                _selectedAuthorityGroup = value;
                OnPropertyChanged(nameof(SelectedAuthorityGroup));
            }
        }

        public List<AuthorityGroup> ChubbGroups
        { 
            get { return _chubbGroups; }
            set { _chubbGroups = value; OnPropertyChanged(nameof(ChubbGroups)); }
        }

        public int _userCount { get; set; }

        public int _groupCount { get; set; }

        public int _departmentCount { get; set; }

        public int _expiredCount { get; set; }
        public int? UserCount
        {
            get => _userCount;
            set
            {
                _userCount = value ?? 0;
                OnPropertyChanged(nameof(UserCount));
            }
        }

        public int? GroupCount
        {
            get => _groupCount;
            set
            {
                _groupCount = value?? 0;
                OnPropertyChanged(nameof(GroupCount));
            }
        }

        public int? DepartmentCount
        {
            get => _departmentCount;
            set
            {
                _departmentCount = value ??0;
                OnPropertyChanged(nameof(DepartmentCount));
            }
        }

        public int? ExpiredCount
        {
            get => _expiredCount;
            set
            {
                _expiredCount = value ?? 0;
                OnPropertyChanged(nameof(ExpiredCount));
            }
        }

        public ChubbReportViewModel(UserReportModel userReportmodel, NavigationStore navigationStore) :base(userReportmodel, navigationStore)
        {
            //SelectFileCommand = new AddUsersCommand(base._userReportModel);
            FileName = base._userReportModel?.FilePathInfo.Name;
            ChubbGroups = base._userReportModel?.AuthorityGroups.Values.ToList();
            UserCount = base._userReportModel?.ChubbUsers.Count;
            GroupCount = base._userReportModel?.AuthorityGroups.Count;
            DepartmentCount = base._userReportModel?.DepartmentGroups.Count;
            ExpiredCount = base._userReportModel?.ExpiredUsers.Count;
            SelectFileCommand = new NewReportCommand(SelectNewReport);
        }



        public void SelectNewReport()
        {
            //select file dialog

            FileInfo? newChubbFile = FileModel.SelectFile("All User Report", ".txt", "Text documents (.txt)|*.txt", "", @"W:\SSTSCardAccess\Reports");
            if (newChubbFile != null)
            {

                base._userReportModel.Reinitialize(newChubbFile.FullName);
                //this.FacultyFileName = newChubbFile.FullName;
                //FacultyListModel = new FacultyListModel(base._userReportModel, FacultyFileInfo.FullName);
                //GridView = new ObservableCollection<FacultyListUser>(FacultyListModel.Users.Values);
                //return true;
                FileName = base._userReportModel.FilePathInfo.Name;
                ChubbGroups = base._userReportModel.AuthorityGroups.Values.ToList();
                UserCount = base._userReportModel.ChubbUsers.Count;
                GroupCount = base._userReportModel.AuthorityGroups.Count;
                DepartmentCount = base._userReportModel.DepartmentGroups.Count;
                ExpiredCount = base._userReportModel.ExpiredUsers.Count;
            }

        }
    }
}
