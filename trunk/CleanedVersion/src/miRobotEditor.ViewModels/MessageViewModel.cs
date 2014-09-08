using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using miRobotEditor.Core.Classes;
using miRobotEditor.Core.Classes.Messaging;

namespace miRobotEditor.Core
{
    public delegate void MessageAddedHandler(object sender, EventArgs e);
   public class MessageViewModel:ViewModelBase
   {
       #region Members
       private const string ToolContentId = @"MessageViewTool";
       public event MessageAddedHandler MessageAdded;

       #endregion

       static BitmapImage GetMsgIcon(MsgIcon icon)
       {
           switch (icon)
           {
               case MsgIcon.Error:
                   return Utilities.LoadBitmap(Global.ImgError);
                   break;
               case MsgIcon.Info:
                  return Utilities.LoadBitmap(Global.ImgInfo);
           }
           throw new NotImplementedException();
       }

       void RaiseMessageAdded()
       {
           if (MessageAdded != null)
               MessageAdded(this, new EventArgs());
       }


       #region Constructor

       public MessageViewModel() : base()
       {
           
       }


       #endregion
   }
}
