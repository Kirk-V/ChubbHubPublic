using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        //event keyword means only this class can invoke the event
        // PropertyChangedEventHandler is a delegate that enforces subscribers to the event 
        // to have the (object, propertyChangedEventArgs) signature
        public event PropertyChangedEventHandler? PropertyChanged;

        public UserReportModel _userReportModel;

        public NavigationStore _navigationStore;

        protected void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ViewModelBase(UserReportModel userReportModel, NavigationStore navigationStore)
        {
            _userReportModel = userReportModel;
            _navigationStore = navigationStore;
        }   
    }
}
