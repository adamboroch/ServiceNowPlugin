using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceNowPlugin.api
{
    internal class ServiceNowResponseModel
    {
        [JsonProperty("result")]
        public List<ServiceNowUserModel> Data { get; set; }
        
    }
}