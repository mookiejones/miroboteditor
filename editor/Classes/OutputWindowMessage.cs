﻿using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;
using RobotEditor.Interfaces;

namespace RobotEditor.Classes
{
    public sealed class OutputWindowMessage : MessageBase, IMessage
    {
        public string Time { get; set; }
        public string Description { get; set; }

        public bool ForceActivation { get; set; }

        public string Title { get; set; }
        public BitmapImage Icon { get; set; }
    }
}