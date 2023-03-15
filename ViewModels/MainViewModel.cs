using ChubbHubMVVM.Commands;
using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChubbHubMVVM.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        //Naavigation store and UserReport objs defined in base class ViewModelBase.cs

        //CurrentViewModelWindow dictates which page we are showing (add Users, deprtment groups faculty hub etc...
        public ViewModelBase CurrentViewModelWindow => base._navigationStore.CurrentViewModel;

        public ICommand RegistrarHubNavCommand { get; }

        public ICommand ChubbReportNavCommand { get; }

        public ICommand ClassHubNavCommand { get; }

        public ICommand DepartmentGroupsNavCommand { get; }

        public ICommand FacultyHubNavCommand { get; }

        public ICommand ConfigNavCommand { get; }

        public ICommand StudentHubNavCommand { get; }

        public ICommand ShowNotificationsCommand { get; }

        public UserReportModel UserReport { get; }

        //private List<string> _messages;
        public ObservableCollection<string> Messages { get; }

        public MainViewModel(UserReportModel userReportModel, NavigationStore navigationStore) : base(userReportModel, navigationStore) 
        {
            //Instatiate all the navigation commands along the left
            //AddUsersNavCommand = new AddUsersCommand(base._userReportModel, base._navigationStore);

            UserReport = userReportModel;

            Messages = UserReport.NotificationList.Messages;

            

            ChubbReportNavCommand = new ChubbReportCommand(base._userReportModel, base._navigationStore);

            DepartmentGroupsNavCommand = new DepartmentGroupsCommand(base._userReportModel, base._navigationStore);

            FacultyHubNavCommand = new FacultyHubCommand(base._userReportModel, base._navigationStore);

            StudentHubNavCommand = new StudentHubCommand(base._userReportModel, base._navigationStore);

            ClassHubNavCommand = new ClassHubCommand(base._userReportModel, base._navigationStore);

            RegistrarHubNavCommand = new RegistrarHubCommand(base._userReportModel, base._navigationStore);

            base._navigationStore.CurrentViewModelChanged += onCurrentViewModelChanged; //subscribe onCurrentViewModelChanged() to currentViewModelChanged event

            ShowNotificationsCommand = new ShowNotificationMessagesCommand();

        }

        //When currentViewmodel window is changed this function fires. 
        //This function calls the OnPropertyChanged fn of the baseclass, which
        // will invoke the property changed event
        public void onCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModelWindow));
        }

    }
}
