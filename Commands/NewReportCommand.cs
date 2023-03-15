using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class NewReportCommand : CommandBase
    {

        public Action Callback;
        public NewReportCommand(Action newReportAction)
        {
            Callback = newReportAction;
        }
        public override void Execute(object? parameter)
        {
            Callback?.Invoke();
        }
    }
}
