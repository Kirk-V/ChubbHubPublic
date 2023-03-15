using ChubbHubMVVM.Model;
using ChubbHubMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.ViewModels 
{
    public class ConfigViewModel : ViewModelBase
    {

        private List<Department> h;
        public ConfigViewModel(UserReportModel userReportModel, NavigationStore navigationStore) : base(userReportModel, navigationStore)
        {
        }
    }
}
