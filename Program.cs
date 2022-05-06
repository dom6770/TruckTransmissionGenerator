using Newtonsoft.Json;

public class TruckTransmissionGenerator {
    private static Random _random = new Random();
    private static ConsoleColor GetRandomConsoleColor() {
        var consoleColors = Enum.GetValues(typeof(ConsoleColor));
        string[] c = new string[] { "ConsoleColor.DarkGreen","ConsoleColor.DarkRed" };

        return (ConsoleColor)consoleColors.GetValue(_random.Next(consoleColors.Length));
    }
    public static ConsoleColor RandomConsoleColor() {
        return (ConsoleColor)new Random().Next(1,15);
    }
    static void Main(string[] args) {
        string jsonFile;

        // check if jsonFile has been passed as argument, if not ask for input
        if(args.Length == 0) {
            Console.WriteLine("Please specify json location");
            Console.Write("Input: ");
            jsonFile = Console.ReadLine();
        } else {
            //Console.Write("args[0]: ");
            //Console.WriteLine(args[0]);
            jsonFile = args[0];
        }

        // read json file
        StreamReader r = new StreamReader(jsonFile);
        string jsonString = r.ReadToEnd();
        dynamic jsonData = JsonConvert.DeserializeObject(jsonString); 

        // set necessary variables
        string templateFolder;
        string outputFolder;
        string[] transmissionsFiles;

        try {
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"\n {jsonData?.game} \n");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("setting templateFolder variable...");
            templateFolder = jsonData?.templateFolder;
            Console.WriteLine("getting transmission templates...");
            transmissionsFiles = Directory.GetFiles(templateFolder,"*.sii");

            Console.WriteLine("setting outputFolder variable...");
            outputFolder = jsonData?.outputFolder;


            if(Directory.Exists(outputFolder + "/def")) {
                Directory.Delete(outputFolder + "/def",true);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nFolder deleted!");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n---------");
            Console.WriteLine("Found " + jsonData?.trucks.Count + " trucks");
            Console.WriteLine("Found " + jsonData?.transmissions.Count + " transmissions");
            Console.WriteLine("Found " + jsonData?.differential_ratios.Count + " differential ratios");
            Console.WriteLine("---------\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue...");
            Console.Read();

            foreach(var truck in jsonData?.trucks) {
                foreach(var transmission in jsonData?.transmissions) {
                    Console.Write($"{truck}: Added {transmission.name}\t");
                    foreach(var differential_ratio in jsonData?.differential_ratios) {

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
                Console.ForegroundColor = RandomConsoleColor();
            }
            var totalCount = jsonData?.trucks.Count * jsonData?.transmissions.Count * jsonData?.differential_ratios.Count;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"\nFinished! (Total: {totalCount})\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
    }
}