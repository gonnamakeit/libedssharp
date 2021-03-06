using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libEDSsharp;

namespace EDSSharp
{
    class Program
    {

        static libEDSsharp.EDSsharp eds = new EDSsharp();
        static string gitversion = "";

        static void Main(string[] args)
        {
            try
            {

                Dictionary<string, string> argskvp = new Dictionary<string, string>();

                int argv = 0;

                for (argv = 0; argv < (args.Length - 1); argv++)
                {
                    if (args[argv] == "--infile")
                    {
                        argskvp.Add("--infile", args[argv + 1]);
                    }

                    if (args[argv] == "--outfile")
                    {
                        argskvp.Add("--outfile", args[argv + 1]);
                    }

                    if (args[argv] == "--type")
                    {
                        argskvp.Add("--type", args[argv + 1]);
                    }

                    argv++;
                }


                if (argskvp.ContainsKey("--type") && argskvp.ContainsKey("--infile") && argskvp.ContainsKey("--outfile"))
                {
                    string infile = argskvp["--infile"];
                    string outfile = argskvp["--outfile"];

                    switch (Path.GetExtension(infile).ToLower())
                    {
                        case ".xdd":
                            openXDDfile(infile, outfile);
                            break;

                        case ".xml":
                            openXMLfile(infile,outfile);
                            break;

                        case ".eds":
                            openEDSfile(infile, outfile,InfoSection.Filetype.File_EDS);
                            break;


                        default:
                            return;

                    }
                }
                else
                {
                    Console.WriteLine("Usage EDSEditor --type CanOpenNode --infile file.[xdd|eds|xml] --outfile CO_OD.c");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void openEDSfile(string infile, string outfile, InfoSection.Filetype ft)
        {
          
            eds.Loadfile(infile);

            exportCOOD(outfile);

        }

        private static void exportCOOD(string outpath)
        {

            outpath = Path.GetFullPath(outpath);

            string savePath = Path.GetDirectoryName(outpath);

            eds.fi.exportFolder = savePath;

            Warnings.warning_list.Clear();

            CanOpenNodeExporter cone = new CanOpenNodeExporter();
            cone.export(savePath, Path.GetFileNameWithoutExtension(outpath), gitversion, eds);

            foreach(string warning in Warnings.warning_list)
            {
                Console.WriteLine("WARNING :" + warning);
            }

        }

        private static void openXMLfile(string path,string outpath)
        {

            CanOpenXML coxml = new CanOpenXML();
            coxml.readXML(path);

            Bridge b = new Bridge();

            eds = b.convert(coxml.dev);
            eds.xmlfilename = path;

            exportCOOD(outpath);

        }

        private static void openXDDfile(string path, string outpath)
        {
            CanOpenXDD coxml = new CanOpenXDD();
            eds = coxml.readXML(path);

            if (eds == null)
                return;

            eds.xddfilename = path;
            exportCOOD(outpath);
        }
    }
}
