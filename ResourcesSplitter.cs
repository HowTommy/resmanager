using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ResourcesSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string refToHowTommy =
                    "// that resource file was created with a tool: resmanager by howtommy (https://github.com/HowTommy/resmanager)";

                var sb = new StringBuilder();

                var sbLogs = new StringBuilder();

                string resources = File.ReadAllText("resources.lng");
                var lines = resources.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                var languages = lines[0].Split(new string[] {"\t"}, StringSplitOptions.None).Skip(1);

                File.WriteAllText("defaultLanguage.php", string.Format("<?php{0}\t$defaultLanguage = '{1}';{0}?>", Environment.NewLine, languages.ElementAt(0)));

                string fileBeginningWithLanguages =
                    string.Format("<?php{0}\t{1}{0}\t$languages = array('{2}');{0}",
                        Environment.NewLine,
                        refToHowTommy,
                        string.Join("', '", languages));

                for (var i = 0; i < languages.Count(); i++)
                {
                    sb.Append(fileBeginningWithLanguages);

                    string language = languages.ElementAt(i);

                    sb.Append(string.Format("\t$currentLanguage = '{0}';{1}", language, Environment.NewLine));

                    sb.Append(string.Format("\t$resources = array(", Environment.NewLine));

                    bool isFirstLine = true;

                    foreach (var line in lines.Skip(1))
                    {
                        if (!line.StartsWith("//"))
                        {
                            string[] lineParts = line.Split(new string[] {"\t"}, StringSplitOptions.None);

                            if (lineParts.Count() == languages.Count() + 1)
                            {
                                if (isFirstLine)
                                {
                                    isFirstLine = false;
                                }
                                else
                                {
                                    sb.Append(",");
                                }

                                sb.Append(string.Format("{0}\t\t'{1}' => '{2}'", Environment.NewLine, lineParts[0],
                                    lineParts[i + 1]));
                            }
                            else
                            {
                                if (0 == i && lineParts.Any())
                                {
                                    sbLogs.AppendLine("Error with resource: " + lineParts[0]);
                                }
                            }
                        }
                    }

                    sb.Append(string.Format("{0}\t);{0}?>", Environment.NewLine));

                    File.WriteAllText("resources_" + language + ".php", sb.ToString());
                    sb.Clear();
                }
                if (!string.IsNullOrEmpty(sbLogs.ToString()))
                {
                    Console.WriteLine(sbLogs.ToString());
                    Console.Read();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured:");
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}
