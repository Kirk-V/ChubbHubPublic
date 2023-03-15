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
    public class AddUsersCommand : CommandBase
    {
        private readonly UserReportModel _userReportModel;
        public AddUsersCommand(UserReportModel userReport)
        {
            _userReportModel = userReport;
        }

        public override void Execute(object? parameter)
        {
            
        }
    }
}
