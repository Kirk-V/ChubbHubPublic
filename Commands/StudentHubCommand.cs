using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using ChubbHubMVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class StudentHubCommand : NavCommandBase
    {
        public StudentHubCommand(UserReportModel userReport, NavigationStore navigationStore) : base(userReport, navigationStore)
        {
        }

        public override void Execute(object? parameter)
        {
            base._navigationStore.CurrentViewModel = new StudentHubViewModel(base._reportModel, base._navigationStore);
        }
    }
}
