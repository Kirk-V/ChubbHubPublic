using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class CreateFacultyImportFileCommand : CommandBase
    {

        private readonly Action<bool> _callBackMethod;
        public CreateFacultyImportFileCommand(Action<bool> callBackMethod)
        {
            this._callBackMethod = callBackMethod;
        }
        public override void Execute(object? parameter)
        {
            if(parameter == null)
            {
                this._callBackMethod?.Invoke(false);
            }
            else
            {
                this._callBackMethod?.Invoke(true);
            }
            
        }
    }
}
