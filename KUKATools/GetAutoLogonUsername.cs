using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace KUKATools
{
    public class GetAutoLogonUsername
    {

                   private const string path = "c:\\kuka\\tools\\autologon\\autologon.reg";

        private const string localmachine =
            "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon]";

        private const string software = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";
        private const string validerrpath = "c:\\kuka\\tools\autologon\\restoreautologon.err";
        private const string rootpath = "c:\\restoreautologon.err";
        public  GetAutoLogonUsername()
        {

            var newline = Environment.NewLine;
            var str = String.Format("Windows Registry Editor Version 5.00{0}{0}",  newline);
            var str2 = String.Format("{0}{1}",localmachine,newline);
             try
            {
                var key1 =RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(software, true);

                var username = key1.GetValue("DefaultUserName").ToString();

                var registryKey2 =RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(software, true);

                var autoAdminLogon = registryKey2.GetValue("AutoAdminLogon").ToString();

                if (autoAdminLogon.Equals("1") && username.Length != 0)
                {
                    


                    var str3 = "\"DefaultUserName\"=\"" + username + "\"" + newline;
                    var str4 = "\"AutoAdminLogon\"=\"" + autoAdminLogon + "\"" + newline;

                    var message =
                        String.Format(
                            "Default Username is {0} \r\n AutoAdminLogin is {1}\r\n Would you like me to write this to a file?",
                            username, autoAdminLogon);
                    var result = MessageBox.Show(message, str,MessageBoxButton.YesNo,MessageBoxImage.Information,MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes)
                    {

                        if (File.Exists(path))
                        {
                            File.SetAttributes(path, FileAttributes.Normal);
                            File.Delete(path);
                        }
                        File.WriteAllText(path, String.Format("{0}{1}{2}{3}", str, str2, str3, str4));                
                    }
                 }
                else
                {
                    var fileStream2 = File.Create(validerrpath);
                    var bytes2 = new UTF8Encoding(true).GetBytes("no correct autologonuser info defined in registry");
                    fileStream2.Write(bytes2, 0, bytes2.Length);
                    fileStream2.Close();
                }
            }
            catch
            {
                try
                {
                    var fileStream2 = File.Create(validerrpath);
                    var bytes2 =
                        new UTF8Encoding(true).GetBytes("not able to read autologon info before sysprep command");
                    fileStream2.Write(bytes2, 0, bytes2.Length);
                    fileStream2.Close();
                    MessageBox.Show("KUKA tools autologon folder not found. Error Written to " + rootpath);

                }
                catch
                {
                    var fileStream2 = File.Create(rootpath);
                    var bytes2 = new UTF8Encoding(true).GetBytes("kuka tools autologon folder not found");
                    fileStream2.Write(bytes2, 0, bytes2.Length);
                    fileStream2.Close();

                    MessageBox.Show("KUKA tools autologon folder not found. Error Written to "+rootpath);
                }
        }
        }
    }
}