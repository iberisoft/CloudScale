using CloudScale.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloudScale.Service
{
    class Settings
    {
        public string ServerHost { get; set; }

        public int ServerPort { get; set; }

        public Dictionary<string, GlobalPosition> BeaconPositions { get; set; } = new Dictionary<string, GlobalPosition>();

        public static Settings Default { get; } = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath));

        public void Save() => File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));

        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    }
}
