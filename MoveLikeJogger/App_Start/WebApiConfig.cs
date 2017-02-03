using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using MoveLikeJogger.DataContracts;
using MoveLikeJogger.OData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MoveLikeJogger
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API config and services

            SetJsonFormatterByDefault(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            RegisterOData(config);
        }

        private static void RegisterOData(HttpConfiguration config)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            ODataModelBuilderHelper.BuildModel(builder);

            config.MapODataServiceRoute(
                routeName: "odata",
                routePrefix: "api",
                model: builder.GetEdmModel());
        }

        private static void SetJsonFormatterByDefault(HttpConfiguration config)
        {
            //Remove default XML handler
            foreach (var match in config
                .Formatters
                .Where(
                    x => x.SupportedMediaTypes.Any(
                        m =>
                            m.MediaType.Equals("application/xml", StringComparison.OrdinalIgnoreCase)
                            || m.MediaType.Equals("text/xml", StringComparison.OrdinalIgnoreCase)))
                .ToArray())
            {
                config.Formatters.Remove(match);
            }

            // register JSON media type formatter
            config.Formatters.Insert(0, new JsonMediaTypeFormatter());

            // ignore reference looping
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                ReferenceLoopHandling.Ignore;

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
