﻿    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using miRobotEditor.Core.Annotations;

namespace miRobotEditor.Core
{


        [Localizable(false)]
        public class ViewModelBase : SerializeBase,INotifyPropertyChanged 
        {
            public event PropertyChangingEventHandler PropertyChanging;



            protected void RaisePropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }

            private string _serializefile;
            public override string SerializeFileName
            {
                get { return _serializefile; }
                set
                {
                    _serializefile = value;
                    RaisePropertyChanged();
                }
            }
            // ReSharper disable UnusedMember.Local
            void BringToFront()
            // ReSharper restore UnusedMember.Local
            {

            }

            public void SendMessage(string title, string message, MsgIcon icon, bool forceshow = true)
            {
                MessageViewModel.Instance.Add(title, message, icon, forceshow);
            }


            public event PropertyChangedEventHandler PropertyChanged;


            protected virtual void RaisePropertyChanged( string propertyName )
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

