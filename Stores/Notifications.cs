using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Stores
{
    public class Notifications
    {
        private ObservableCollection<string> _messages;
        public ObservableCollection<string> Messages 
        {
            get => _messages;
            set
            {
                _messages = value;
                
            }
        }

        public Notifications()
        {
            Messages = new ObservableCollection<string>();
        }


        public bool IsEmpty()
        {
            if(Messages.Count == 0)
                return false;
            return true;
        }
    }
}
