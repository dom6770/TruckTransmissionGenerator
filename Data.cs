using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TruckTransmissionGenerator {
    internal class Data {
        public string JsonFile { get; set; } = "data.json";
        public dynamic JsonData { get; set; } = default!;
        public string TemplateFolder { get; set; } = string.Empty;
        public string OutputFolder { get; set; } = string.Empty;
        public string[] TransmissionsFiles { get; set; } = new string[0];
    }
}
