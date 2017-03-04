using Examples.FileSystem.Plugins;
using Sitecore.DataExchange.Models;

namespace Examples.FileSystem
{
    public static class EndpointExtensions
    {
        public static TextFileSettings GetTextFileSettings(this Endpoint endpoint)
        {
            return endpoint.GetPlugin<TextFileSettings>();
        }
        public static bool HasTextFileSettings(this Endpoint endpoint)
        {
            return (GetTextFileSettings(endpoint) != null);
        }
    }
}
