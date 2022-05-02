using System.Data;
using Newtonsoft.Json;
public class TruckTransmissionGenerator {
    static void Main(string[] args) {
        string jsonFile = @"C:\Users\Dominik\OneDrive - Dominik Pflegerl\Projects\Modding\American Truck Simulator\Allison Transmission\data\data.json";
        string templateFolder = @"C:\Users\Dominik\OneDrive - Dominik Pflegerl\Projects\Modding\American Truck Simulator\Allison Transmission\data\transmission_templates";
        string outputFolder = @"C:\Users\Dominik\OneDrive - Dominik Pflegerl\Projects\Modding\American Truck Simulator\Allison Transmission\output";
        string[] transmissionsFiles = Directory.GetFiles(templateFolder, "*.sii");

        StreamReader r = new StreamReader(jsonFile);
        string jsonString = r.ReadToEnd();
        dynamic jsonData = JsonConvert.DeserializeObject(jsonString);
        
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
        


        //Array.ForEach(transmissionsFiles,Console.WriteLine);
        //
        //string siiFile = File.ReadAllText(transmissionsFiles[0]);
        //siiFile = siiFile.Replace("@name",jsonData.transmissions[0].name.ToString());
        //siiFile = siiFile.Replace("$differential_ratio_name",jsonData.differential_ratios[0].ToString().Replace(".",""));
        //siiFile = siiFile.Replace("$truck",jsonData.trucks[8].ToString());
        //siiFile = siiFile.Replace("$displayName",$"\"{jsonData.transmissions[0].displayName.ToString()}\"");
        //siiFile = siiFile.Replace("@price",jsonData.transmissions[0].price.ToString());
        //siiFile = siiFile.Replace("@unlock",jsonData.transmissions[0].unlock.ToString());
        //siiFile = siiFile.Replace("$differential_ratio",jsonData.differential_ratios[0].ToString());
        //
        //Console.Write(siiFile);
        //
        //
        //
        //File.WriteAllText(templateFolder + @"\test.txt",siiFile);

    }
}