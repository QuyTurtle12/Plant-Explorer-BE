namespace Plant_Explorer.Core.Utils
{
    public class RegionHelper
    {
        // Use a single Random instance to avoid issues with multiple re-seedings.
        private static readonly Random rnd = new Random();

        public static string GetRegions()
        {
            // Define a list of regions excluding those without significant plant life (e.g. Antarctica and Africa).
            string[] regions = new string[]
            {
            "North America",
            "South America",
            "Europe",
            "Asia",
            "Middle East",
            "Central America",
            "Central Asia",
            "Southeast Asia",
            "East Asia",
            "South Asia",
            "North Asia",
            "Western Europe",
            "Eastern Europe",
            "Scandinavia"
            };

            // Shuffle the list and take three unique regions.
            var randomRegions = regions.OrderBy(x => rnd.Next()).Take(3).ToArray();

            // Return the three regions as a comma-separated string.
            return string.Join(", ", randomRegions);
        }
        public static string GetHabitat()
        {
            // Define a list of habitats
            string[] habitats = new string[]
            {
                "Forest",
                "Wetlands",
                "Grassland",
                "Mountain",
                "Savanna",
                "Rainforest"
            };

            // Select a random habitat
            return habitats[rnd.Next(habitats.Length)];
        }
    }
}

