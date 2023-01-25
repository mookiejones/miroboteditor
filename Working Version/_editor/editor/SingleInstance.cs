using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    public static class SingleInstance<TApplication> where TApplication : Application, ISingleInstanceApp
    {
        private const string Delimiter = ":";
        private const string ChannelNameSuffix = "SingeInstanceIPCChannel";
        private const string RemoteServiceName = "SingleInstanceApplicationService";
        private const string IpcProtocol = "ipc://";
        private static Mutex _singleInstanceMutex;
// ReSharper disable once StaticFieldInGenericType
        private static IpcServerChannel _channel;
        public static IList<string> CommandLineArgs { get; private set; }

        public static bool InitializeAsFirstInstance(string uniqueName)
        {
            CommandLineArgs = GetCommandLineArgs(uniqueName);
            var text = uniqueName + Environment.UserName;
            var channelName = text + ":" + "SingeInstanceIPCChannel";
            var flag = false;
            _singleInstanceMutex = new Mutex(true, text, out flag);
            if (flag)
            {
                CreateRemoteService(channelName);
            }
            else
            {
                SignalFirstInstance(channelName, CommandLineArgs);
            }
            return flag;
        }

        public static void Cleanup()
        {
            if (_singleInstanceMutex != null)
            {
                _singleInstanceMutex.Close();
                _singleInstanceMutex = null;
            }
            if (_channel != null)
            {
                ChannelServices.UnregisterChannel(_channel);
                _channel = null;
            }
        }

        private static IList<string> GetCommandLineArgs(string uniqueApplicationName)
        {
            string[] array = null;
            if (AppDomain.CurrentDomain.ActivationContext == null)
            {
                array = Environment.GetCommandLineArgs();
            }
            else
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    uniqueApplicationName);
                var path2 = Path.Combine(path, "cmdline.txt");
                if (File.Exists(path2))
                {
                    try
                    {
                        using (TextReader textReader = new StreamReader(path2, Encoding.Unicode))
                        {
                            array = NativeMethods.CommandLineToArgvW(textReader.ReadToEnd());
                        }
                        File.Delete(path2);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            if (array == null)
            {
                array = new string[0];
            }
            return new List<string>(array);
        }

        [Localizable(false)]
        private static void CreateRemoteService(string channelName)
        {
            var sinkProvider = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = TypeFilterLevel.Full
            };
            IDictionary dictionary = new Dictionary<string, string>();
            dictionary["name"] = channelName;
            dictionary["portName"] = channelName;
            dictionary["exclusiveAddressUse"] = "false";
            _channel = new IpcServerChannel(dictionary, sinkProvider);
            ChannelServices.RegisterChannel(_channel, true);
            var obj = new IpcRemoteService();
            RemotingServices.Marshal(obj, "SingleInstanceApplicationService");
        }

        [Localizable(false)]
        private static void SignalFirstInstance(string channelName, IList<string> args)
        {
            var chnl = new IpcClientChannel();
            ChannelServices.RegisterChannel(chnl, true);
            var url = "ipc://" + channelName + "/SingleInstanceApplicationService";
            var ipcRemoteService = (IpcRemoteService) RemotingServices.Connect(typeof (IpcRemoteService), url);
            if (ipcRemoteService != null)
            {
                try
                {
                    ipcRemoteService.InvokeFirstInstance(args);
                }
                catch (RemotingException)
                {
                }
            }
        }

        private static object ActivateFirstInstanceCallback(object arg)
        {
            var args = arg as IList<string>;
            ActivateFirstInstance(args);
            return null;
        }

        private static void ActivateFirstInstance(IList<string> args)
        {
            if (Application.Current != null)
            {
                var tApplication = (TApplication) Application.Current;
                tApplication.SignalExternalCommandLineArgs(args);
            }
        }

        private class IpcRemoteService : MarshalByRefObject
        {
            public void InvokeFirstInstance(IList<string> args)
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new DispatcherOperationCallback(ActivateFirstInstanceCallback), args);
                }
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }
        }
    }
}