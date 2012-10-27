using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace miRobotEditor.WPFPresentation.Controls
{
    /// <summary>
    /// Interaction logic for ExtendedGridSplitterControl.xaml
    /// </summary>
    public partial class ExtendedGridSplitterControl : GridSplitter
    {
        /// <summary>
        /// Initializes a new instance of the ExtendedGridSplitter class,
        /// which inherits from System.Windows.Controls.ExtendedGridSplitterControl.
        /// </summary>
        public ExtendedGridSplitterControl()
        {
            InitializeComponent();
            // Set default values
            DefaultStyleKey = typeof (ExtendedGridSplitterControl);

            CollapseMode = GridSplitterCollapseMode.None;
            LayoutUpdated += delegate { _gridCollapseDirection = GetCollapseDirection(); };

            // All ExtendedGridSplitter visual states are handled by the parent GridSplitter class.
        }

        public static bool Animating = false;


        #region TemplateParts

        private const string ELEMENT_LABEL_NAME = "LabelHandle";
        private const string ELEMENT_SWITCHARROW_HANDLE_NAME = "SwitchArrows";
        private const string ELEMENT_HORIZONTAL_HANDLE_NAME = "HorizontalGridSplitterHandle";
        private const string ELEMENT_VERTICAL_HANDLE_NAME = "VerticalGridSplitterHandle";
        private const string ELEMENT_HORIZONTAL_TEMPLATE_NAME = "HorizontalTemplate";
        private const string ELEMENT_VERTICAL_TEMPLATE_NAME = "VerticalTemplate";
        private const string ELEMENT_GRIDSPLITTER_BACKGROUND = "GridSplitterBackground";

        private ToggleButton _elementHorizontalGridSplitterButton;
        private ToggleButton _elementVerticalGridSplitterButton;
        private ToggleButton _elementSwitchWindowsButton;


        private Rectangle _elementGridSplitterBackground;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a value that indicates the CollapseMode.
        /// </summary>
        public GridSplitterCollapseMode CollapseMode
        {
            get { return (GridSplitterCollapseMode) GetValue(CollapseModeProperty); }
            set { SetValue(CollapseModeProperty, value); }
        }

        /// <summary>
        /// Identifies the CollapseMode dependency property
        /// </summary>
        public static readonly DependencyProperty CollapseModeProperty = DependencyProperty.Register("CollapseMode",
                                                                                                     typeof (
                                                                                                         GridSplitterCollapseMode
                                                                                                         ),
                                                                                                     typeof (
                                                                                                         ExtendedGridSplitterControl
                                                                                                         ),
                                                                                                     new PropertyMetadata
                                                                                                         (GridSplitterCollapseMode
                                                                                                              .None,
                                                                                                          OnCollapseModePropertyChanged));

        /// <summary>
        /// Gets or sets the style that customizes the appearance of the horizontal handle 
        /// that is used to expand and collapse the ExtendedGridSplitter.
        /// </summary>
        public Style SwitchArrowStyle
        {
            get { return (Style) GetValue(SwitchArrowStyleProperty); }
            set { SetValue(SwitchArrowStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style that customizes the appearance of the horizontal handle 
        /// that is used to expand and collapse the ExtendedGridSplitter.
        /// </summary>
        public Style LabelStyle
        {
            get { return (Style) GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style that customizes the appearance of the horizontal handle 
        /// that is used to expand and collapse the ExtendedGridSplitter.
        /// </summary>
        public Color FocusColor
        {
            get { return (Color) GetValue(FocusColorProperty); }
            set { SetValue(FocusColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style that customizes the appearance of the horizontal handle 
        /// that is used to expand and collapse the ExtendedGridSplitter.
        /// </summary>
        public Color UnfocusColor
        {
            get { return (Color) GetValue(UnfocusColorProperty); }
            set { SetValue(UnfocusColorProperty, value); }
        }

        public virtual void OnFocusStyleChanged(Style v)
        {


        }

        /// <summary>
        /// Gets or sets the style that customizes the appearance of the horizontal handle 
        /// that is used to expand and collapse the ExtendedGridSplitter.
        /// </summary>
        public Style HorizontalHandleStyle
        {
            get { return (Style) GetValue(HorizontalHandleStyleProperty); }
            set { SetValue(HorizontalHandleStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty HorizontalHandleStyleProperty =
            DependencyProperty.Register("HorizontalHandleStyle", typeof (Style), typeof (ExtendedGridSplitterControl),
                                        null);

        /// <summary>
        /// Identifies the HorizontalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty FocusColorProperty = DependencyProperty.Register("FocusColor",
                                                                                                   typeof (Color),
                                                                                                   typeof (
                                                                                                       ExtendedGridSplitterControl
                                                                                                       ), null);

        /// <summary>
        /// Identifies the HorizontalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty UnfocusColorProperty = DependencyProperty.Register("UnfocusColor",
                                                                                                     typeof (Color),
                                                                                                     typeof (
                                                                                                         ExtendedGridSplitterControl
                                                                                                         ), null);

        /// <summary>
        /// Identifies the HorizontalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle",
                                                                                                   typeof (Style),
                                                                                                   typeof (
                                                                                                       ExtendedGridSplitterControl
                                                                                                       ), null);

        /// <summary>
        /// Identifies the SwitchArrowStyle dependency property
        /// </summary>
        public static readonly DependencyProperty SwitchArrowStyleProperty =
            DependencyProperty.Register("SwitchArrowStyle", typeof (Style), typeof (ExtendedGridSplitterControl), null);

        ///<summary>
        /// Gets or sets the style that customizes the appearance of the vertical handle 
        /// that is used to expand and collapse the ExtendedGridSplitter.
        /// </summary>
        public Style VerticalHandleStyle
        {
            get { return (Style) GetValue(VerticalHandleStyleProperty); }
            set { SetValue(VerticalHandleStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty VerticalHandleStyleProperty =
            DependencyProperty.Register("VerticalHandleStyle", typeof (Style), typeof (ExtendedGridSplitterControl),
                                        null);

        /// <summary>
        /// Gets or sets a value that indicates if the collapse and
        /// expanding actions should be animated.
        /// </summary>
        public bool IsAnimated
        {
            get { return (bool) GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register("IsAnimated",
                                                                                                   typeof (bool),
                                                                                                   typeof (
                                                                                                       ExtendedGridSplitterControl
                                                                                                       ), null);

        /// <summary>
        /// Gets or sets a value that indicates if the target column is 
        /// currently collapsed.
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool) GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsCollapsed dependency property
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register("IsCollapsed",
                                                                                                    typeof (bool),
                                                                                                    typeof (
                                                                                                        ExtendedGridSplitterControl
                                                                                                        ),
                                                                                                    new PropertyMetadata
                                                                                                        (OnIsCollapsedPropertyChanged));

        /// <summary>
        /// The IsCollapsed property porperty changed handler.
        /// </summary>
        /// <param name="d">ExtendedGridSplitter that changed IsCollapsed.</param>
        /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
        private static void OnIsCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = d as ExtendedGridSplitterControl;

            var value = (bool) e.NewValue;
            if (s != null) s.OnIsCollapsedChanged(value);
        }

        /// <summary>
        /// The CollapseModeProperty property changed handler.
        /// </summary>
        /// <param name="d">ExtendedGridSplitter that changed IsCollapsed.</param>
        /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
        private static void OnCollapseModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = d as ExtendedGridSplitterControl;

            var value = (GridSplitterCollapseMode) e.NewValue;
            if (s != null) s.OnCollapseModeChanged(value);
        }

        #endregion



        #region Local Members

        private GridCollapseDirection _gridCollapseDirection = GridCollapseDirection.Auto;
        private GridLength _savedGridLength;
        private double _savedActualValue;
        private const double _animationTimeMillis = 200;

        #endregion

        #region Control Instantiation


        /// <summary>
        /// This method is called when the tempalte should be applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _elementSwitchWindowsButton = GetTemplateChild(ELEMENT_SWITCHARROW_HANDLE_NAME) as ToggleButton;
            _elementHorizontalGridSplitterButton = GetTemplateChild(ELEMENT_HORIZONTAL_HANDLE_NAME) as ToggleButton;
            _elementVerticalGridSplitterButton = GetTemplateChild(ELEMENT_VERTICAL_HANDLE_NAME) as ToggleButton;
            _elementGridSplitterBackground = GetTemplateChild(ELEMENT_GRIDSPLITTER_BACKGROUND) as Rectangle;


            // Wire up the Checked and Unchecked events of the HorizontalGridSplitterHandle.
            if (_elementHorizontalGridSplitterButton != null)
            {
                _elementHorizontalGridSplitterButton.Checked += GridSplitterButton_Checked;
                _elementHorizontalGridSplitterButton.Unchecked += GridSplitterButton_Unchecked;
            }

            // Wire up the Checked and Unchecked events of the VerticalGridSplitterHandle.
            if (_elementVerticalGridSplitterButton != null)
            {
                _elementVerticalGridSplitterButton.Checked += GridSplitterButton_Checked;
                _elementVerticalGridSplitterButton.Unchecked += GridSplitterButton_Unchecked;
            }

            // Set default direction since we don't have all the components layed out yet.
            _gridCollapseDirection = GridCollapseDirection.Auto;
            // Directely call these events so design-time view updates appropriately
            OnCollapseModeChanged(CollapseMode);
            OnIsCollapsedChanged(IsCollapsed);
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Handles the property change event of the IsCollapsed property.
        /// </summary>
        /// <param name="isCollapsed">The new value for the IsCollapsed property.</param>
        protected virtual void OnIsCollapsedChanged(bool isCollapsed)
        {
            // Determine if we are dealing with a vertical or horizontal splitter.
            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Sets the target ToggleButton's IsChecked property equal
                // to the provided isCollapsed property.
                if (_elementHorizontalGridSplitterButton != null)
                    _elementHorizontalGridSplitterButton.IsChecked = isCollapsed;
            }
            else
            {
                // Sets the target ToggleButton's IsChecked property equal
                // to the provided isCollapsed property.

                if (_elementVerticalGridSplitterButton != null)
                    _elementVerticalGridSplitterButton.IsChecked = isCollapsed;
            }
        }




        /// <summary>
        /// Handles the property change event of the CollapseMode property.
        /// </summary>
        /// <param name="collapseMode">The new value for the CollapseMode property.</param>
        protected virtual void OnCollapseModeChanged(GridSplitterCollapseMode collapseMode)
        {
            // Hide the handles if the CollapseMode is set to None.
            if (collapseMode == GridSplitterCollapseMode.None)
            {
                if (_elementHorizontalGridSplitterButton != null)
                    _elementHorizontalGridSplitterButton.Visibility = Visibility.Collapsed;

                if (_elementVerticalGridSplitterButton != null)
                    _elementVerticalGridSplitterButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Ensure the handles are Visible.
                if (_elementHorizontalGridSplitterButton != null)
                    _elementHorizontalGridSplitterButton.Visibility = Visibility.Visible;

                if (_elementVerticalGridSplitterButton != null)
                    _elementVerticalGridSplitterButton.Visibility = Visibility.Visible;

                //TODO:  Add in error handling if current template does not include an existing ScaleTransform.

                // Rotate the direction that the handle is facing depending on the CollapseMode.
                if (collapseMode == GridSplitterCollapseMode.Previous)
                {
                    if (_elementHorizontalGridSplitterButton != null)
                    {
                        //                        _elementHorizontalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, -1.0);
                    }
                    if (_elementVerticalGridSplitterButton != null)
                    {
                        //                      _elementVerticalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, -1.0);
                    }
                }
                else if (collapseMode == GridSplitterCollapseMode.Next)
                {
                    if (_elementHorizontalGridSplitterButton != null)
                    {
                        //     _elementHorizontalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, 1.0);
                    }
                    if (_elementVerticalGridSplitterButton != null)
                    {
                        //     _elementVerticalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, 1.0);
                    }
                }

            }

        }

        #endregion


        public void Goto(int value)
        {

        }

        #region Collapse and Expand Methods

        /// <summary>
        /// Collapses the target ColumnDefinition or RowDefinition.
        /// </summary>
        public void Collapse()
        {

            var parentGrid = Parent as Grid;
            int splitterIndex;

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Get the index of the row containing the splitter
                splitterIndex = (int) GetValue(Grid.RowProperty);

                // Determing the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Previous)
                {
                    // Save the next rows Height information
                    if (parentGrid != null)
                    {
                        _savedGridLength = parentGrid.RowDefinitions[splitterIndex + 1].Height;
                        _savedActualValue = parentGrid.RowDefinitions[splitterIndex + 1].ActualHeight;

                        // Collapse the next row
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.RowDefinitions[splitterIndex + 1]);
                        else
                            parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty,
                                                                                  new GridLength(0));
                    }
                }
                else
                {
                    // Save the previous row's Height information
                    if (parentGrid != null)
                    {
                        _savedGridLength = parentGrid.RowDefinitions[splitterIndex + 1].Height;
                        _savedActualValue = parentGrid.RowDefinitions[splitterIndex + 1].ActualHeight;

                        // Collapse the previous row
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.RowDefinitions[splitterIndex - 1]);
                        else
                            parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty,
                                                                                  new GridLength(0));
                    }
                }
            }
            else
            {
                // Get the index of the column containing the splitter
                splitterIndex = (int) GetValue(Grid.ColumnProperty);

                // Determing the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Next)
                {
                    // Save the next column's Width information
                    if (parentGrid != null)
                    {
                        _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex + 1].Width;
                        _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex + 1].ActualWidth;

                        // Collapse the next column
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                        else
                            parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty,
                                                                                     new GridLength(0));
                    }
                }
                else
                {
                    // Save the previous column's Width information
                    if (parentGrid != null)
                    {
                        _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex - 1].Width;
                        _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex - 1].ActualWidth;

                        // Collapse the previous column
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                        else
                            parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty,
                                                                                     new GridLength(0));
                    }
                }
            }


        }

        /// <summary>
        /// Expands the target ColumnDefinition or RowDefinition.
        /// </summary>
        private void Expand()
        {

            var parentGrid = Parent as Grid;
            int splitterIndex;

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Get the index of the row containing the splitter
                splitterIndex = (int) GetValue(Grid.RowProperty);

                // Determine the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Previous)
                {
                    // Expand the next row
                    if (IsAnimated)
                    {
                        if (parentGrid != null) AnimateExpand(parentGrid.RowDefinitions[splitterIndex + 1]);
                    }
                    else if (parentGrid != null)
                        parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty,
                                                                              _savedGridLength);
                }
                else
                {
                    // Expand the previous row
                    if (IsAnimated)
                    {
                        if (parentGrid != null) AnimateExpand(parentGrid.RowDefinitions[splitterIndex - 1]);
                    }
                    else if (parentGrid != null)
                        parentGrid.RowDefinitions[splitterIndex - 1].SetValue(RowDefinition.HeightProperty,
                                                                              _savedGridLength);
                }
            }
            else
            {
                // Get the index of the column containing the splitter
                splitterIndex = (int) GetValue(Grid.ColumnProperty);

                // Determine the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Next)
                {
                    // Expand the next column
                    if (IsAnimated)
                    {
                        if (parentGrid != null) AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                    }
                    else if (parentGrid != null)
                        parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty,
                                                                                 _savedGridLength);
                }
                else
                {
                    // Expand the previous column
                    if (IsAnimated)
                    {
                        if (parentGrid != null) AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                    }
                    else if (parentGrid != null)
                        parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty,
                                                                                 _savedGridLength);
                }
            }

        }

        /// <summary>
        /// Determine the collapse direction based on the horizontal and vertical alignments
        /// </summary>
        private GridCollapseDirection GetCollapseDirection()
        {
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridCollapseDirection.Columns;
            }
            if ((VerticalAlignment == VerticalAlignment.Stretch) && (ActualWidth <= ActualHeight))
            {
                return GridCollapseDirection.Columns;
            }
            return GridCollapseDirection.Rows;
        }

        #endregion

        #region Event Handlers and Throwers

        // Define Collapsed and Expanded evenets
        public event EventHandler<EventArgs> Collapsed;
        public event EventHandler<EventArgs> Expanded;

        /// <summary>
        /// Raises the Collapsed event.
        /// </summary>
        /// <param name="e">Contains event arguments.</param>
        protected virtual void OnCollapsed(EventArgs e)
        {
            EventHandler<EventArgs> handler = Collapsed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the Expanded event.
        /// </summary>
        /// <param name="e">Contains event arguments.</param>
        protected virtual void OnExpanded(EventArgs e)
        {
            EventHandler<EventArgs> handler = Expanded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Handles the Checked event of either the Vertical or Horizontal
        /// GridSplitterHandle ToggleButton.
        /// </summary>
        /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
        /// <param name="e">Contains event arguments for the routed event that fired.</param>
        private void GridSplitterButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IsCollapsed != true)
            {
                // In our case, Checked = Collapsed.  Which means we want everything
                // ready to be expanded.
                Collapse();

                IsCollapsed = true;

                // Deactivate the background so the splitter can not be dragged.
                _elementGridSplitterBackground.IsHitTestVisible = false;
                _elementGridSplitterBackground.Opacity = 0.5;

                // Raise the Collapsed event.
                OnCollapsed(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Handles the Unchecked event of either the Vertical or Horizontal
        /// GridSplitterHandle ToggleButton.
        /// </summary>
        /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
        /// <param name="e">Contains event arguments for the routed event that fired.</param>
        private void GridSplitterButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsCollapsed)
            {
                // In our case, Unchecked = Expanded.  Which means we want everything
                // ready to be collapsed.
                Expand();

                IsCollapsed = false;

                // Activate the background so the splitter can be dragged again.
                _elementGridSplitterBackground.IsHitTestVisible = true;
                //_elementGridSplitterBackground.Opacity = 1;

                // Raise the Expanded event.
                OnExpanded(EventArgs.Empty);
            }
        }

        #endregion

        #region Collapse and Expand Animations

        #region Property for animating rows

        private RowDefinition AnimatingRow;

        private static readonly DependencyProperty RowHeightAnimationProperty =
            DependencyProperty.Register(
                "RowHeightAnimation",
                typeof (double),
                typeof (ExtendedGridSplitterControl),
                new PropertyMetadata(RowHeightAnimationChanged)
                );

        private static void RowHeightAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var extendedGridSplitter = d as ExtendedGridSplitterControl;
            if (extendedGridSplitter != null)
                extendedGridSplitter.AnimatingRow.Height = new GridLength((double) e.NewValue);
        }

        #endregion

        #region Property for animating columns

        private ColumnDefinition AnimatingColumn;

        private static readonly DependencyProperty ColWidthAnimationProperty =
            DependencyProperty.Register(
                "ColWidthAnimation",
                typeof (double),
                typeof (ExtendedGridSplitterControl),
                new PropertyMetadata(ColWidthAnimationChanged)
                );

        private static void ColWidthAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var extendedGridSplitter = d as ExtendedGridSplitterControl;
            if (extendedGridSplitter != null)
                extendedGridSplitter.AnimatingColumn.Width = new GridLength((double) e.NewValue);
        }

        #endregion

        /// <summary>
        /// Uses DoubleAnimation and a StoryBoard to animated the collapsing
        /// of the specificed ColumnDefinition or RowDefinition.
        /// </summary>
        /// <param name="definition">The RowDefinition or ColumnDefintition that will be collapsed.</param>
        private void AnimateCollapse(object definition)
        {
            double currentValue;

            // Setup the animation and StoryBoard
            var gridLengthAnimation = new DoubleAnimation
                                          {Duration = new Duration(TimeSpan.FromMilliseconds(_animationTimeMillis))};
            var sb = new Storyboard();

            // Add the animation to the StoryBoard
            sb.Children.Add(gridLengthAnimation);

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                AnimatingRow = (RowDefinition) definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("RowHeightAnimation"));

                currentValue = AnimatingRow.ActualHeight;
            }
            else
            {
                // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                AnimatingColumn = (ColumnDefinition) definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("ColWidthAnimation"));

                currentValue = AnimatingColumn.ActualWidth;
            }

            gridLengthAnimation.From = currentValue;
            gridLengthAnimation.To = 0;

            // Start the StoryBoard.
            sb.Begin();
        }

        /// <summary>
        /// Uses DoubleAnimation and a StoryBoard to animate the expansion
        /// of the specificed ColumnDefinition or RowDefinition.
        /// </summary>
        /// <param name="definition">The RowDefinition or ColumnDefintition that will be expanded.</param>
        private void AnimateExpand(object definition)
        {
            double currentValue;

            // Setup the animation and StoryBoard
            var gridLengthAnimation = new DoubleAnimation
                                          {Duration = new Duration(TimeSpan.FromMilliseconds(_animationTimeMillis))};
            var sb = new Storyboard();

            // Add the animation to the StoryBoard
            sb.Children.Add(gridLengthAnimation);

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                AnimatingRow = (RowDefinition) definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("RowHeightAnimation"));

                currentValue = AnimatingRow.ActualHeight;
            }
            else
            {
                // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                AnimatingColumn = (ColumnDefinition) definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("ColWidthAnimation"));

                currentValue = AnimatingColumn.ActualWidth;
            }
            gridLengthAnimation.From = currentValue;
            gridLengthAnimation.To = _savedActualValue;

            // Start the StoryBoard.
            sb.Begin();
        }

        #endregion

        /// <summary>
        /// An enumeration that specifies the direction the ExtendedGridSplitter will
        /// be collapased (Rows or Columns).
        /// </summary>
        private enum GridCollapseDirection
        {
            Auto,
            Columns,
            Rows
        }

    }

    /// <summary>
    /// Specifies different collapse modes of a ExtendedGridSplitter.
    /// </summary>
    public enum GridSplitterCollapseMode
    {
        /// <summary>
        /// The ExtendedGridSplitter cannot be collapsed or expanded.
        /// </summary>
        None = 0,

        /// <summary>
        /// The column (or row) to the right (or below) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Next = 1,

        /// <summary>
        /// The column (or row) to the left (or above) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Previous = 2
    }
}

