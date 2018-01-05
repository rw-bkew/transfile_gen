using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace transfile_gen
{
    class Program
    {
        private static List<TransXref> translist = new List<TransXref>();

        static void Main(string[] args)
        {

            var transPath = "C:/GitHub/RenWeb.Application.LMS/src/content/locales/";
            var primaryTransFname = transPath + "locale-en_US.json";


            // http://stackoverflow.com/questions/27025435/how-do-i-read-and-write-a-c-sharp-string-dictionary-to-a-file
            var baseDictionary = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(primaryTransFname));



            if (args.Length == 0)
            {
                Console.WriteLine("get json files");
                var jsonFiles = Directory.GetFiles(transPath, "*.json", SearchOption.AllDirectories);

                foreach (var json in jsonFiles)
                {
                    if (json != primaryTransFname)
                    {
                        Console.WriteLine(json);
                        var csvPath = Path.ChangeExtension(json, "csv");


                        var transDictionary = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(json));
                        var missingKeys = baseDictionary.Keys.Except(transDictionary.Keys);
                        Console.WriteLine("Keys missing from {0}", json);
                        foreach (var dictKey in missingKeys)
                        {
                            Console.WriteLine(dictKey);
                        }
                        missingKeys = transDictionary.Keys.Except(baseDictionary.Keys);
                        Console.WriteLine("Keys in {0} not in US file", json);
                        foreach (var dictKey in missingKeys)
                        {
                            Console.WriteLine(dictKey);
                        }

                        WriteTransFile(csvPath, baseDictionary, transDictionary);

                    }

                }
            }
            else
            {
                Console.WriteLine(args.Length);
                Console.WriteLine(args[0]);
                var csvPath = transPath + "locale-" + args[0] + ".csv";
                var jsonPath = "locale-" + args[0] + ".json";

                var jsonFiles = Directory.GetFiles(transPath, jsonPath, SearchOption.AllDirectories);
                Console.WriteLine(jsonFiles.Length);
                if (jsonFiles.Length > 0)
                {
                    Console.WriteLine("{0} already exists, use the default conversion for this file", jsonPath);
                }
                else
                {
                    WriteTransFile(csvPath, baseDictionary, null);
                }
                
            }

        }

        private static void WriteTransFile(string csvPath, Dictionary<string, string> baseDictionary,
            Dictionary<string, string> transDictionary)
        {
            List<string> fileContents = new List<string>();

            // writing to csv file for excel
            fileContents.Add("sep=|");
            fileContents.Add("Key (Do not change!)|English|Translation");
            foreach (var transrec in baseDictionary)
            {
                string result;
                string s;

                if (transDictionary == null || !transDictionary.TryGetValue(transrec.Key, out result))
                {
                    result = null;
                }
                translist.Add(new TransXref(transrec.Key, transrec.Value, result));
                // excel still needs the double quotes
                // in order to have double quotes in the file, they need to be doubled
                if (String.IsNullOrEmpty(translist.Last().transText)) {
                    s = String.Format("\"{0}\"|\"{1}\"|\"\"", translist.Last().baseKey, translist.Last().baseText.Replace("\"", "\"\""));
                }
                else {
                    s = String.Format("\"{0}\"|\"{1}\"|\"{2}\"", translist.Last().baseKey, translist.Last().baseText.Replace("\"", "\"\""),
                        translist.Last().transText.Replace("\"", "\"\""));
               
                }
                fileContents.Add(s);
            }

            File.WriteAllLines(csvPath, fileContents, Encoding.Unicode);
        }
    }
}
