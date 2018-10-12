using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Approva.QA.Common;
using Microsoft.Win32;

namespace ModifyRegistry
{
   
    class RegisitryCRUD
    {
        string _regPath = null;
        string _regValue = null;

        public bool addRegistry()
        {
            bool added = false;


            return added;
        }

        public bool modifiyRegistry(string _regPath, string _regValue)
        {
            bool modified = false;

            try
            {
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Company\\Compfolder", true);
                if (myKey != null)
                {
                    myKey.SetValue(_regValue, "1", RegistryValueKind.String);
                    
                    myKey.Close();
                }
            }
            catch (Exception ex)
            {

                
            }

            return modified;
        }

        public string readRegistryValue(string _regPath, string _keyName)
        {
            string _regValue = null;

            try
            {
                RegistryKey regKey = Registry.LocalMachine;
                regKey = regKey.OpenSubKey("@"+_regPath);

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
