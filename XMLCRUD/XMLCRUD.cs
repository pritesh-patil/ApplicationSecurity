using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using CopyFiles;
using Approva.QA.Common;

namespace XMLOps
{
    public class XMLCRUD
    {
        public static void _modifyXML(string _sourcePath, string _fileName, string _node, string _refNode, bool _createNode)
        {
            // string fname = "C:\\Pritesh_Data\\PP\\Web.config";
            XmlNode _xmlnode = null;
            XmlElement _element = null;
            XmlAttribute _attribute = null;
            //  XmlAttribute _attribute = null;
            XmlDocument xmldoc = new XmlDocument();
            if (_createNode)
                _xmlnode = xmldoc.CreateNode("element", _node, "");
            else
                _element = xmldoc.CreateElement(_node);

            if (CopyDLL._backupFiles(_sourcePath, _fileName))
            {

                switch (_node)
                {
                    case "httpProtocol":
                        _xmlnode.InnerXml = "<customHeader><remove name = \"X-Powered-By\"/><add name = \"Stric-Transport-Security\" value = \"max-age=31536000\"/></customHeader>";
                        _node = _node + "/customHeader/remove";
                        break;
                    case "security":
                        _xmlnode.InnerXml = "<requestFiltering><fileExtensions allowUnlisted=\"false\"><remove fileExtension=\".browser\"/><remove fileExtension=\".skin\"/><remove fileExtension=\".asax\" /><remove fileExtension=\".ascx\" /><remove fileExtension=\".config\"/><remove fileExtension=\".master\"/><remove fileExtension=\".resources\"/><remove fileExtension=\".resx\"/><remove fileExtension=\".sitemap\"/><add fileExtension=\".zip\" allowed=\"true\"/><add fileExtension=\".xslt\" allowed=\"true\"/><add fileExtension=\".xsd\" allowed=\"true\"/><add fileExtension=\".xml\" allowed=\"true\"/><add fileExtension=\".sitemap\" allowed=\"true\"/><add fileExtension=\".resx\" allowed=\"true\"/><add fileExtension=\".resources\" allowed=\"true\"/><add fileExtension=\".master\" allowed=\"true\"/><add fileExtension=\".js\" allowed=\"true\"/><add fileExtension=\".html\" allowed=\"true\"/><add fileExtension=\".htm\" allowed=\"true\"/><add fileExtension=\".csv\" allowed=\"true\"/><add fileExtension=\".css\" allowed=\"true\"/><add fileExtension=\".config\" allowed=\"true\"/><add fileExtension=\".bin\" allowed=\"true\"/><add fileExtension=\".bez\" allowed=\"true\"/><add fileExtension=\".beu\" allowed=\"true\"/><add fileExtension=\".aspx\" allowed=\"true\"/><add fileExtension=\".asmx\" allowed=\"true\"/><add fileExtension=\".asax\" allowed=\"true\"/><add fileExtension=\".ascx\" allowed=\"true\"/><add fileExtension=\".skin\" allowed=\"true\"/><add fileExtension=\".axd\" allowed=\"true\"/><add fileExtension=\".png\" allowed=\"true\"/><add fileExtension=\".jpeg\" allowed=\"true\"/><add fileExtension=\".ico\" allowed=\"true\"/><add fileExtension=\".gif\" allowed=\"true\"/><add fileExtension=\".swf\" allowed=\"true\"/><add fileExtension=\".settings\" allowed=\"true\"/><add fileExtension=\".jpg\" allowed=\"true\"/><add fileExtension=\".pdf\" allowed=\"true\"/><add fileExtension=\".xls\" allowed=\"true\"/><add fileExtension=\".xlsx\" allowed=\"true\"/><add fileExtension=\".doc\" allowed=\"true\"/><add fileExtension=\".docx\" allowed=\"true\"/><add fileExtension=\".tff\" allowed=\"true\"/><add fileExtension=\".browser\" allowed=\"true\"/><add fileExtension=\".ashx\" allowed=\"true\"/></fileExtensions> </requestFiltering>";
                        _node = _node + "/requestFiltering/fileExtensions";
                        break;
                    case "deployment":
                        _attribute = xmldoc.CreateAttribute("retail");
                        _attribute.Value = "true";
                        _element.Attributes.Append(_attribute);
                        break;
                    case "AntiHttpCrossSiteForgeryRequestModule":
                        _element = null;
                        xmldoc.CreateElement("add");
                        _attribute = xmldoc.CreateAttribute("name");
                        _attribute.Value = "AntiHttpCrossSiteForgeryRequestModule";
                        _element.Attributes.Append(_attribute);
                        XmlAttribute _attribute2 = xmldoc.CreateAttribute("type");
                        _attribute2.Value = "Approva.Presentation.Framework.HttpModule.AntiHttpCrossSiteForgeryRequestModule, Approva.Presentation.Framework.HttpModule";
                        _element.Attributes.Append(_attribute2);
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
                string sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
                //string sourceFile = "C:\\Pritesh_Data\\PP\\Web.config";
                if (System.IO.File.Exists(sourceFile))
                {
                    try
                    {
                        string refNodeMarkup = _refNode;
                        if (!isNodeExists(sourceFile, refNodeMarkup, _node))
                        {
                            xmldoc.Load(sourceFile);
                            if (_createNode)
                            {
                                xmldoc.SelectSingleNode(refNodeMarkup).AppendChild(_xmlnode);
                                Logger.WriteMessage("Modified file for " + _node + " setting");

                            }
                            else
                            {
                                xmldoc.SelectSingleNode(refNodeMarkup).AppendChild(_element);
                            }
                            xmldoc.Save(sourceFile);
                        }
                        else
                            Logger.WriteMessage("Node +" + _node.ToString() + " is already present");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteMessage("FAILED TO Modified file for " + _node + "setting");
                        Logger.WriteError(ex.Message);
                    }
                }
                else
                {
                    Logger.WriteMessage("file " + sourceFile + " is not present.");
                    Logger.WriteMessage("Node " + _node + " not added/created");
                }
            }
        }

        public static void addAttributeToExistingNode(string _sourcePath, string _fileName, string _attribute, string _attributeValue, string _refNode)
        {

            string sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(sourceFile);
            XmlNodeList refNode = xmldoc.GetElementsByTagName(_refNode);
            foreach (XmlNode xmlNode in refNode)
            {
                try
                {
                    if (xmlNode.Attributes.GetNamedItem(_attribute).Value.Equals(_attributeValue))
                    {
                        Logger.WriteMessage("" + sourceFile.ToString() + " already contains " + _attribute + ":" + _attributeValue);
                    }
                }
                catch (Exception e)
                {
                    XmlAttribute attribute = xmldoc.CreateAttribute(_attribute);
                    attribute.Value = _attributeValue;
                    xmlNode.Attributes.Append(attribute);
                    Logger.WriteMessage("Atrribute in file" + sourceFile.ToString() + "added: " + _attribute + ":" + _attributeValue);
                }
            }
            xmldoc.Save(sourceFile);
        }

        public static bool isNodeExists(string _sourceFileName, string _refNodeMarkup, string _node)
        {
            bool _isPresent = false;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_sourceFileName);
            XmlNode exist = null;
            exist = xmlDoc.SelectSingleNode(@"./" + _refNodeMarkup + "/" + _node);
            if (exist != null)
                _isPresent = true;
            else
                _isPresent = false;
            xmlDoc = null;
            return _isPresent;
        }
        public static void _commentInWebConfigXML(string _sourcePath, string _fileName, string _refNode, string _node, bool _commentNode)
        {
            XmlDocument xmldoc = new XmlDocument();
            string sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
            Logger.WriteMessage("Source file Path: " + sourceFile);
            try
            {
                xmldoc.Load(sourceFile);
                Logger.WriteMessage("Source file " + sourceFile + " successfully loaded");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while rading XML file :" + sourceFile);
                Logger.WriteMessage("Error while rading XML file :" + sourceFile + "\n" + e.Message);
                Environment.Exit(0);
            }

            string refNodeMarkup = _refNode + _node;

            // Get the target node using XPath
            try
            {
                System.Xml.XmlNode elementToComment = xmldoc.SelectSingleNode(refNodeMarkup);
                String commentContents = elementToComment.OuterXml;
                XmlComment commentNode = xmldoc.CreateComment(commentContents);
                // Get a reference to the parent of the target node
                XmlNode parentNode = elementToComment.ParentNode;
                // Replace the target node with the comment
                parentNode.ReplaceChild(commentNode, elementToComment);

            }
            catch (Exception ex)
            {
                Logger.WriteMessage("verify if node is present in file");
                //Logger.WriteError(ex.Message);
            }
            xmldoc.Save(sourceFile);
        }
    }
}
