using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class SelectClassFileCommand : CommandBase
    {

        private readonly Action _callBackMethod;

        public SelectClassFileCommand(Action CallBackMethod)
        {
            _callBackMethod = CallBackMethod;
        }
        public override void Execute(object? parameter)
        {
            _callBackMethod?.Invoke();
        }
    }
}
