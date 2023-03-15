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
    public class RegistrarHubCommand : NavCommandBase
    {
        private readonly UserReportModel _userReportModel;
        public RegistrarHubCommand(UserReportModel userReport, NavigationStore navStore) :base(userReport, navStore)
        {
            _userReportModel = userReport;
        }

        public override void Execute(object? parameter)
        {
            base._navigationStore.CurrentViewModel = new RegistrarHubViewModel(base._reportModel, base._navigationStore);
        }
    }
}
