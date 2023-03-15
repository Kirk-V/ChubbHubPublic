using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ChubbHubMVVM.ViewModels
{
    public class DepartmentHubViewModel : ViewModelBase
    {

        private List<Department> _departments;

        public List<Department> Departments
        {
            get
            {
                return _departments;
            }
            set
            {
                this._departments = value; OnPropertyChanged(nameof(Departments));
            }
        }

        private Department _selectedDepartment;

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                this._selectedDepartment = value;
                AuthorityGroups = _selectedDepartment.AuthorityGroups.Values.ToList();
                OnPropertyChanged(nameof(SelectedDepartment));
            }
        }

        private List<AuthorityGroup> _authorityGroups;
        public List<AuthorityGroup> AuthorityGroups
        {
            get => _authorityGroups;
            set
            {
                _authorityGroups = value;
                OnPropertyChanged(nameof(AuthorityGroups));
                
            }
        }

        private AuthorityGroup? _selectedAuthorityGroup;

        public AuthorityGroup? SelectedAuthorityGroup
        {
            get => _selectedAuthorityGroup;
            set
            {
                _selectedAuthorityGroup = value;
                if (value == null)
                {
                    OnPropertyChanged(nameof(SelectedAuthorityGroup));
                    GridView = null;
                    return;
                }
                GridView = new ObservableCollection<ChubbUser>(value.GroupUsers.Values.ToList());
                OnPropertyChanged(nameof(SelectedAuthorityGroup));
            }
        }

        private ObservableCollection<ChubbUser>? _gridView;

        public ObservableCollection<ChubbUser>? GridView
        {
            get => _gridView;
            set
            {
                this._gridView = value;
                this.GridViewWrapper = CollectionViewSource.GetDefaultView(_gridView);
                OnPropertyChanged(nameof(GridView));
            }
        }

        private ICollectionView? _gridViewWrapper;

        public ICollectionView? GridViewWrapper
        {
            get { return _gridViewWrapper; }
            set { 
                _gridViewWrapper = value; 
                OnPropertyChanged(nameof(GridViewWrapper)); 
            }
        }

        public DepartmentHubViewModel(UserReportModel userReportModel, NavigationStore navigationStore) : base(userReportModel, navigationStore)
        {
            base._userReportModel.BuildAllDepartments();
            Departments = base._userReportModel.Departments.Values.ToList();
            SelectedDepartment = Departments.First();
        }
    }
}
