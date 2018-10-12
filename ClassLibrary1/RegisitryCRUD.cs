using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Approva.QA.Common;
using Microsoft.Win32;

namespace RegOps
{

    public class RegisitryCRUD
    {
        string _regHive = null;
        string _regPath = null;
        string _regKey = null;
        int _regvalue = 0;

        public RegisitryCRUD(string _regHive, string _regPath, string _regKey, int _regvalue)
        {
            this._regHive = _regHive;
            this._regPath = _regPath;
            this._regKey = _regKey;
            this._regvalue = _regvalue;
        }

        public RegisitryCRUD()
        {
        }

        public bool addRegistry()
        {
            bool added = false;


            return added;
        }


        public string getInstallationFolderPath(string _applicationType)
        {
            string _path = null;
            RegistryKey key = null;

            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                switch (_applicationType.ToUpper())
                {
                    case "IRC":
                        key = hklm.OpenSubKey(@"SOFTWARE\Approva\BizRights");
                        break;
                    case "CM":
                        key = hklm.OpenSubKey(@"SOFTWARE\Approva\CertificationManager");
                        break;
                    default:
                        throw new Exception("applicaiton tyep is incoorect");
                }

            try
            {
                if (key != null)
                {

                    _path = key.GetValue("InstallPath").ToString();
                    Logger.WriteMessage(key.GetValue("InstallPath").ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error IRC Application registry not found");
                Logger.WriteError(ex.Message);
            }
            return _path;
        }



        public void createRegistryKey(string _regHive, string _regPath, string _subkey, object _value, string regValueKind)
        {
            RegistryKey regKey = null;
            switch (_regHive)
            {
                case "HKEY_LOCAL_MACHINE":
                    regKey = Registry.LocalMachine;
                    break;
                case "HKEY_CURRENT_USER":
                    regKey = Registry.CurrentUser;
                    break;
                case "HKEY_USERS":
                    regKey = Registry.Users;
                    break;
                default:
                    throw new System.InvalidOperationException("parameter Registry->regHive must be either \"HKEY_LOCAL_MACHINE\" or \"HKEY_CURRENT_USER\" or \"HKEY_USERS\" in the App Security xml");
            }

            try
            {
                regKey = regKey.CreateSubKey(_regPath);
                switch (regValueKind)
                {
                    case "DWORD":
                        regKey.SetValue(_subkey, _value, RegistryValueKind.DWord);                        
                        break;
                    default:
                        regKey.SetValue(_subkey, _value);
                        break;
                }
                Logger.WriteMessage("Registry for " + _regHive + "\\" + _regPath + " of subkey '" + _subkey + "'" + " with Value " + _value + " created successfully");
            }
            catch (Exception ex)
            {
                Logger.WriteMessage("Error for creating Registry :" + _regHive + "\\" + _regPath + " of subkey '" + _subkey + "'" + " with Value " + _value);                
                Logger.WriteError(ex.Message);
            }
            finally
            {
                regKey.Close();
            }
        }

        public string readRegistryValue(string _regPath, string _keyName)
        {
            string _regValue = null;
            try
            {
                RegistryKey regKey = Registry.LocalMachine;
                regKey = regKey.OpenSubKey("@" + _regPath);

                if (regKey != null)
                {
                    _regValue = regKey.GetValue(_keyName).ToString();
                    return _regValue;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error while accessing Registry");
                Logger.WriteError(ex.Message);
                return null;
            }
        }

        public bool _regBackUp()
        {
            bool _backedup = false;

            return _backedup;

        }

    }
}
