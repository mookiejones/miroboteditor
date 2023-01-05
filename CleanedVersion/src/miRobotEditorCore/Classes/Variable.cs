using System;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel; 
using miRobotEditor.Core.Interfaces;

namespace miRobotEditor.Core.Classes
{
    public class Variable : ObservableRecipient, IVariable
    {
        #region IsSelected

        /// <summary>
        ///     The <see cref="IsSelected" /> property's name.
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelected;

        /// <summary>
        ///     Sets and gets the IsSelected property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsSelected
        {
                     get=>_isSelected;
            set=>SetProperty(ref _isSelected,value);

        }

        #endregion

        #region Icon

        /// <summary>
        ///     The <see cref="Icon" /> property's name.
        /// </summary>
        public const string IconPropertyName = "Icon";

        private BitmapImage _image;

        /// <summary>
        ///     Sets and gets the Icon property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public BitmapImage Icon
        {
                      get=>_image;
            set=>SetProperty(ref _image,value);
        }

        #endregion

        #region Description

        /// <summary>
        ///     The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _description = String.Empty;

        /// <summary>
        ///     Sets and gets the Description property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Description
        {
                        get=>_description;
            set=>SetProperty(ref _description,value);

        }

        #endregion

        #region Name

        /// <summary>
        ///     The <see cref="Name" /> property's name.
        /// </summary>
        private const string NamePropertyName = "Name";

        private string _name = String.Empty;

        /// <summary>
        ///     Sets and gets the Name property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Name
        {
                     get=>_name;
            set=>SetProperty(ref _name,value);

        }

        #endregion

        #region Type

        /// <summary>
        ///     The <see cref="Type" /> property's name.
        /// </summary>
        public const string TypePropertyName = "Type";

        private string _type = String.Empty;

        /// <summary>
        ///     Sets and gets the Type property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Type
        {
                       get=>_type;
            set=>SetProperty(ref _type,value);

        }

        #endregion

        #region Path

        /// <summary>
        ///     The <see cref="Path" /> property's name.
        /// </summary>
        public const string PathPropertyName = "Path";

        private string _path = String.Empty;

        /// <summary>
        ///     Sets and gets the Path property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Path
        {
                       get=>_path;
            set=>SetProperty(ref _path,value);

        }

        #endregion

        #region Value

        /// <summary>
        ///     The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";

        private string _value = String.Empty;

        /// <summary>
        ///     Sets and gets the Value property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Value
        {
                        get=>_value;
            set=>SetProperty(ref _value,value);

        }

        #endregion

        #region Comment

        /// <summary>
        ///     The <see cref="Comment" /> property's name.
        /// </summary>
        public const string CommentPropertyName = "Comment";

        private string _comment = String.Empty;

        /// <summary>
        ///     Sets and gets the Comment property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Comment
        {
                     get=>_comment;
            set=>SetProperty(ref _comment,value);

        }

        #endregion

        #region Declaration

        /// <summary>
        ///     The <see cref="Declaration" /> property's name.
        /// </summary>
        public const string DeclarationPropertyName = "Declaration";

        private string _declaration = String.Empty;

        /// <summary>
        ///     Sets and gets the Declaration property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Declaration
        {
                        get=>_declaration;
            set=>SetProperty(ref _declaration,value);

        }

        #endregion

        #region Offset

        /// <summary>
        ///     The <see cref="Offset" /> property's name.
        /// </summary>
        public const string OffsetPropertyName = "Offset";

        private int _offset = -1;

        /// <summary>
        ///     Sets and gets the Offset property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Offset
        {
                      get=>_offset;
            set=>SetProperty(ref _offset,value);

        }

        #endregion
    }
}