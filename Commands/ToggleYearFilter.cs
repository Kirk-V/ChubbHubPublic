using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Commands
{
    public class ToggleYearFilter : CommandBase
    {
        private Action<string> _callBack;
        public ToggleYearFilter(Action<string> callBack)
        {
            _callBack = callBack;
        }
        public override void Execute(object? parameter)
        {
            if (parameter == null)
            {
                this._callBack?.Invoke(string.Empty);
            }
            else
            {
                this._callBack?.Invoke(parameter as string);
            }
        }
    }
}
