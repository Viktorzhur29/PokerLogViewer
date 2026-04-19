using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;

namespace PokerLogViewer.Models
{
    public class HandCollection
    {
        public ObservableCollection<HandData> Hands { get; }

        public HandCollection(Dispatcher dispatcher)
        {
            Hands = new ObservableCollection<HandData>();
            BindingOperations.EnableCollectionSynchronization(Hands, new object());
        }
    }
}
