using Newtonsoft.Json;

namespace TruckTransmissionGenerator {
    // TODO
    // - add check for empty string in json (f.ex. empty truck in array)
    // - add default json location
    class TruckTransmissionGenerator {
        static string VerifyJsonFile(string? jsonFile) {
            // check if jsonFile has been passed as argument, if not ask for input
            if(jsonFile == null) {
                Console.WriteLine("Please specify json location");
                Console.Write("Input: ");
                while(string.IsNullOrWhiteSpace(jsonFile = Console.ReadLine().Replace("\"",""))) {
                    Console.SetCursorPosition(0,Console.CursorTop - 1);
                    Console.Write("Input: ");
                }
            } else {
                Console.WriteLine(jsonFile + "\n");
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
            return jsonFile;
        }
        static dynamic GetJsonData(string jsonFile) {
            string jsonString = new StreamReader(jsonFile).ReadToEnd();
            dynamic? jsonData = JsonConvert.DeserializeObject(jsonString);
            if(jsonFile == null || jsonString.Length == 0) {
                Console.WriteLine("JSON File is empty. Please try again");
                Environment.Exit(1);
            }

            return jsonData;
        }
        static void SetData(Data data) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Receiving data from input...");
            try {

                Console.WriteLine("... JSON data received");
                data.TemplateFolder = data.JsonData.templateFolder;
                data.TransmissionsFiles = Directory.GetFiles(data.TemplateFolder,"*.sii");
                Console.WriteLine("... Transmission Templates received");
                data.OutputFolder = data.JsonData.outputFolder;
                Console.WriteLine("... Output directory set");
                Console.ResetColor();
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        static void DeleteExistingFiles(string directory) {
            if(Directory.Exists(directory + "/def")) {
                Directory.Delete(directory + "/def",true);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nExisting directory deleted!\n");
                Console.ResetColor();
            }
        }
        static void PressAnyKeyTo(string keyword, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine($"Press any key to {keyword}...");
            Console.ResetColor();
            Console.ReadKey();
        }
        static string ModifySiiFile(string siiFile, dynamic truck, dynamic transmission, dynamic differential_ratio) {
            siiFile = siiFile.Replace("@name",transmission.name.ToString());
            siiFile = siiFile.Replace("@differential_ratio_name",differential_ratio.ToString().Replace(".",""));
            siiFile = siiFile.Replace("@truck",truck.ToString());
            siiFile = siiFile.Replace("@displayName",$"\"{transmission.displayName.ToString()}\"");
            siiFile = siiFile.Replace("@price",transmission.price.ToString());
            siiFile = siiFile.Replace("@unlock",transmission.unlock.ToString());
            siiFile = siiFile.Replace("@differential_ratio",differential_ratio.ToString());

            return siiFile;
        }
        static void Main(string[] args) {
            Console.Title = "TruckTransmissionGenerator";

            Data data = new();
            data.JsonFile = VerifyJsonFile(args[0]);
            data.JsonData = GetJsonData(data.JsonFile);
            
            
            // DEBUG
            //Console.WriteLine($"JsonFile: {data.JsonFile}");
            //Console.WriteLine(data.JsonData);
            //Console.WriteLine(data.TemplateFolder);
            //Console.WriteLine(data.OutputFolder);
            //Console.WriteLine(data.TransmissionsFiles[0]);

            try {
                Output ConsoleOutput = new();
                ConsoleOutput.WriteGameTitel(data.JsonData.game.ToString());
                SetData(data);
                ConsoleOutput.WriteDataFound(data.JsonData);

                PressAnyKeyTo("continue",ConsoleColor.White);

                DeleteExistingFiles(data.OutputFolder);

                foreach(var truck in data.JsonData.trucks) {
                    foreach(var transmission in data.JsonData.transmissions) {
                        Console.Write($"{truck}: Added {transmission.name}\t");
                        foreach(var differential_ratio in data.JsonData.differential_ratios) {
                            // read the template transmission file
                            string siiFile = File.ReadAllText($@"{data.TemplateFolder}\{transmission.name}.sii");
                            // replacing placeholder values in template transmission file with data from the json file
                            siiFile = ModifySiiFile(siiFile,truck,transmission,differential_ratio);

                            // setting string variable to truck transmission folder            
                            string outputFolderFull = $@"{data.OutputFolder}\def\vehicle\truck\{truck.ToString()}\transmission";
                            // creates said directory, likely doesn't exist because all folders will be deleted with DeleteExistingFiles();
                            Directory.CreateDirectory(outputFolderFull);

                            // setting file name
                            string outputFile = $@"{outputFolderFull}\{transmission.name.ToString()}.{differential_ratio.ToString().Replace(".","")}.sii";
                            // writes file to folder
                            File.WriteAllText(outputFile,siiFile);

                            Console.Write($" ({differential_ratio})");
                        }
                        Console.Write("\n");
                    }
                    Console.Write("\n");
                    Console.ForegroundColor = Output.RandomConsoleColor();
                }
                var totalCount = data.JsonData.trucks.Count * data.JsonData.transmissions.Count * data.JsonData.differential_ratios.Count;
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
        class Output {
            private static Random _random = new Random();
            private static ConsoleColor GetRandomConsoleColor() {
                var consoleColors = Enum.GetValues(typeof(ConsoleColor));
                string[] c = new string[] { "ConsoleColor.DarkGreen","ConsoleColor.DarkRed" };
        
                return (ConsoleColor)consoleColors.GetValue(_random.Next(consoleColors.Length));
            }
            public static ConsoleColor RandomConsoleColor() {
                return (ConsoleColor)new Random().Next(1,15);
            }
            public void WriteGameTitel(string gameTitle) {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkRed; Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n {gameTitle} \n");
                Console.ResetColor();
            }
            public void WriteDataFound(dynamic data) {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n---------------------------------");
                Console.WriteLine($"| Found {data.trucks.Count} trucks\t\t|");
                Console.WriteLine($"| Found {data.transmissions.Count} transmissions\t\t|");
                Console.WriteLine($"| Found {data.differential_ratios.Count} differential ratios\t|");
                Console.WriteLine("---------------------------------\n");
                Console.ResetColor();
            }
        
        }
    }
}