using Newtonsoft.Json;

namespace TruckTransmissionGenerator {
    public class TruckTransmissionGenerator {
        static string jsonFile = "";
        static dynamic jsonData = "";
        static string templateFolder;
        static string outputFolder;
        static string[] transmissionsFiles;

        static void GetJsonFile(string[] args) {
            // check if jsonFile has been passed as argument, if not ask for input
            if(args.Length == 0) {
                Console.WriteLine("Please specify json location");
                Console.Write("Input: ");
                while(string.IsNullOrWhiteSpace(jsonFile = Console.ReadLine().Replace("\"",""))) {
                    Console.SetCursorPosition(0,Console.CursorTop - 1);
                    Console.Write("Input: ");
                }
            } else {
                Console.WriteLine(args[0] + "\n");
                jsonFile = args[0];
            }

            // check if jsonFile exists, if not ask for new input
            while(!File.Exists(jsonFile)) {
                Console.BackgroundColor = ConsoleColor.DarkRed; Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("File does not exist! Please enter valid json file");
                Console.ResetColor();

                Console.Write("Input: ");
                while(string.IsNullOrWhiteSpace(jsonFile = Console.ReadLine().Replace("\"",""))) {
                    Console.SetCursorPosition(0,Console.CursorTop - 1);
                    Console.Write("Input: ");
                }
            }
            Console.WriteLine("\u2713 Valid JSON File");
        }

        static void GetJsonData() {
            StreamReader r = new StreamReader(jsonFile);
            string jsonString = r.ReadToEnd();
            jsonData = JsonConvert.DeserializeObject(jsonString);

            if(jsonString == null || jsonData == null) { Environment.Exit(1); }
        }
        static void DeleteExistingFiles() {
            if(Directory.Exists(outputFolder + "/def")) {
                Directory.Delete(outputFolder + "/def",true);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Folder deleted!\n");
            }
        }
        static void SetVariablesFromJson() {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("setting templateFolder variable...");
            templateFolder = jsonData.templateFolder;
            Console.WriteLine("getting transmission templates...");
            transmissionsFiles = Directory.GetFiles(templateFolder,"*.sii");

            Console.WriteLine("setting outputFolder variable...");
            outputFolder = jsonData.outputFolder;
        }

        static void PressAnyKeyTo(string keyword, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine($"Press any key to {keyword}...");
            Console.ReadKey();
        }

        static void Main(string[] args) {
            Console.Title = "TruckTransmissionGenerator";

            GetJsonFile(args);
            GetJsonData();            

            try {
                Output o = new Output();

                o.WriteGameTitel();
                SetVariablesFromJson();
                o.WriteDataFound();
                PressAnyKeyTo("continue",ConsoleColor.White);                

                foreach(var truck in jsonData.trucks) {
                    foreach(var transmission in jsonData.transmissions) {
                        Console.Write($"{truck}: Added {transmission.name}\t");
                        foreach(var differential_ratio in jsonData.differential_ratios) {

                            // Reads transmission template file (prepped with special strings)
                            string siiFile = File.ReadAllText(templateFolder + @"\" + transmission.name + ".sii");

                            // Replacing prepped special strings with actual data
                            siiFile = siiFile.Replace("@name",transmission.name.ToString());
                            siiFile = siiFile.Replace("@differential_ratio_name",differential_ratio.ToString().Replace(".",""));
                            siiFile = siiFile.Replace("@truck",truck.ToString());
                            siiFile = siiFile.Replace("@displayName",$"\"{transmission.displayName.ToString()}\"");
                            siiFile = siiFile.Replace("@price",transmission.price.ToString());
                            siiFile = siiFile.Replace("@unlock",transmission.unlock.ToString());
                            siiFile = siiFile.Replace("@differential_ratio",differential_ratio.ToString());

                            // setting new string variable to last folder where .sii files will be placed            
                            string outputFolderFull = outputFolder + @"\def\vehicle\truck\" + truck.ToString() + @"\transmission\";
                            // creates said directory, likely doesn't exist because all folders will be deleted in line ~40
                            Directory.CreateDirectory(outputFolderFull);

                            // setting file name
                            string outputFile = outputFolderFull + transmission.name.ToString() + "." + differential_ratio.ToString().Replace(".","") + ".sii";
                            // writes file to folder
                            File.WriteAllText(outputFile,siiFile);

                            Console.Write($" ({differential_ratio})");
                        }
                        Console.Write("\n");
                    }
                    Console.Write("\n");
                    Console.ForegroundColor = Output.RandomConsoleColor();
                }
                var totalCount = jsonData.trucks.Count * jsonData.transmissions.Count * jsonData.differential_ratios.Count;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"\nFinished! (Total: {totalCount})\n");
                Console.ForegroundColor = ConsoleColor.White;

                PressAnyKeyTo("exit",ConsoleColor.DarkGreen);
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally {
                Environment.Exit(0);
            }
        }

        public class Output {
            private static Random _random = new Random();
            private static ConsoleColor GetRandomConsoleColor() {
                var consoleColors = Enum.GetValues(typeof(ConsoleColor));
                string[] c = new string[] { "ConsoleColor.DarkGreen","ConsoleColor.DarkRed" };

                return (ConsoleColor)consoleColors.GetValue(_random.Next(consoleColors.Length));
            }
            public static ConsoleColor RandomConsoleColor() {
                return (ConsoleColor)new Random().Next(1,15);
            }
            public void WriteGameTitel() {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkRed; Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n {jsonData.game} \n");
                Console.ResetColor();
            }
            public void WriteDataFound() {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n---------");
                Console.WriteLine("Found " + jsonData.trucks.Count + " trucks");
                Console.WriteLine("Found " + jsonData.transmissions.Count + " transmissions");
                Console.WriteLine("Found " + jsonData.differential_ratios.Count + " differential ratios");
                Console.WriteLine("---------\n");
            }
        }
    }
}