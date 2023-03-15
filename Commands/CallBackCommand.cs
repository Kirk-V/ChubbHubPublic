using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class CallBackCommand : CommandBase
    {
        public Action CallBack;
        public override void Execute(object? parameter)
        {
            CallBack?.Invoke();
        }

        public CallBackCommand(Action callBack)
        {
            CallBack = callBack;
        }   
    }
}
