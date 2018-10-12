using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegOps;
using System.Threading;

using Approva.QA.Common;
using CopyFiles;
using XMLOps;
using CMDOps;
using System.Xml;
using SQLOps;

using System.Reflection;




namespace ApplicationSecurity
{
    class ApplicationSecurity
    {
        private static XmlDocument _appConfig = null;
        static void Main(string[] args)
        {
            string _websiteName = null;
            bool _https = false;
            string _port = null;
            string _DBconnectionString = null;
            string _application = null;
            string _driveName = null;
            string node = null;
            string _sourcePath = null;
            string _targetPath = null;
            string _fileName = null;
            XmlNodeList copyFiles = null;
            XmlNodeList regOperation = null;
            XmlNodeList sqlOperations = null;          
            XmlNodeList grouppolicyOperation = null;
            RegisitryCRUD registrycrud = null;

           
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\AppSecurity.xml") && System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\data.xml"))
            {

                _appConfig = new XmlDocument();
                _appConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\AppSecurity.xml");
                _websiteName = _appConfig.SelectSingleNode("settings/websiteName").InnerText;
                _application = _appConfig.SelectSingleNode("settings/application").InnerText;
                _driveName = _appConfig.SelectSingleNode("settings/DriveLetter").InnerText;
                _https = Convert.ToBoolean(_appConfig.SelectSingleNode("settings/https").InnerText);
                _port = _appConfig.SelectSingleNode("settings/port").InnerText;
                _DBconnectionString = _appConfig.SelectSingleNode("settings/DBconnectionString").InnerText;

                _appConfig = null;
                _appConfig = new XmlDocument();
                _appConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\data.xml");

                regOperation = _appConfig.SelectNodes("settings/RegistryOperations/Registry");
                sqlOperations = _appConfig.SelectNodes("settings/SqlOperations/Sql");          
                grouppolicyOperation = _appConfig.SelectNodes("settings/GroupPolicy/policy");

                RegisitryCRUD _registryCrud = new RegisitryCRUD();
                string _installationPath = null;
                _installationPath = _registryCrud.getInstallationFolderPath(_application);
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                try
                {
                    if (_installationPath.Equals(null)){}
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("" + _application.ToUpper() + " IS NOT INSTALLED ON SERVER. Exiting from Utility");
                    Logger.WriteMessage("" + _application.ToUpper() + " IS NOT INSTALLED ON SERVER. Exiting from Utility");
                    System.Environment.Exit(0);
                }


                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("                 -----------------------------------------------");
                Console.WriteLine("                     APPLICATION SECURITY VERSION :" + Assembly.GetExecutingAssembly().GetName().Version);
                Logger.WriteMessage("                   APPLICATION SECURITY VERSION :" + Assembly.GetExecutingAssembly().GetName().Version);
                Console.WriteLine("                 -----------------------------------------------");

                Console.ForegroundColor = ConsoleColor.Cyan;
                /**
                * -------------------------------------------------------------------------
                * Web-content is on a non-system partition
                * ----------------------------------------------------------------------
                */
              
                string _wwwrootPath = null;
                string _binDirectory = null;
                string _binDirUnderSetting = null;

                _wwwrootPath = _driveName + @":\wwwroot";
                _binDirectory = _wwwrootPath + "\\bin";
                _binDirUnderSetting = _installationPath + "\\Settings\\bin";

                if (_application.ToUpper().Equals("IRC"))
                {
                    Console.WriteLine(" ----------------------------------------------------------------------");
                    Logger.WriteMessage(" Start - Web-content is on a non-system partition");
                    Console.WriteLine(" Start - Web-content is on a non-system partition");
                    Console.WriteLine(" ----------------------------------------------------------------------");

                    if (!System.IO.Directory.Exists(_wwwrootPath))
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(_wwwrootPath);
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteError("Error while creating driectory on " + _wwwrootPath);
                            Logger.WriteMessage(ex.Message);
                        }
                    }
                    try
                    {
                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories("C:\\inetpub\\wwwroot", "*", SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace("C:\\inetpub\\wwwroot", _wwwrootPath));
                        System.Threading.Thread.Sleep(10000);
                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles("C:\\inetpub\\wwwroot", "*.*", SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace("C:\\inetpub\\wwwroot", _wwwrootPath), true);
                        System.Threading.Thread.Sleep(10000);
                    }
                    catch (Exception ex)
                    {

                        Logger.WriteError("Error while coping files in D drive");
                        Logger.WriteMessage(ex.Message); ;
                    }

                    Logger.WriteMessage("End - Web-content is on a non-system partition");
                    Console.WriteLine(" END - Web-content is on a non-system partition");
                    Console.WriteLine(" ----------------------------------------------------------------------");

                    try
                    {
                        System.IO.Directory.CreateDirectory(_binDirectory);
                        System.Threading.Thread.Sleep(2000);
                        Logger.WriteMessage("" + _binDirectory + " directory is created");
                        Console.WriteLine("" + _binDirectory + " directory is created");
                        Console.WriteLine(" ----------------------------------------------------------------------");

                    }
                    catch (Exception ex)
                    {
                        Logger.WriteError("Error while creating folder at " + _binDirectory);
                        Logger.WriteMessage(ex.Message);
                    }

                    try
                    {
                        System.IO.Directory.CreateDirectory(_binDirUnderSetting);
                        System.Threading.Thread.Sleep(2000);
                        Logger.WriteMessage("" + _binDirUnderSetting + " directory is created");
                        Console.WriteLine("" + _binDirUnderSetting + " directory is created");
                        Console.WriteLine(" ----------------------------------------------------------------------");

                    }
                    catch (Exception ex)
                    {
                        Logger.WriteError("Error while creating folder at " + _binDirUnderSetting);
                        Logger.WriteMessage(ex.Message);
                    }
                }

                switch (_application.ToUpper())
                {
                    case "IRC":
                        _sourcePath = _installationPath + @"\BizRightsPresentation\bin";
                        break;
                    case "CM":
                        _sourcePath = _installationPath + @"\presentation\bin";
                        break;
                    default:
                        throw new Exception("Applicaton type is in incorrect format");
                }
                _fileName = "Approva.Presentation.Framework.HttpModule.dll";
                try
                {
                    CopyDLL._CopyFromLocation(_sourcePath, _binDirectory, _fileName);
                    System.Threading.Thread.Sleep(3000);
                }
                catch (Exception ex)
                {
                    Logger.WriteError("Error while copying " + _fileName);
                    Logger.WriteMessage(ex.Message); ;
                }
                _sourcePath = null;
                _fileName = null;


                //----------------------------------------------------------------------

                /**
                * -------------------------------------------------------------------------
                * Copy the Approva.Presentation.Framework.HttpModule.dll from the [IRC InstallPath]\BizRightsPresentation\bin to 3 locations:
                * [IRC Install path]\Core\bin
                * [IRC Install path]\Adapters\TMonitor\bin
                * [IRC Install path]\BRPublisher\bin
                * ----------------------------------------------------------------------
                */
                switch (_application.ToUpper())
                {
                    case "IRC":
                        copyFiles = _appConfig.SelectNodes("settings/IRCcopyFiles");
                        break;
                    case "CM":
                        copyFiles = _appConfig.SelectNodes("settings/CMcopyFiles");
                        break;
                    default:
                        throw new Exception("Applicaton type is in incorrect format");
                }

                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Copying the Approva.Presentation.Framework.HttpModule.dll from the [IRC InstallPath]\BizRightsPresentation\bin to 4 locations:");
                Console.WriteLine(@" Copying the Approva.Presentation.Framework.HttpModule.dll from the[IRC InstallPath]\\BizRightsPresentation\bin to 4 locations:");
               

                foreach (XmlNode itemCopyFiles in copyFiles)
                {
                    // foreach (XmlNode filepath in copyFiles)
                    //{
                    Console.WriteLine("------------------------------------------------");
                    _sourcePath = itemCopyFiles.SelectSingleNode("SourceFile").Attributes["path"].Value;
                    _fileName = itemCopyFiles.SelectSingleNode("SourceFile").Attributes["fileName"].Value;
                    _sourcePath = _installationPath + _sourcePath;
                    XmlNodeList _locationlist = itemCopyFiles.SelectNodes("SourceFile/location");
                    foreach (XmlNode item in _locationlist)
                    {
                        _targetPath = item.InnerText;
                        _targetPath = _installationPath + _targetPath;
                        CopyDLL._CopyFromLocation(_sourcePath, _targetPath, _fileName);
                        Logger.WriteMessage(@"Copied to:"+ _targetPath);
                        Console.WriteLine(@"Copied to:" + _targetPath);                        
                        System.Threading.Thread.Sleep(500);
                        _targetPath = null;
                    }
                    _sourcePath = null;
                    //}
                }

                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"WhiteHat - Fixing the information leakage vulnerability");
                Console.WriteLine(@" WhiteHat - Fixing the information leakage vulnerability");
                bool iswebConfigPresent = System.IO.File.Exists(_wwwrootPath + "\\web.config");

                if (_application.ToUpper().Equals("IRC"))
                {
                    if (iswebConfigPresent)
                    {


                        Console.WriteLine("Web.config is already present & backedup with 'OldWeb.config' at " + _wwwrootPath);
                        try
                        {
                            File.Copy(_wwwrootPath + "\\web.config", _wwwrootPath + "\\OldWeb.config", true);
                            File.Delete(_wwwrootPath + "\\web.config");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Error while renaming Web.config");
                            Logger.WriteMessage("Error while renaming Web.config");
                            throw;
                        }
                    }

                    // string webConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\web.config";
                    try
                    {
                        CopyDLL._CopyFromLocation(AppDomain.CurrentDomain.BaseDirectory, _wwwrootPath, "web.config");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteMessage("Problem while copying web.config file ");
                        Console.WriteLine("Problem while copying web.config file");
                        Logger.WriteMessage(ex.Message);
                    }
                }


                // Remove or comment out the following node from these paths to unregister"AntiHttpCrossSiteForgeryRequestModule" managed HTTP module.               * 
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Remove/comment out the following node from these paths to unregister AntiHttpCrossSiteForgeryRequestModule managed HTTP module from Bizrightspresentation/bin");
                Console.WriteLine(@" Remove/comment out the following node from these paths to unregister AntiHttpCrossSiteForgeryRequestModule managed HTTP module from Bizrightspresentation/bin");
                _fileName = "web.config";
                switch (_application.ToUpper())
                {
                    case "IRC":
                        _sourcePath = _installationPath + "\\BizrightsPresentation";
                        File.Copy(_sourcePath + "\\web.config", _wwwrootPath + "\\Old_IRC_Web.config", true);
                        break;
                    case "CM":
                        _sourcePath = _installationPath + "\\presentation";
                        File.Copy(_sourcePath + "\\web.config", _wwwrootPath + "\\Old_CM_Web.config", true);
                        break;
                    default:
                        throw new Exception("Application type not correct");
                }
                string _node = @"add[@name='AntiHttpCrossSiteForgeryRequestModule']";
                string _refNode = @"/configuration/system.webServer/modules/";
                XMLCRUD._commentInWebConfigXML(_sourcePath, _fileName, _refNode, _node, true);

                _node = null;
                _refNode = null;


                /*
                 * --------------------------------------------------------------------------------------
                 * Cookie security: cookie not sent over SSL
                 * ------------------------------------------------------------------------------------
                */
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Cookie security: cookie not sent over SSL");
                Console.WriteLine(@" Cookie security: cookie not sent over SSL");
                string _attribute = null;
                string _attributeValue = null;

                if (_application.ToUpper().Equals("IRC") && _https)
                {

                    _attribute = "cookieRequireSSL";
                    _attributeValue = "true";
                    _refNode = "roleManager";
                    XMLCRUD.addAttributeToExistingNode(_sourcePath, _fileName, _attribute, _attributeValue, _refNode);

                    _attribute = "null";
                    _attributeValue = "null";
                    _refNode = "null";

                    _attribute = "requireSSL";
                    _refNode = "forms";
                    _attributeValue = "true";
                    XMLCRUD.addAttributeToExistingNode(_sourcePath, _fileName, _attribute, _attributeValue, _refNode);

                    _attribute = "null";
                    _attributeValue = "null";
                    _refNode = "null";

                    _attribute = "requireSSL";
                    _refNode = "httpCookies";
                    _attributeValue = "true";
                    XMLCRUD.addAttributeToExistingNode(_sourcePath, _fileName, _attribute, _attributeValue, _refNode);

                    _attribute = "null";
                    _attributeValue = "null";
                    _refNode = "null";
                }


                /*
                 * -------------------------------------------------------------------------
                 * CIS - CAT hardening - ASP.Net configuration recommendations   
                  //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config
                 * ----------------------------------------------------------------------
                 */
                if (_application.ToUpper().Equals("IRC"))
                {
                    Console.WriteLine(" ----------------------------------------------------------------------");
                    Logger.WriteMessage(@"CIS - CAT hardening - ASP.Net configuration recommendations");
                    Console.WriteLine(@" CIS - CAT hardening - ASP.Net configuration recommendations");
                    node = "deployment";
                    CopyDLL._backupFiles("C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\Config", "machine.config");
                    XMLCRUD._modifyXML("C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\Config", "machine.config", node, "configuration/system.web", false);
                    node = null;

                    /*
                    * -------------------------------------------------------------------------
                         Ensure Handler is not granted Write and Script/Execute
                    * ----------------------------------------------------------------------
                    */

                    Console.WriteLine(" ----------------------------------------------------------------------");
                    Logger.WriteMessage(@"Ensure Handler is not granted Write and Script/Execute");
                    Console.WriteLine(@" Ensure Handler is not granted Write and Script/Execute");
                    CopyDLL._backupFiles("C:\\Windows\\system32\\inetsrv\\config", "applicationHost.config");
                    XmlDocument _appHostConfig = new XmlDocument();
                    try
                    {

                        try
                        {
                            _appHostConfig.Load("C:\\Windows\\system32\\inetsrv\\config\\applicationHost.config");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error while Loading Config file: " + e.Message);
                        }

                        XmlAttribute handlers = (XmlAttribute)_appHostConfig.SelectSingleNode("configuration//location//system.webServer//handlers/@accessPolicy");
                        String _policy = handlers.InnerText;
                        // Console.WriteLine("Present Handler Values: " + _policy);
                        Logger.WriteMessage("Present Handler Values: " + _policy);
                        if (_policy.ToUpper().Contains("Write"))
                        {
                            handlers.Value = "Read, Script";
                            _appHostConfig.Save("C:\\Windows\\system32\\inetsrv\\config\\applicationHost.config");
                            //   Console.WriteLine("applicationHost.config file has been modified successfully");
                            Logger.WriteMessage("applicationHost.config file has been modified successfully");
                        }
                        else
                        {
                            //  Console.WriteLine("Required Handler Values are present");
                            Logger.WriteMessage("Required Handler Values are present");

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while modifing/reading Handler in ApplicationHost.config file");
                        Logger.WriteMessage("Error while modifing/reading Handler in ApplicationHost.config file" + e.Message);
                    }
                }

               // Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Change IIS Website Physical path");
                //Console.WriteLine(@"Change IIS Website Physical path");
                node = null;
                node = "Change IIS Physical path";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                //Console.WriteLine("change physical path to " + _wwwrootPath);
                System.Threading.Thread.Sleep(3000);

              //  Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Change IIS log path");
               // Console.WriteLine(@"Change IIS log path");
                node = null;
                node = "Change IIS log path";
                string _logPath = _driveName + @":\LogFiles";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _driveName);
               // Console.WriteLine("change IIS log");
                System.Threading.Thread.Sleep(3000);


                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"IIS Hardening -Configuring host headers on all sites");
                Console.WriteLine(@" IIS Hardening -Configuring host headers on all sites ");
                // IIS Hardening -Configuring host headers on all sites
                node = "Host Header";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);

                node = null;

                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"IIS - Setting the default application pool identity to least privilege principal");
                Console.WriteLine(@" IIS - Setting the default application pool identity to least privilege principal");
                // IIS - Setting the default application pool identity to least privilege principal
                node = "Default application pool identity";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;


                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"IIS - Configuring the anonymous user identity to use the Application Pool identity");
                Console.WriteLine(@" IIS - Configuring the anonymous user identity to use the Application Pool identity");
                //IIS - Configuring the anonymous user identity to use the Application Pool identity
                node = "Anonymous user identity to use the Application Pool identity";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;


                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"IIS - Configuring the Require SSL option in the Forms authentication");
                Console.WriteLine(@" IIS - Configuring the Require SSL option in the Forms authentication");
                // IIS - Configuring the Require SSL option in the Forms authentication                
                if (_https)
                {
                    //string[] subsites = new string[] { "BizRightsCoreSettings", "BRPublisher", "CertificationManager", "Core", "IRC", "PDSService", "TMAdapter" };
                    node = "Required SSL in form authentication";
                    CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                    //foreach (string s in subsites)
                    //{
                    //    string temp = _websiteName + "/" + s;
                    //    CMDOperations.cmdRun(node, temp, _https, _port, _wwwrootPath);
                    //    temp = null;
                    //}
                    node = null;
                }

                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"IIS - Turning the Debug Off in the IIS  ");
                Console.WriteLine(@" IIS - Turning the Debug Off in the IIS ");
                //IIS - Turning the Debug Off in the IIS
                node = "Debug off in the IIS";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);

                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Adding X-Content - Type - Options ");
                Console.WriteLine(@" Adding X-Content - Type - Options ");

                //Adding X-Content - Type - Options
                if (_application.ToUpper().Equals("IRC"))
                {
                    node = "X-Content-Type-Options";
                    CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                    node = null;
                }

                //Disabling directory browsing feature of Certification Manager Site using IIS

                if (_application.ToUpper().Equals("CM"))
                {
                    Console.WriteLine(" ----------------------------------------------------------------------");
                    Logger.WriteMessage(@"Disabling directory browsing feature of Certification Manager Site");
                    node = "Directory Browsing";
                    CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                    node = null;
                }


                //Enabling Advanced IIS logging

                //Setting cookies with the HttpOnly attribute
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Setting cookies with the HttpOnly attribute is incorporated in pre shipped web config");


               // Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Setting 'forms authentication' to use cookies");
                Console.WriteLine(@" Setting 'forms authentication' to use cookies");
                //Setting 'forms authentication' to use cookies
                node = "forms Authentication to use cookies";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;

                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Ensure non-ASCII Characters in URLs are not allowed");
                Console.WriteLine(@" Ensure non-ASCII Characters in URLs are not allowed");
                //Ensure non-ASCII Characters in URLs are not allowed
                node = "Request Filter";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;

                //To configure general request-filter-Configure Double escaping (allowdoubleescaping)
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Ensure Double-Encoded requests will be rejected");
                Console.WriteLine(@"Ensure Double-Encoded requests will be rejected");
                //Ensure non-ASCII Characters in URLs are not allowed
                node = "Double Escaping";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;


                //To configure configure IIS to deny HTTP TRACE requests (Ensure 'HTTP Trace Method' is disabled)
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Ensure 'HTTP Trace Method' is disabled");
                Console.WriteLine(@"Ensure 'HTTP Trace Method' is disabled");
                //Ensure non-ASCII Characters in URLs are not allowed
                node = "HTTP TRACE";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;

                //Edit ISAPI and CGI Restrictions Settings
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Ensure 'notListedIsapisAllowed/notListedCgisAllowed' is set to false");
                Console.WriteLine(@"Ensure 'notListedIsapisAllowed/notListedCgisAllowed' is set to false");
                //Ensure non-ASCII Characters in URLs are not allowed
                node = "ISAPI_CGI Restrictions";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;

                //Machine Key Settings
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(@"Ensure 'MachineKey validation method - .Net 4.5' is configured");
                Console.WriteLine(@"Ensure 'MachineKey validation method - .Net 4.5' is configured");
                //Ensure non-ASCII Characters in URLs are not allowed
                node = "Machine Key 4.5 Validation";
                CMDOperations.cmdRun(node, _websiteName, _https, _port, _wwwrootPath);
                node = null;

                /**
               * -------------------------------------------------------------------------
               * Disabling the RC4 Cipher suites,  
               * Ensure TLS 1.2 is enabled
               * Ensure NULL Cipher Suites is disabled 
               * Ensure DES Cipher Suites is disabled,Ensure RC2 Cipher Suites is disabled
               * Ensure AES 256/256 Cipher Suite is enabled
               * ----------------------------------------------------------------------
               */
                string _regHive = null;
                string _regPath = null;
                string _regKey = null;
                int _regvalue = 0;
                string _sregvalue = null;
                string _regvalueType = null;
                string _regvalueKind = null;
                if (_application.ToUpper().Equals("IRC"))
                {
                    Console.WriteLine(" ----------------------------------------------------------------------");
                    Logger.WriteMessage(@"Disabling the RC4 Cipher suites, Ensure TLS 1.2 is enabled ");
                    Console.WriteLine(@"Disabling the RC4 Cipher suites");
                    registrycrud = null;
                    foreach (XmlNode itemReg in regOperation)
                    {
                        registrycrud = new RegisitryCRUD();
                        _regHive = itemReg.Attributes["regHive"].Value.ToString();
                        _regPath = itemReg.Attributes["regPath"].Value.ToString();
                        _regKey = itemReg.Attributes["regKey"].Value.ToString();
                        _regvalueType = itemReg.Attributes["regValueType"].Value.ToString();
                        _regvalueKind = itemReg.Attributes["regValueKind"].Value.ToString().Equals(" ") ? null : itemReg.Attributes["regValueKind"].Value.ToString();
                        if (_regvalueType.Equals("int"))
                        {
                            _regvalue = Convert.ToInt32(itemReg.Attributes["regvalue"].Value);
                            registrycrud.createRegistryKey(_regHive, _regPath, _regKey, _regvalue, _regvalueKind);
                        }
                        else
                        {
                            _sregvalue = itemReg.Attributes["regvalue"].Value.ToString();
                            registrycrud.createRegistryKey(_regHive, _regPath, _regKey, _sregvalue, _regvalueKind);
                        }
                        registrycrud = null;
                    }


                    /**
                    * -------------------------------------------------------------------------
                    * Group Policy Operations
                    * ----------------------------------------------------------------------
                    */
                    Console.WriteLine(" ----------------------------------------------------------------------");
                    Logger.WriteMessage(@"Group Policy Operations");
                    Console.WriteLine(@"Group Policy Operations");

                    foreach (XmlNode policy in grouppolicyOperation)
                    {
                        string _policyName = policy.Attributes["name"].Value.ToString();
                        string _policyValue = policy.Attributes["value"].Value.ToString();
                        _regHive = "HKEY_LOCAL_MACHINE";
                        _regPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
                        _regKey = null;
                        _regvalue = 0;

                        switch (_policyName)
                        {
                            case "User Account Control: Behavior of the elevation prompt for administrators inAdmin Approval Mode":
                                _regKey = "ConsentPromptBehaviorAdmin";
                                _regvalue = 5;
                                break;
                            case "User Account Control: Switch to the secure desktop when prompting forelevation":
                                _regKey = "PromptOnSecureDesktop";
                                _regvalue = 1;
                                break;
                            default:
                                throw new System.InvalidOperationException("parameter  in the App Security xml");
                        }
                        registrycrud = new RegisitryCRUD();
                        registrycrud.createRegistryKey(_regHive, _regPath, _regKey, _regvalue, "");
                        Logger.WriteMessage(_policyName + " value changed to '" + _policyValue + "'");
                        registrycrud = null;
                    }

                    /*
                    * -------------------------------------------------------------------------
                    * SQL Operations
                    * ----------------------------------------------------------------------
                    */
                    Console.WriteLine(" ----------------------------------------------------------------------");
                    Logger.WriteMessage(@"SQL Operations");
                    Console.WriteLine(@"SQL Operations");
                    String dbName = _DBconnectionString.Split(';')[3].Split('=')[1].ToString();
                    if (!sqlOperations.Equals(""))
                    {
                        foreach (XmlNode sqlQuery in sqlOperations)
                        {
                            string _sql = sqlQuery.InnerText.ToString();
                            if (_sql.Contains("Revoke"))
                            {
                                SQLOperations._revokeGuestUserfromDB(dbName, _sql, _DBconnectionString);
                            }
                            else
                                SQLOperations._executeSQLQueries(_sql, _DBconnectionString);
                        }
                    }
                }
            }
        }
    }
}

