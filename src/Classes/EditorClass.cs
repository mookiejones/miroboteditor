using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace miRobotEditor.Classes
{
    public static class EditorClass
    {
        private static RelayCommand _closeAllFoldsCommand;

        /// <summary>
        /// Gets the CloseAllFoldsCommand.
        /// </summary>
        public static RelayCommand CloseAllFoldsCommand => _closeAllFoldsCommand
                    ?? (_closeAllFoldsCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));

        private static RelayCommand _openAllFoldsCommand;

        /// <summary>
        /// Gets the OpenAllFoldsCommand.
        /// </summary>
        public static RelayCommand OpenAllFoldsCommand => _openAllFoldsCommand
                    ?? (_openAllFoldsCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));

        private static RelayCommand _gotoCommand;

        /// <summary>
        /// Gets the GotoCommand.
        /// </summary>
        public static RelayCommand GotoCommand => _gotoCommand
                    ?? (_gotoCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
        private static RelayCommand _reloadCommand;

        /// <summary>
        /// Gets the ReloadCommand.
        /// </summary>
        public static RelayCommand ReloadCommand => _reloadCommand
                    ?? (_reloadCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));

        private static RelayCommand _renumberLinesCommand;

        /// <summary>
        /// Gets the RenumberLinesCommand.
        /// </summary>
        public static RelayCommand RenumberLinesCommand => _renumberLinesCommand
                    ?? (_renumberLinesCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
    }
}
