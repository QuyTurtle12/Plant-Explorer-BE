using System.Text.Json.Serialization;

namespace Plant_Explorer.Core.Utils
{
    public class CachedImage
    {
        public byte[] ImageBytes { get; set; }  //Cache image into memory using stream
        public string AccessToken { get; set; } //Cache access token to get plant infors
    }
    public class PlantIdIdentifyResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
    public class PlantIdResultsResponse
    {
        [JsonPropertyName("result")]
        public PlantResult? Result { get; set; }
    }
    public class PlantResult
    {
        [JsonPropertyName("classification")]
        public Classification? Classification { get; set; }
    }
    public class Classification
    {
        [JsonPropertyName("suggestions")]
        public List<Prediction>? Suggestions { get; set; }
    }
    public class Prediction
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("probability")]
        public double Probability { get; set; }
    }

    public class GbifMatchResponse
    {
        [JsonPropertyName("usageKey")]
        public int? UsageKey { get; set; }
    }

    public class GbifSpeciesResponse
    {
        [JsonPropertyName("vernacularName")]
        public string? VernacularName { get; set; } // Change to string

        [JsonPropertyName("family")]
        public string? Family { get; set; }

        [JsonPropertyName("species")]
        public string? Species { get; set; }
    }


    public class VernacularName
    {
        [JsonPropertyName("vernacularName")]
        public string? Name { get; set; }
    }

}
