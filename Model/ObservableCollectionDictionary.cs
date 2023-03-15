using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Model
{
    public class ObservableCollectionDictionary : INotifyCollectionChanged 
    {
        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }
}

