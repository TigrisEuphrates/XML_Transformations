﻿using System.Reflection.Metadata;
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
    static string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    public static void Main(string[] args)
    {
        ////ValidateXSD("hardware.xml", "hardware.xsd");
        ////TransformXML("hardware.xml", "ToHTML_Table.xslt", "html");
        ////TransformXML("hardware.xml", "ToPlainText.xslt", "txt");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load($"{CurrentDirectory}\\..\\..\\..\\..\\Source\\hardware.xml");
        xmlDoc.PreserveWhitespace = true;

        if (args.Length==0)
        {
            //produce textfile with resulting data.
            xpathOutput("/*/*/*[1]", xmlDoc);
        }
        if (args.Length==1)
        {
            xpathOutput(args[0], xmlDoc);
        }
        if (args.Length > 1)
        {
            Console.Write("App accepts only 0 or 1 arguments.\nEnter first argument (if none entered, default will execute): ");
            string? firstArg = Console.ReadLine();
            if (firstArg == null||firstArg=="")
            {
                xpathOutput("/*/*/*[1]", xmlDoc);
            }
            else 
            { 
                xpathOutput(firstArg, xmlDoc);
            }
        }

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

        StreamWriter sW = System.IO.File.CreateText($"..\\..\\..\\Source\\{txtName}");
        sW.Close();
        Console.WriteLine($"Created File {txtName}");


        XslCompiledTransform transform = new XslCompiledTransform();



        transform.Load($"..\\..\\..\\Source\\{xsltString}");


        transform.Transform($"..\\..\\..\\Source\\{inputXml}", $"..\\..\\..\\Source\\{txtName}");
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

        settings.ValidationType = ValidationType.DTD;
        settings.XmlResolver= new XmlUrlResolver();
        XmlReader readerDTD = XmlReader.Create($"..\\..\\..\\Source\\{XMLName}", settings);
        while(readerDTD.Read()) ;
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