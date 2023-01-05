using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using miRobotEditor.Classes;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Core
{

    public delegate void MessageAddedHandler(object sender, EventArgs e);
    public sealed class MessageViewModel:ToolViewModel
    {
        private const string ToolContentId = "MessageViewTool";
        public event MessageAddedHandler MessageAdded;

        #region Properties
        private static MessageViewModel _instance;
        public static MessageViewModel Instance { get { return _instance ?? new MessageViewModel(); }
            private set { _instance = value; } }


        #region SelectedMessage
        /// <summary>
        /// The <see cref="SelectedMessage" /> property's name.
        /// </summary>
        public const string SelectedMessagePropertyName = "SelectedMessage";

        private OutputWindowMessage _selectedMessage = null;

        /// <summary>
        /// Sets and gets the SelectedMessage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public OutputWindowMessage SelectedMessage
        {
            get
            {
                return _selectedMessage;
            }

            set
            {
                if (_selectedMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedMessagePropertyName);
                _selectedMessage = value;
                RaisePropertyChanged(SelectedMessagePropertyName);
            }
        }
        #endregion


        public ObservableCollection<IMessage> Messages { get; set; }

        #endregion

        void RaiseMessageAdded()
        {
            if (MessageAdded!=null)
                MessageAdded(this,new EventArgs());
        }

        #region Constructor
        public MessageViewModel():base("Output Window")
        {
            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Bottom;
            Messages = new ObservableCollection<IMessage>();
            Instance = this;


            Messenger.Default.Register<Exception>(this, GetException);
        }

        private void GetException(Exception obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static void Add(IMessage msg)
        {
            Add(msg.Title,msg.Description,msg.Icon);
        }

        public void Add(string title, string message, MsgIcon  icon,bool forceactivate=true)
        {
            BitmapImage img = null;

            switch (icon)
            {
                case MsgIcon.Error:
                    img = Utilities.LoadBitmap(Global.ImgError);
                    break;
                case MsgIcon.Info:
                    img = Utilities.LoadBitmap(Global.ImgInfo);
                    break;
            }


        	Messages.Add(new OutputWindowMessage{Title=title, Description=message,Icon=img});

            if (forceactivate)
                RaiseMessageAdded();
        }


        void HandleMouseOver(object param)
        {

           SelectedMessage = (OutputWindowMessage)((ListViewItem)param).Content;
        }

        /// <summary>
        /// Create MessageBox window and displays
        /// </summary>
        /// <param name="message"></param>
        public static void ShowMessage(string message)
        {
            System.Windows.MessageBox.Show(message);
        }

        void ClearItems()
        {
        	Messages.Clear();//=new ObservableCollection<OutputWindowMessage>();
            
        	RaisePropertyChanged("Messages");
        }

        public static void AddError(string message,Exception ex)
        {
            var trace = new System.Diagnostics.StackTrace();
            var msg = new OutputWindowMessage
                {
                    Title = "Internal Error",
                    Icon = Utilities.LoadBitmap(Global.ImgError),
                    Description = String.Format("Internal error\r\n {0} \r\n in {1}", ex.Message, trace.GetFrame(2))
                };
//            msg.Icon = (BitmapImage)Application.Current.Resources.MergedDictionaries[0]["error"];

            Instance.Messages.Add(msg);

        }

        public static void Add(string title, string message, BitmapImage icon, bool forceactivate = true)
        {

            Instance.Messages.Add(new OutputWindowMessage { Title = title, Description = message, Icon = icon });

            if (forceactivate)
                Instance.RaiseMessageAdded();
        }

        #region Commands


        #region ClearMessagesCommand
        private RelayCommand _clearMessagesCommand;

        /// <summary>
        /// Gets the ClearMessagesCommand.
        /// </summary>
        public RelayCommand ClearMessagesCommand => _clearMessagesCommand
                    ?? (_clearMessagesCommand = new RelayCommand(ClearItems));

        #endregion





        #region MouseOverCommand
        private RelayCommand<object> _mouseOverCommand;

        /// <summary>
        /// Gets the MouseOverCommand.
        /// </summary>
        public RelayCommand<object> MouseOverCommand => _mouseOverCommand
                    ?? (_mouseOverCommand = new RelayCommand<object>(HandleMouseOver));


        #endregion
        #endregion

    }
}
