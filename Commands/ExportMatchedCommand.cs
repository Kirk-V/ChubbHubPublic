using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    internal class ExportMatchedCommand: CommandBase
    {
        private readonly Action _callBackMethod;
        public ExportMatchedCommand(Action callBackMethod)
        {
            _callBackMethod = callBackMethod;
        }
        public override void Execute(object? parameter)
        {
            _callBackMethod.Invoke();
        }
    }
}
