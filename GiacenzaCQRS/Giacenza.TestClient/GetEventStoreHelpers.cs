using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Giacenza.TestClient
{
    public static class GetEventStoreHelpers
    {
        public const string EventClrTypeHeader = "EventClrTypeName";

        public static readonly JsonSerializerSettings SerializerSettings =
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

        public static object DeserializeEvent(byte[] metadata, byte[] data)
        {
            try
            {
                var eventClrTypeName = JObject
                    .Parse(Encoding.UTF8.GetString(metadata))
                    .Property(EventClrTypeHeader).Value;

                return JsonConvert
                    .DeserializeObject(Encoding.UTF8.GetString(data),
                                       Type.GetType((string)eventClrTypeName));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static EventData ToEventData(Guid eventId, object evnt, IDictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));

            var eventHeaders = new Dictionary<string, object>(headers)
                                   {
                                       {
                                           EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName
                                       }
                                   };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            var typeName = evnt.GetType().Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }
    }
}