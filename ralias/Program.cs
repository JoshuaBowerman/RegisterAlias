using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace ralias
{
    internal class Program
    {

        public static string Path;
        
        
        public static void Main(string[] args)
        {
            Path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.bashrc";
            if (args.Length < 1)
            {
                ShowUsage();
                return;
            }
            if (args[0] == "--show" || args[0] == "-show")
            {
                listAlias();
            }
            if (args[0] == "-r")
            {
                if (args.Length < 2)
                {
                    ShowUsage();
                    return;
                }

                string aliasCommand = "alias";
                args[0] = "";    //remove -r
                foreach (var arg in args)
                {
                    aliasCommand += " " + arg;
                }
                aliasCommand = aliasCommand.Remove(5, 1); // remove extra space
                removeAlias(aliasCommand);
            }
            else
            {
                string aliasCommand = "";
                foreach (var arg in args)
                {
                    aliasCommand += " " + arg;
                }
                addAlias("alias" + aliasCommand);
                Process.Start("alias", aliasCommand);

            }
            
        }



        public static void addAlias(string alias)
        {
                StreamWriter f = File.AppendText(Path);
                f.Write(alias + "\n"); 
                f.Close();
        }

        public static void removeAlias(string alias)
        {
            StreamReader f = File.OpenText(Path);
            var content = f.ReadToEnd();

            if (!content.Contains(alias + "\n"))
            {
                f.Close();
                Console.WriteLine("Alias \'" + alias + "\' was not found!");
                return;
            }
            
            
            var newContent = "";
            foreach (var part in content.Split(new string[]{alias + "\n"},StringSplitOptions.None))
            {
                newContent += " " + part;
            }
            f.Close();
            var attributes = File.GetAttributes(Path);
            File.Delete(Path);
            var file = File.CreateText(Path);
            
            file.WriteLine(newContent);
            file.Close();
            File.SetAttributes(Path,attributes);
            
            
        }

        public static void listAlias()
        {
            Process listCommand = new Process();
            listCommand.StartInfo.FileName = "alias";
            listCommand.StartInfo.Arguments = "-p";
            listCommand.StartInfo.RedirectStandardOutput = true;
            listCommand.Start();
            string output = listCommand.StandardOutput.ReadToEnd();
            listCommand.WaitForExit();
            Console.Write(output);            
        }
        public static void ShowUsage()
        {
            Console.WriteLine("Usage ralias [options] <alias>=\'<command>\'");
            Console.WriteLine("Flags:");
            Console.WriteLine(" --show     List all current aliases.");
            Console.WriteLine(" -s         See: --show");
            Console.WriteLine(" -r         Removes the alias specified");
        }
    }
}