using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using miRobotEditor.Classes;
using miRobotEditor.GUI;
using miRobotEditor.Commands;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace miRobotEditor.ViewModel
{

    //TODO Add Message Icons
    public enum MSGIcon { ERROR, INFO }

    public class MessageViewModel:ViewModelBase
    {


        #region Properties
        private static MessageViewModel _instance;
        public static MessageViewModel Instance { get { return _instance ?? new MessageViewModel(); } set { _instance = value; } }



        private OutputWindowMessage _selectedMessage ;
        public OutputWindowMessage SelectedMessage {get{return _selectedMessage;}set{_selectedMessage=value;RaisePropertyChanged("SelectedMessage");}}

        public ObservableCollection<OutputWindowMessage> Messages { get; set; }

        #endregion

        #region Constructor
        public MessageViewModel()
        {
            Messages = new ObservableCollection<OutputWindowMessage>();
            Instance = this;
        }        

        #endregion

        public static void Add(OutputWindowMessage msg)
        {
            Add(msg.Title,msg.Description,msg.Icon,true);
        }


        public  void Add(string title, string message, MSGIcon  icon,bool forceactivate=true)
        {
            BitmapImage img = null;

            switch (icon)
            {
                case MSGIcon.ERROR:
                    img = Utilities.LoadBitmap(Global.ImgError);;
                    break;
                case MSGIcon.INFO:
                    img = Utilities.LoadBitmap(Global.ImgInfo);
                    break;
            }


        	Messages.Add(new OutputWindowMessage{Title=title, Description=message,Icon=img});

            if (forceactivate)
                Workspace.Instance.BringToFront("Output Window");
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
            var msg = new OutputWindowMessage();
            msg.Title = "Internal Error";
//            msg.Icon = (BitmapImage)Application.Current.Resources.MergedDictionaries[0]["error"];
            msg.Icon = Utilities.LoadBitmap(Global.ImgError);
            msg.Description = String.Format("Internal error\r\n {0} \r\n in {1}", ex.Message, trace.GetFrame(2));

            Instance.Messages.Add(msg);

        }

        public static void Add(string title, string message, System.Windows.Media.Imaging.BitmapImage icon, bool forceactivate = true)
        {

            MessageViewModel.Instance.Messages.Add(new OutputWindowMessage { Title = title, Description = message, Icon = icon });

            if (forceactivate)
                Workspace.Instance.BringToFront("Output Window");
        }

        #region Commands
        private  RelayCommand _clearMessagesCommand;
        public  ICommand ClearMessagesCommand
        {
            get { return _clearMessagesCommand ?? (_clearMessagesCommand = new RelayCommand(param => ClearItems(), param => true)); }
        }

        private RelayCommand _mouseOverCommand;
        public ICommand MouseOverCommand
        {
            get { return _mouseOverCommand ?? (_mouseOverCommand = new RelayCommand(p => HandleMouseOver(p), p => true)); }
        }
        #endregion

    }
    public class OutputWindowMessage
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public BitmapImage Icon { get; set; }
    }
    /*
    public class MessageWindow
    {
        /// <summary>
        /// Interface Message to reach Message Window
        /// </summary>
        /// <param name="msg"></param>
        public static void Add(OutputWindowMessage msg)
        {
            MessageViewModel.Instance.Messages.Add(msg);
        }
        public static void AddError(Exception ex)
        {
            var trace = new System.Diagnostics.StackTrace();
            var msg = new OutputWindowMessage();
            msg.Title = "Internal Error";
            msg.Description = String.Format("Internal error\r\n {0} \r\n in {1}", ex.Message, trace.GetFrame(1).GetMethod().Name);

            MessageViewModel.Instance.Messages.Add(msg);

        }



      
        public static void Add(OutputWindowMessage msg, bool debugonly)
        {
#if DEBUG
            MessageViewModel.Instance.Messages.Add(msg);
#endif

        }

        public static void Add(System.Exception ex)
        {
            OutputWindowMessage msg = new OutputWindowMessage();
            msg.Title = "Exception";
            msg.Description = ex.InnerException.InnerException + "\r\n" + ex.StackTrace;

            MessageViewModel.Instance.Messages.Add(msg);
        }



        void HandleMouseOver(object sender)
        {

            MessageViewModel.Instance.SelectedMessage = (OutputWindowMessage)((ListViewItem)sender).Content;
        }
    }
    */
}
