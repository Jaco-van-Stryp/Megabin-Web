namespace Megabin_Web.Configuration
{
    public class APILimitOptions
    {
        public int AutoCompletePerDayLimit { get; set; } = 30;
        public int RouteGenerationPerDayLimit { get; set; } = 10;
    }
}
