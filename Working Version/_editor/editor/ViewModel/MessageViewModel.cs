using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using miRobotEditor.Classes;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Utilities;

namespace miRobotEditor.ViewModel
{
    public delegate void MessageAddedHandler(object sender, EventArgs e);

    public sealed class MessageViewModel : ToolViewModel
    {
        private const string ToolContentId = "MessageViewTool";

        #region Constructor

        public MessageViewModel()
            : base("Messages")
        {
            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Bottom;


            Messenger.Default.Register<IMessage>(this, AddMessage);
        }

        private void AddMessage(IMessage obj)
        {
            _messages.Add(obj);
        }

        #endregion

        private RelayCommand _clearMessagesCommand;
        private RelayCommand<object> _mouseOverCommand;

        public ICommand ClearMessagesCommand
        {
            get { return _clearMessagesCommand ?? (_clearMessagesCommand = new RelayCommand(ClearItems)); }
        }

        public RelayCommand<object> MouseOverCommand
        {
            get { return _mouseOverCommand ?? (_mouseOverCommand = new RelayCommand<object>(HandleMouseOver)); }
        }

        public event MessageAddedHandler MessageAdded;

        private void RaiseMessageAdded()
        {
            if (MessageAdded != null)
            {
                MessageAdded(this, new EventArgs());
            }
        }

        public void Add(IMessage msg)
        {
            Add(msg.Title, msg.Description, msg.Icon, true);
            if (MessageAdded != null)
                MessageAdded(this, new EventArgs());
        }

        public void Add(string title, string message, MsgIcon icon, bool forceactivate = true)
        {
            BitmapImage icon2 = null;
            switch (icon)
            {
                case MsgIcon.Error:
                    icon2 = ImageHelper.LoadBitmap("..\\..\\Resources\\error.png");
                    break;
                case MsgIcon.Info:
                    icon2 = ImageHelper.LoadBitmap("..\\..\\Resources\\info.png");
                    break;
            }
            _messages.Add(new OutputWindowMessage
            {
                Title = title,
                Description = message,
                Icon = icon2
            });
            if (forceactivate)
            {
                RaiseMessageAdded();
            }
        }

        private void HandleMouseOver(object param)
        {
            SelectedMessage = (OutputWindowMessage) ((ListViewItem) param).Content;
        }

        public static void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void ClearItems()
        {
            _messages.Clear();
            RaisePropertyChanged("Messages");
        }

        public void AddError(string message, Exception ex)
        {
            var stackTrace = new StackTrace();
            var item = new OutputWindowMessage
            {
                Title = "Internal Error",
                Icon = ImageHelper.LoadBitmap("..\\..\\Resources\\error.png"),
                Description = string.Format("Internal error\r\n {0} \r\n in {1}", ex.Message, stackTrace.GetFrame(2))
            };
            _messages.Add(item);
        }

        public void Add(string title, string message, BitmapImage icon, bool forceActivate = true)
        {
            _messages.Add(new OutputWindowMessage
            {
                Title = title,
                Description = message,
                Icon = icon
            });
            if (forceActivate)
            {
                RaiseMessageAdded();
            }
        }

        #region SelectedMessage

       

        private IMessage _selectedMessage;

        /// <summary>
        ///     Sets and gets the SelectedMessage property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public IMessage SelectedMessage
        {
            get { return _selectedMessage; }

            set
            {
                if (_selectedMessage == value)
                {
                    return;
                }

                 
                _selectedMessage = value;
                RaisePropertyChanged(nameof(SelectedMessage));
            }
        }

        #endregion

        #region Messages

        private readonly ObservableCollection<IMessage> _messages = new ObservableCollection<IMessage>();
        private ReadOnlyObservableCollection<IMessage> _readOnlyMessages;

        public ReadOnlyObservableCollection<IMessage> Messages
        {
            get
            {
                return _readOnlyMessages ?? (_readOnlyMessages = new ReadOnlyObservableCollection<IMessage>(_messages));
            }
        }

        #endregion
    }
}