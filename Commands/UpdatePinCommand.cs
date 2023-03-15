using ChubbHubMVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChubbHubMVVM.Commands
{
    public class UpdatePinCommand : ICommand
    {
        ObservableCollection<FacultyListUser> Users;
        public UpdatePinCommand(ObservableCollection<FacultyListUser> users)
        {
            Users = users;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object? parameter, string newPin)
        {
            foreach (FacultyListUser usr in Users)
            {
                usr.Pin = newPin;
            }
            
        }

        public void Execute(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
