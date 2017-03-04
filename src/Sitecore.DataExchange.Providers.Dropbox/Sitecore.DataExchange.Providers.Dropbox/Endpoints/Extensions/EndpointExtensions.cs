using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Providers.Dropbox.Plugins;

namespace Sitecore.DataExchange.Providers.Dropbox.Endpoints.Extensions
{
    public static class EndpointExtensions
    {
        public static DropboxSettings GetDropboxSettings(this Endpoint endpoint)
        {
            return endpoint.GetPlugin<DropboxSettings>();
        }
        public static bool HasDropboxSettings(this Endpoint endpoint)
        {
            return (GetDropboxSettings(endpoint) != null);
        }
    }
}