using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class CreateClassImportFileCommand : CommandBase
    {
        private Action<bool> _callBackMethod;

        public CreateClassImportFileCommand(Action<bool> callBackMethod)
        {
            _callBackMethod = callBackMethod;
        }
    
        public override void Execute(object? parameter)
        {
            if (parameter == null) return;
            _callBackMethod?.Invoke((bool)parameter);
        }
    }
}
