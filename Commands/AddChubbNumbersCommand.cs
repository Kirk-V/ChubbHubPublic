using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class AddChubbNumbersCommand : CommandBase
    {

        private Action _callBackMethod;
        public AddChubbNumbersCommand(Action callBackMethod)
        {
            _callBackMethod = callBackMethod;
        }
        public override void Execute(object? parameter)
        {
           
            _callBackMethod?.Invoke();

            
        }
    }
}
