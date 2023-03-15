using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.ViewModels
{
    public class AddUsersViewModel: ViewModelBase
    {

        public AddUsersViewModel(UserReportModel userReportModel, NavigationStore navigationStore ) :base(userReportModel, navigationStore)
        {

        }
    }
}
