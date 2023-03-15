using ChubbHubMVVM.Model;
using ChubbHubMVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class SelectFacultyFileCommand : CommandBase
    {
        private FacultyHubViewModel FacultyHubViewModel { get; set; }
        public SelectFacultyFileCommand(FacultyHubViewModel facultyHubViewModel)
        {
            FacultyHubViewModel = facultyHubViewModel;
        }
        public override void Execute(object? parameter)
        {
            this.FacultyHubViewModel.NewFacultyFile();
        }
    }
}
