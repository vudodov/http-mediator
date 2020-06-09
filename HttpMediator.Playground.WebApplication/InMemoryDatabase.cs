using System.Collections.Generic;

namespace HttpMediator.Playground.WebApp
{
    internal static class InMemoryDatabase
    {
        public static Dictionary<string, dynamic> Database { get; } = new Dictionary<string, dynamic>();

        public static void PopulateWithData()
        {
            Database["human_friendly"] = "Unknown";
            Database["temperature"] = 0;
            Database["temperature_log"] = new List<float>();
        }
    }
}