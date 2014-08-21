    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using miRobotEditor.Core.Annotations;

namespace miRobotEditor.Core
{


        public class ViewModelBase : SerializeBase,INotifyPropertyChanged ,INotifyPropertyChanging
        {
            public event PropertyChangingEventHandler PropertyChanging;



            protected void RaisePropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


