using ChubbHubMVVM.Commands;
using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using ChubbHubMVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Command
{
    public class ConfigNavCommand : NavCommandBase
    {
        public ConfigNavCommand(UserReportModel userReport, NavigationStore navigationStore) : base(userReport, navigationStore)
        {

        }
        public override void Execute(object? parameter)
        {
            base._navigationStore.CurrentViewModel = new ConfigViewModel(base._reportModel, base._navigationStore);
        }
    }
}
