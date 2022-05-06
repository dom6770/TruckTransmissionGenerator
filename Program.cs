using Newtonsoft.Json;

public class TruckTransmissionGenerator {
    static void Main(string[] args) {
        string jsonFile;

        // check if jsonFile has been passed as argument, if not ask for input
        if(args.Length == 0) {
            Console.WriteLine("Please specify json location");
            Console.Write("Input: ");
            jsonFile = Console.ReadLine();
        } else {
            Console.Write("args[0]: ");
            Console.WriteLine(args[0]);
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
            Console.WriteLine("setting templateFolder variable...");
            templateFolder = jsonData?.templateFolder;
            Console.WriteLine("getting transmission templates");
            transmissionsFiles = Directory.GetFiles(templateFolder,"*.sii");

            Console.WriteLine("setting outputFolder variable");
            outputFolder = jsonData?.outputFolder;


            if(Directory.Exists(outputFolder + "/def")) {
                Directory.Delete(outputFolder + "/def",true);
                Console.WriteLine("Folder deleted.");
            }

            Console.Read();

            foreach(var truck in jsonData?.trucks) {
                foreach(var transmission in jsonData?.transmissions) {
                    foreach(var differential_ratio in jsonData?.differential_ratios) {
            
                        Console.WriteLine(transmission.name + " " + differential_ratio);
            
                        string siiFile = File.ReadAllText(templateFolder + @"\" + transmission.name + ".sii");
                        siiFile = siiFile.Replace("@name",transmission.name.ToString());
                        siiFile = siiFile.Replace("@differential_ratio_name",differential_ratio.ToString().Replace(".",""));
                        siiFile = siiFile.Replace("@truck",truck.ToString());
                        siiFile = siiFile.Replace("@displayName",$"\"{transmission.displayName.ToString()}\"");
                        siiFile = siiFile.Replace("@price",transmission.price.ToString());
                        siiFile = siiFile.Replace("@unlock",transmission.unlock.ToString());
                        siiFile = siiFile.Replace("@differential_ratio",differential_ratio.ToString());
            
                        //Console.Write(siiFile);
            
            
                        string outputFolderFull = outputFolder + @"\def\vehicle\truck\" + truck.ToString() + @"\transmission\";
                        string outputFile = outputFolderFull + transmission.name.ToString() + "." + differential_ratio.ToString().Replace(".","") + ".sii";
                        Console.WriteLine(outputFolderFull);
            
                        if(!Directory.Exists(outputFolderFull)) { Directory.CreateDirectory(outputFolderFull); }
                        File.WriteAllText(outputFile,siiFile);
            
                        Console.WriteLine("\n---------------------\n");
                    }
                }
            }

        }
        catch(Exception e) {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
    }
}