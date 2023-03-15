using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using ChubbHubMVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChubbHubMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //App has global navigation, notifications and userreport
        private readonly UserReportModel _chubbUserReport;
        private readonly NavigationStore _navigationStore;
        private readonly Notifications _notifications;
        

        public App()
        {
            try
            {
                _navigationStore = new NavigationStore();
                _notifications = new Notifications();
                _chubbUserReport = new UserReportModel(_notifications);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = new ChubbReportViewModel(_chubbUserReport, _navigationStore);
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_chubbUserReport, _navigationStore)
            };

            //We set the navigation to be the UserReport
            
            MainWindow.Show();
            base.OnStartup(e);
        }

    }
}
