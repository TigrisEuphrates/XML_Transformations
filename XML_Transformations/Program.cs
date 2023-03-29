using System.Reflection.Metadata;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System;
using System.IO;
using System.Text;
using System.Reflection.PortableExecutable;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace XML_Transformations;
public class Program
{
#nullable enable
    static readonly string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    static  string defaultDir = $"{CurrentDirectory}\\Source\\Task_1_2\\XPaths2.txt";
    static readonly XmlDocument xmlDoc = new();
    public static void Main(string[] args)
    {
        xmlDoc.Load($"{CurrentDirectory}\\Source\\Task_1_2\\hardware.xml");
        xmlDoc.PreserveWhitespace = true;

        ValidateDTD("hardware.xml");
        ValidateXSD("hardware.xml","hardware.xsd");


        TransformXML("Hardware.xml", "ToHTML_Table.xslt", "html");
        TransformXML("Hardware.xml", "ToHTML_List.xslt", "html");
        TransformXML("Hardware.xml", "ToHTML_TXT.xslt", "txt");
    }


    public static void xPathResultsToTxt()
    {
        
            xpathWriteToText(defaultDir, xmlDoc);
        
    }


    public static void xpathOutput(string xpath, XmlDocument doc)
    {
       // XmlDocument doc = new XmlDocument();
       // doc.PreserveWhitespace = true;
        if (doc.DocumentElement is not null)
        {
            XmlNodeList? nodes = doc.DocumentElement.SelectNodes(xpath);

            if (nodes is not null)
            {
                if (nodes.Count == 0)
                {
                    Console.WriteLine("No object found or XPath written incorrect.");
                    return;
                }
                foreach (XmlNode node in nodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        Console.WriteLine(node.Name + " = " + node.InnerText);
                    }
                }
            }
        }






    }



    public static void TransformXML(string inputXml, string xsltString, string format)
    {
        string txtName = xsltString.Remove(xsltString.Length - 5, 5);
        txtName = txtName.Insert(txtName.Length, "." + format);

        StreamWriter sW = System.IO.File.CreateText($"{CurrentDirectory}\\Source\\Created_files\\{txtName}");
        sW.Close();
        Console.WriteLine($"Created File {txtName}");
        //XslCompiledTransform XSLTrans = new XslCompiledTransform();
        //XSLTrans.Load($"{CurrentDirectory}\\Source\\Task_3_4\\{xsltString}");
        //XmlTextWriter myWriter = new XmlTextWriter($"{CurrentDirectory}\\Source\\Created_files\\{txtName}", null);
        //XSLTrans.Transform($"{CurrentDirectory}\\Source\\Task_1_2\\{inputXml}", null, myWriter);


        //using (XmlReader xsltReader = XmlReader.Create($"{CurrentDirectory}\\Source\\Task_3_4\\{xsltString}", new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
        //{
        //    XslCompiledTransform XSLTrans = new XslCompiledTransform();
        //    XSLTrans.Load(xsltReader);
        //    XmlTextWriter myWriter = new XmlTextWriter($"{CurrentDirectory}\\Source\\Created_files\\{txtName}", null);
        //    XSLTrans.Transform($"{CurrentDirectory}\\Source\\Task_1_2\\{inputXml}", null, myWriter);
        //}

        using (XmlReader xmlReader = XmlReader.Create($"{CurrentDirectory}\\Source\\Task_1_2\\{inputXml}", new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
        {
            XslCompiledTransform XSLTrans = new XslCompiledTransform();
            XSLTrans.Load($"{CurrentDirectory}\\Source\\Task_3_4\\{xsltString}");
            XmlTextWriter myWriter = new XmlTextWriter($"{CurrentDirectory}\\Source\\Created_files\\{txtName}", null);
            XSLTrans.Transform(xmlReader, null, myWriter);
        }

    }

    public static async void printResult(XmlNodeList nodeList, string txtName, String tab = "", bool needRecursion = true)
    {
        foreach (XmlNode node in nodeList)
        {
            if (node.NodeType == XmlNodeType.Whitespace || node.NodeType == XmlNodeType.Text)
            {
                continue;
            }

            Console.WriteLine(tab + node.NodeType + " -> " + node.Name + " -> " + node.InnerText);

            using (StreamWriter file = new($"..\\..\\..\\{txtName}", true))
            {
                await file.WriteLineAsync(tab + node.NodeType + " -> " + node.Name + " -> " + node.InnerText);
            }

            if (node.NodeType == XmlNodeType.Element && needRecursion)
            {
                printResult(node.ChildNodes, txtName, tab + "   ", needRecursion);
            }
        }
    }

    public static void ValidateDTD(string XMLName)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Parse;
        settings.ValidationType = ValidationType.DTD;
        settings.XmlResolver = new XmlUrlResolver();
        settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

        XmlReader reader = XmlReader.Create($"..\\..\\..\\Source\\{XMLName}", settings);

        while (reader.Read()) ;
    }

    private static void ValidationCallBack(object? sender, ValidationEventArgs e)
    {
        Console.WriteLine("Validation Error: {0}", e.Message);
    }


    static void ValidateXSD(string XMLName, string XSDName)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.Schemas.Add("https://www.company.com", $"..\\..\\..\\Source\\{XSDName}");
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationEventHandler += XSDValidationCallBack;
        settings.DtdProcessing = DtdProcessing.Parse;

        XmlReader readerXSD = XmlReader.Create($"..\\..\\..\\Source\\{XMLName}", settings);
        while (readerXSD.Read()) ;

        //settings.ValidationType = ValidationType.DTD;
        //settings.XmlResolver= new XmlUrlResolver();
        //XmlReader readerDTD = XmlReader.Create($"{CurrentDirectory}\\Source\\Task_1_2\\{XMLName}", settings);
        //while(readerDTD.Read()) ;
    }

    static void XSDValidationCallBack(object? sender, ValidationEventArgs e)
    {
        if (e.Severity == XmlSeverityType.Warning)
        {
            Console.Write("WARNING: ");
            Console.WriteLine(e.Message);
        }
        else if (e.Severity == XmlSeverityType.Error)
        {
            Console.Write("ERROR: ");
            Console.WriteLine(e.Message);
        }
    }
}