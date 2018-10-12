using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;
using Microsoft.Web.Administration;
using Approva.QA.Common;

namespace CMDOps
{
    public class CMDOperations
    {
       
        public static void cmdRun(string _cmd, string _websiteName, bool _ishttps, string _portNumber, string _wwwrootPath)
        {
            string _cmdtext = null;
            string[] subsites = new string[] { "BizRightsCoreSettings", "BRPublisher", "Core", "IRC", "PDSService", "TMAdapter", "CertificationManager" };
            switch (_cmd)
            {
                case "forms Authentication to use cookies":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "\" /section:system.web/authentication /forms.cookieless:\"UseCookies\"";
                    break;
                case "Debug off in the IIS":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "\" /commit:WEBROOT /section:compilation /!debug:False";
                    break;
                case "Required SSL in form authentication":
                    foreach (string site in subsites)
                    {
                        _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config  \"" + _websiteName + "/" + site.ToString() + "\"  /section:system.web/authentication /forms.requireSSL:true";
                        runCommand(_cmd+" for:"+ _websiteName + "/" + site.ToString(), _cmdtext);
                    }
                    _cmdtext = null;
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config  \"" + _websiteName + "\"  /section:system.web/authentication /forms.requireSSL:true";
                    runCommand(_cmd + " for:" + _websiteName, _cmdtext);
                    break;
                case "Anonymous user identity to use the Application Pool identity":
                    _cmdtext = @"C:\Windows\system32\inetsrv\appcmd.exe set config /section:anonymousAuthentication /userName:";
                    break;
                case "Default application pool identity":
                    _cmdtext = @"C:\Windows\system32\inetsrv\appcmd.exe set config /section:applicationPools /[name='IRC'].processModel.identityType:ApplicationPoolIdentity";
                    break;
                case "Host Header":
                    string _fqdnName = GetFQDN();
                    Console.WriteLine("     FQDN: " + _fqdnName);
                    if (_ishttps)
                        _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set site /site.name: \"" + _websiteName + "\" /bindings.[protocol='https',bindingInformation='*:" + _portNumber + ":'].bindingInformation:*:" + _portNumber + ":" + _fqdnName;
                    else
                        _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set site /site.name: \"" + _websiteName + "\" /bindings.[protocol='http',bindingInformation='*:" + _portNumber + ":'].bindingInformation:*:" + _portNumber + ":" + _fqdnName;
                    break;
                case "Directory Browsing":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "/CertificationManager\" /section:directoryBrowse / enabled:false";
                    break;
                case "X-Content-Type-Options":
                   // string[] subsites = new string[] { "BizRightsCoreSettings", "BRPublisher", "Core", "IRC", "PDSService", "TMAdapter", "CertificationManager" };

                    foreach (string site in subsites)
                    {
                        // Logger.WriteMessage("X-Content-Type-Options for " + _websiteName + "/" + s.ToString());
                        //Console.WriteLine("X-Content-Type-Options for " + _websiteName + "/" + s.ToString());
                        _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "/" + site.ToString() + "\" /section:httpProtocol /-customHeaders.[name='X-Content-Type-Options']";
                        // System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", " /C " + _cmdtext);
                        //  Thread.Sleep(3000);
                        runCommand("X-Content-Type-Options for " + _websiteName + "/" + site.ToString(), _cmdtext);
                        // Logger.WriteMessage("Command to " + _cmdtext + " is executed to remove X-Content-Type-Options SuccessFully");
                    }
                    _cmdtext = null;
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "\" /section:httpProtocol /+customHeaders.[name='X-Content-Type-Options',value='nosniff']";
                    break;
                case "Request Filter":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "\" /section:requestfiltering /allowhighbitcharacters:false";
                    break;
                case "Double Escaping":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "\" /section:requestfiltering /allowdoubleescaping:false";
                    break;
                case "HTTP TRACE":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config \"" + _websiteName + "\" -section:system.webServer/security/requestFiltering /+verbs.[verb='TRACE',allowed='False']";
                    break;
                case "ISAPI_CGI Restrictions":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config /section:isapiCgiRestriction /notListedCGIsAllowed:false /notlistedISAPIsAllowed:false";
                    break;
                case "Change IIS Physical path":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set site \"" + _websiteName + "\" /application[path='/'].virtualDirectory[path='/'].physicalPath:" + _wwwrootPath;
                    break;
                case "Change IIS log path":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config -section:sites -siteDefaults.logfile.directory:" + _wwwrootPath + @":\LogFiles";
                    break;
                case "Machine Key 4.5 Validation":
                    _cmdtext = "C:\\Windows\\system32\\inetsrv\\appcmd.exe set config /commit:WEBROOT -section:system.web/machineKey /validation:\"HMACSHA256\"";
                    break;
                default:
                    break;
            }
            runCommand(_cmd, _cmdtext);
        }
        public static string GetFQDN()
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))  // if hostname does not already include domain name
            {
                hostName += domainName;   // add the domain name part
            }
            return hostName;                    // return the fully qualified name
        }

        private static void runCommand(string _cmdName, string _cmdtext)
        {
            try
            {
                Console.WriteLine(" ----------------------------------------------------------------------");
                Logger.WriteMessage(_cmdName);
                Console.WriteLine(" "+_cmdName);
                // System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", " /C " + _cmdtext);
                var pro2 = new System.Diagnostics.Process();
                pro2.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                pro2.StartInfo.Arguments = "/c " + _cmdtext;
                Logger.WriteMessage("Arguments: " + pro2.StartInfo.Arguments);
                pro2.StartInfo.CreateNoWindow = true;
                pro2.StartInfo.UseShellExecute = false;
                pro2.StartInfo.LoadUserProfile = true;
                pro2.StartInfo.RedirectStandardOutput = true;
                pro2.StartInfo.Verb = "runas";
                pro2.Start();
                while (!pro2.StandardOutput.EndOfStream)
                {
                    string line = pro2.StandardOutput.ReadToEnd();
                    //Console.WriteLine("Process OutPut: "+line);
                    Logger.WriteMessage("Process OutPut: " + line);
                }
                pro2.WaitForExit();
                pro2.Close();                
                Logger.WriteMessage("" + _cmdName + " :Command to " + _cmdtext + " is executed to SuccessFully");
               // Console.WriteLine("" + _cmdName + " :Command to " + _cmdtext + " is executed to SuccessFully");
                
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);
                Logger.WriteMessage("Command to " + _cmdtext + " is executed and error occured");
            }
        }
    }
}
