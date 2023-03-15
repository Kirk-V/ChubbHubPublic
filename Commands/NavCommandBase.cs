using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using ChubbHubMVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChubbHubMVVM.Commands
{
    public abstract class NavCommandBase : ICommand
    {
        public readonly UserReportModel _reportModel;

        public readonly NavigationStore _navigationStore;
       
        public event EventHandler? CanExecuteChanged;

        public NavCommandBase(UserReportModel userReport, NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _reportModel = userReport;
        }
        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        //Implementation of Execute depends on the subclass
        public abstract void Execute(object? parameter);
    }
}
