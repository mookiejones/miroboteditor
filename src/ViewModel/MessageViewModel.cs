using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.Classes;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{

    public delegate void MessageAddedHandler(object sender, EventArgs e);
    public class MessageViewModel:ToolViewModel
    {
        public const string ToolContentId = "MessageViewTool";
        public event MessageAddedHandler MessageAdded;

        #region Properties
        private static MessageViewModel _instance;
        public static MessageViewModel Instance { get { return _instance ?? new MessageViewModel(); } set { _instance = value; } }



        private OutputWindowMessage _selectedMessage ;
        public OutputWindowMessage SelectedMessage {get{return _selectedMessage;}set{_selectedMessage=value;RaisePropertyChanged("SelectedMessage");}}

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
        public RelayCommand ClearMessagesCommand
        {
            get
            {
                return _clearMessagesCommand
                    ?? (_clearMessagesCommand = new RelayCommand(ExecuteClearMessagesCommand));
            }
        }

        private void ExecuteClearMessagesCommand()
        {
            ClearItems();
        }
        #endregion

        private RelayCommand<ListViewItem> _mouseOverCommand;

        /// <summary>
        /// Gets the MouseOverCommand.
        /// </summary>
        public RelayCommand<ListViewItem> MouseOverCommand
        {
            get
            {
                return _mouseOverCommand
                    ?? (_mouseOverCommand = new RelayCommand<ListViewItem>(ExecuteMouseOverCommand));
            }
        }

        private void ExecuteMouseOverCommand(ListViewItem item)
        {
            SelectedMessage = (OutputWindowMessage)item.Content;
        }

      
        #endregion

    }

 
    public enum MsgIcon { Error, Info }
    public interface IMessage
    {
        BitmapImage Icon { get; set; }
        string Time { get; set; }
        string Title { get; set; }
        string Description { get; set; }
    }
    public class OutputWindowMessage : IMessage
    {
        public string Time { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public BitmapImage Icon { get; set; }
    }
}
