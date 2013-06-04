using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Windows.Input;
using ZedGraph;
using miRobotEditor.Commands;
using miRobotEditor.ViewModel;
using System.Drawing;
using System.Collections.ObjectModel;
namespace miRobotEditor.Language_Specific
{
    public delegate void VariableEventHandler(object sender, VariableEventArgs e);
    
    public class KUKAVariableViewerViewModel:ViewModelBase
    {
        public KUKAVariableViewerViewModel()
        {
            CreateGraph(Graph);
        }
        #region ViewModel Properties

        private ZedGraphControl _graph = new ZedGraphControl();
        public ZedGraphControl Graph { get { return _graph; } set { _graph = value;RaisePropertyChanged("Graph"); } }

        private VariableAdder _adder = new VariableAdder();
        public VariableAdder Adder { get { return _adder; } set { _adder = value;RaisePropertyChanged("Adder"); } }

        private string _connectionState;
        public string ConnectionStateString{get { return _connectionState; }set { _connectionState = value;RaisePropertyChanged("ConnectionStateString"); }}

        private string _variableName;
        public string VariableName { get { return _variableName; } set { _variableName = value;RaisePropertyChanged("VariableName"); } }
        private bool _ipAddressEnabled;
        public bool IPAddressEnabled{get { return _ipAddressEnabled; }set { _ipAddressEnabled = value;RaisePropertyChanged("IPAddressEnabled"); }}

        private bool _connectionButtonEnabled;
        public bool ConnectionButtonEnabled{get { return _connectionButtonEnabled; }set { _connectionButtonEnabled = value;RaisePropertyChanged("ConnectionButtonEnabled"); }}

        private string _connectionButtonText = "_Connect";
        public string ConnectionButtonText{get { return _connectionButtonText; }set { _connectionButtonText = value;RaisePropertyChanged("ConnectionButtonText"); }}

        private bool _addVariableEnabled;
        public bool AddVariableEnabled{get { return _addVariableEnabled; }set { _addVariableEnabled = value;RaisePropertyChanged("AddVariableEnabled"); }}

        private string _ipAddressString = "192.168.48.128";
        public string IPAddressString{get { return _ipAddressString; }set { _ipAddressString=value;RaisePropertyChanged("IPAddressString"); }}
        #endregion


        #region Commands
        private RelayCommand _playCommand;

        public ICommand ShowAboutCommand
        {
            get { return _playCommand ?? (_playCommand = new RelayCommand(p => Play(p), p => true)); }
        }
        #endregion
        #region "Members"

        /// <summary>
        /// Internal bool for tracking connections
        /// </summary>
        private bool _isConnected;
        public bool IsConnected{get { return _isConnected; }set { _isConnected = value;RaisePropertyChanged("IsConnected"); }}
        private const int ReadBufferSize = 255;
        private const int PortNum = 10000;
        private TcpClient _client;
        private readonly byte[] _readBuffer = new byte[ReadBufferSize];
        #endregion

        #region "Enums"
        private enum ConnectionState { Connected, Disconnected };

        #endregion
        #region "Delegates"
        delegate void UpdateVariableEventListDelegate();
        #endregion

        private readonly ObservableCollection<Variable> _variables = new ObservableCollection<Variable>();
        private ReadOnlyObservableCollection<Variable> _readOnlyVariables;

        public ReadOnlyObservableCollection<Variable> Variables
        {
            get
            {
                return _readOnlyVariables ??
                       (_readOnlyVariables = new ReadOnlyObservableCollection<Variable>(_variables));
            }
        }


        enum command { CONNECTED, DISCONNECTED, CHAT, REFUSED, GET, SET, NONE };
        #region Connection

       
        /// <summary>
        /// Callback function for TcpClient.GetStream.Begin to get an asynchronous read
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        private void DoRead(IAsyncResult ar)
        {
            try
            {
                // Finish asynchronous read into readBuffer and return number of bytes read.
                int BytesRead = _client.GetStream().EndRead(ar);
                if (BytesRead < 1)
                {
                    // if no bytes were read server has close.  Disable input window.
                    MarkAsDisconnected();
                    return;
                }
                // Convert the byte array the message was saved into, minus two for the
                // Chr(13) and Chr(10)
                string strMessage = System.Text.Encoding.ASCII.GetString(_readBuffer, 0, BytesRead - 2);
                ProcessCommands(strMessage);
                // Start a new asynchronous read into readBuffer.
                _client.GetStream().BeginRead(_readBuffer, 0, ReadBufferSize, new AsyncCallback(DoRead), null);

            }
            catch (InvalidOperationException e)
            {
                MessageViewModel.AddError("KUKAVariableViewModel DoRead",e);
                MarkAsDisconnected();
            }

            catch (IOException ioe)
            {
               MessageViewModel.AddError("KUKAVariableViewModel DoRead",ioe);
                MarkAsDisconnected();
                AttemptDisconnect();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
               MessageViewModel.AddError("KUKAVariableViewModel DoRead",e);
                MarkAsDisconnected();
                AttemptDisconnect();
            }
            catch (Exception e)
            {
              MessageViewModel.AddError("KUKAVariableViewModel DoRead",e);
                MarkAsDisconnected();
                AttemptDisconnect();
                throw;
            }

        }



        /// <summary>
        /// Attempt to connect to server.
        /// </summary>
        private void AttemptConnect()
        {

            var host = System.Net.Dns.GetHostName();
            var ip = System.Net.Dns.GetHostEntry(host);
            Console.WriteLine(ip.AddressList[0].ToString());
            SendData(String.Format("{0}|{1}{2}", "CONNECT", System.Net.Dns.GetHostName(), ip.AddressList[0]));

        }


         /// <summary>
        /// Uses a StreamWriter to send the message to the server
        /// </summary>
        /// <param name="data"></param>       
        private void SendData(string data)
        {
            var writer = new StreamWriter(_client.GetStream());
            writer.Write(data + (char)13);
            writer.Flush();

        }
        /// <summary>
        /// Attempt disconnect from server.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
        private void AttemptDisconnect()
        {
            lock (this)
            {
                try
                {
                    var host = System.Net.Dns.GetHostName();
                    var ip = System.Net.Dns.GetHostEntry(host);
                    Console.WriteLine(ip.AddressList[0].ToString());
                    SendData(String.Format("{0}|{1}{2}", "DISCONNECT", System.Net.Dns.GetHostName(), ip.AddressList[0].ToString()));
                }
                catch (InvalidOperationException ioe)
                {
                    //Host Disconnected
                    MessageViewModel.AddError("KUKAVariableViewerViewModel AttempDisconnect",ioe);
                }
            }
        }

        #endregion


        /// <summary>
        /// Connect to robot
        /// </summary>
        /// <param name="address">IP Address of robot</param>
        private void Connect(string address)
        {
            try
            {
                ConnectionStateString = "Connecting...";

                // The TcpClient is a subclass of Socket, providing higher level 
                // functionality like streaming.
                _client = new TcpClient(address, PortNum);

                // Start an asynchronous read invoking DoRead to avoid lagging the user
                // interface.
                _client.GetStream().BeginRead(_readBuffer, 0, ReadBufferSize, new AsyncCallback(DoRead), null);
                // Make sure the window is showing before popping up connection dialog.               
                AttemptConnect();
            }
            catch (SocketException e)
            {
                MessageViewModel.AddError("KUKAVariableViewModel Connect",e);
                MarkAsDisconnected();
                throw;
            }
            catch (Exception e)
            {
                MessageViewModel.AddError("KUKAVariableViewModel Connect",e);
                MarkAsDisconnected();
                throw;
            }
        }

         private delegate void UpdateEnabled(bool enabled);
        void ConnectionChanged(bool isConnected)
        {
            AddVariableEnabled = isConnected;
        }


     
        /// <summary>
        /// Enable/Disable txtIPAddress
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableIpAddressState(bool enabled)
        {
            IPAddressEnabled = enabled;          
        }

       
        private void UpdateBtnConnection(bool enabled)
        {
            ConnectionButtonEnabled = enabled;
        }
        /// <summary>
        /// When the server disconnects, prevent further chat messages from being sent.
        /// </summary>
        private void MarkAsDisconnected()
        {
            ConnectionStateString = "Disconnected";
            IsConnected = false;
            AddVariableEnabled = false;
        }

        private static command GetCommand(string data)
        {
            switch (data)
            {
                case "CONNECTED": return command.CONNECTED;
                case "DISCONNECTED": return command.DISCONNECTED;
                case "REFUSED": return command.REFUSED;
                case "GET": return command.GET;
                case "SET": return command.SET;
            }
            return command.NONE;
        }

        /// <summary>
        ///  Process the command received from the server, and take appropriate action.
        /// </summary>
        /// <param name="strMessage"></param>
        private void ProcessCommands(string strMessage)
        {
            // Message parts are divided by "|"  Break the string into an array accordingly.
            var dataArray = strMessage.Split((char)124);
            
            // dataArray(0) is the command.
            switch (GetCommand(dataArray[0]))
            {
                case command.CONNECTED:
                    // Server acknowledged login.
//                    writeDebug("Has Connected to the robot" + (char)13 + (char)10);
                    EnableIpAddressState(false);
                    ConnectionButtonText = "_Disconnect";                    
                    ConnectionStateString = "Connected";
                    IsConnected = true;
                    break;
                case command.DISCONNECTED:
                    ConnectionButtonText = "_Connect";
                    MarkAsDisconnected();
                    IsConnected = false;
                    break;
                case command.CHAT:
                    // Received chat message, display it.
                   // writeDebug(dataArray[1] + (char)13 + (char)10);
                    break;
                case command.REFUSED:
                    // Server refused login with this user name, try to log in with another.
                    ConnectionStateString = "Connection Refused, Try Again";
                    AttemptDisconnect();
                    AttemptConnect();

                    break;
                case command.GET:
                    UpdateVariable(dataArray[1], dataArray[2]);
                    break;
                default:
                    throw new InvalidOperationException(dataArray[0]);
            }

            AddVariableEnabled = IsConnected;
        }



        /// <summary>
        /// Connect/Disconnect to robot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void change_connection(object sender, EventArgs e)
        {
            switch(ConnectionButtonText.ToUpper())           
            {
                case "_CONNECT":


                    try
                    {
                        //TODO Change Connect Method to accept ipaddress
                        System.Net.IPAddress ipaddress;
                        if (IPAddressString.ToUpper(System.Globalization.CultureInfo.InvariantCulture).Trim() == "LOCALHOST") IPAddressString = "127.0.0.1";
                        ipaddress = System.Net.IPAddress.Parse(IPAddressString);
                        Connect(IPAddressString);
                    }
                    catch (FormatException ex)
                    {
                        MessageViewModel.AddError("KUKAVariableViewerViewModel Change_Connection",ex);
                        MessageBox.Show( @"Invalid IP Address, Please Enter a valid format", @"Invalid IP Address");
                    }
                    catch (Exception ex)
                    {
                        MessageViewModel.AddError("KUKAVariableViewerViewModel Change_Connection",ex);
                    }
                    break;
                case "_DISCONNECT":
                    AttemptDisconnect();
                    break;
            }
        }

        public void UpdateVariable(string name, string value)
        {
//            VariableWindow.Update(name, value);
        }


        private void Add_Variable(object sender, VariableEventArgs e)
        {
            //TODO: Check to see if variable exists
            _variables.Add(e.Variable);
          /*
            variableAdder.Clear();

            var v = VariableWindow.Variables[VariableWindow.GetVariable(e.Variable.VarName)];
            var p = new PointPair();

            v.LineItem = graph.GraphPane.AddCurve(v.VarName, new PointPairList(), CreateRandomColor());
           * */
        }
        public Color CreateRandomColor()
        {
            var r = new Random();
            return Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
        }
        private bool VariableExists(string name)
        {
            return true;
        }


    


        /// <summary>
        /// Get value of variable from robot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetVariable(object sender, VariableInfoEventArgs e)
        {
            SendData(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GET|{0}", e.Name));
        }

      

        //// Build the Chart
        private void CreateGraph(ZedGraphControl zgc)
        {
            // get a reference to the GraphPane
            var myPane = zgc.GraphPane;
            // Set the Titles
            //myPane.Title.Text = "My Test Graph\n(For CodeProject Sample)";
            myPane.XAxis.Title.Text = "Value";
            myPane.YAxis.Title.Text = "Sample";

            //  return;
            //     myPane.CurveList.Clear();

            for (var i = 0; i < _variables.Count; i++)
            {
                if (_variables[i].PointList != null)
                    myPane.CurveList[i].Points = _variables[i].PointList;
            }

            myPane.Legend.Position = LegendPos.Bottom;
            
            
                    // Add a background gradient fill to the axis frame
                    myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 210), -45F);
            
                    // Add a caption and an arrow
                    var myText = new TextObj("Interesting\nPoint", 230F, 70F);
                    myText.FontSpec.FontColor = Color.Red;
                    myText.Location.AlignH = AlignH.Center;
                    myText.Location.AlignV = AlignV.Top;
                    myPane.GraphObjList.Add(myText);
                    var myArrow = new ArrowObj(Color.Red, 12F, 230F, 70F, 280F, 55F);
                    myPane.GraphObjList.Add(myArrow);
            
                  // Tell ZedGraph to reconfigure the axes since the data have changed

          //  if (btnPlay.Text == @"Pause")
                zgc.AxisChange();

        }

        void Play(object param)
        {
          //  switch (btnPlay.ImageIndex)
          //  {
          //      case 0:
          //          btnPlay.ImageIndex = 1;
          //          btnPlay.Text = @"Pause";
          //          break;
          //      case 1:
          //          btnPlay.ImageIndex = 0;
          //          btnPlay.Text = @"Play";
          //          break;
          //
          //  } 
        }
       
        /// <summary>
        /// Variable Class
        /// </summary>       
        public class Variable
        {
            public int Interval
            {
                get { return _mTimer.Interval; }
                set { _mTimer.Interval = value; }
            }

            public string VarName
            {
                get { return _varname; }
                set { _varname = value; }
            }

            public string Value
            {
                get { return _varValue; }              
            }

           

            /// <summary>
            /// Timer for monitoring variable
            /// </summary>
            private readonly Timer _mTimer = new Timer { Enabled = true };
           
            
            /// <summary>
            /// Name of variable
            /// </summary>
            private string _varname = string.Empty;

            /// <summary>
            /// Value of variable
            /// </summary>
            private string _varValue = string.Empty;

            /// <summary>
            /// Get variable state
            /// </summary>
            public event VariableEventHandler GetVarState;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="interval">Timespan for polling value of variable</param>
            /// <param name="varname">Variable name</param>
            public Variable(int interval, string varname)
            {
                _mTimer.Interval = interval;
                _mTimer.Tick += mTimer_Tick;
                _varname = varname;
                Initialize();
            }

            /// <summary>
            /// Constructor for dummy variable used for threading
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public Variable(string name, string value)
            {
                _varname = name;
                _varValue = value;
                
            }

            public PointPairList PointList
            {
                get
                {
                    var result = new PointPairList();
                    for (int i = 0; i < _values.Count ; i++)
                    {
                        result.Add(_values[i]);
                    }
                    return result;
                }


            }
            private readonly List<PointPair> _values = new List<PointPair>();
            [CLSCompliant(true)]
            public List<PointPair> Values
            {
                get { return _values; }
            }
            public void Initialize()
            {
               
            }

          
            /// <summary>
            /// Start monitoring variable
            /// </summary>
            public void Start()
            {
                _mTimer.Start();
            }

            /// <summary>
            /// Stop monitoring variable
            /// </summary>
            public void Stop()
            {
                _mTimer.Stop();
            }


            public void UpdateVariable(string name, string value)
            {
                Int32 plotterValue = -1;
                if (name == _varname)
                    _varValue = value;
                // Determine of variable is bool
                try
                {
                    plotterValue = Convert.ToInt32(value);
                }
                catch (FormatException ex)
                {
                    switch (_varValue.ToLower())
                    {
                        case "true": plotterValue = 1; break;
                        case "false": plotterValue = 0; break;
                        default:
                            Console.WriteLine(ex.ToString());
                            break;
                    }
                }
                finally
                {
                  _values.Add(new PointPair(_values.Count, plotterValue));
                }
            }


            void mTimer_Tick(object sender, EventArgs e)
            {                
                GetVarState(this, new VariableEventArgs(_varname));
            }
            

            public void Dispose()
            {
                Dispose(true);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                _mTimer.Dispose();

                
            }

                public LineItem LineItem { get; set; }
        }


    }
    public class VariableEventArgs:EventArgs
{ 
    public VariableEventArgs()
    {}
    public VariableEventArgs(string name)
    {_name = name;}
    private string _name = string.Empty;   
    public string Name
    {
        get{return _name;}
        set{_name = value;}
    }
        private readonly KUKAVariableViewerViewModel.Variable _variable;
        public KUKAVariableViewerViewModel.Variable Variable
        {
            get { return _variable; }
        }

        public VariableEventArgs(KUKAVariableViewerViewModel.Variable variable)
        {
            _variable = variable;
        }
    }    
    public delegate void AddVariableEventHandler(object sender,VariableEventArgs e);
    public class VariableAdder:ViewModelBase
    {

         public event AddVariableEventHandler AddVariableClick;
        private string _text;
        public string Text{get { return _text; }set { _text = value;RaisePropertyChanged("Text"); }}

        private int _interval;
        public int Interval{get { return _interval; }set { _interval = value;RaisePropertyChanged("Interval"); }}



        void AddVariable(object param)
        {
            if ((!String.IsNullOrEmpty(Text))&&(AddVariableClick!=null))
                AddVariableClick(this,new VariableEventArgs(new KUKAVariableViewerViewModel.Variable(Interval,Text)));
        }

        public void Clear()
        {
            
        }
    }
     public class VariableInfoEventArgs : EventArgs
    {
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public VariableInfoEventArgs(string name)
        {
            _name = name;
        }

    }
}

