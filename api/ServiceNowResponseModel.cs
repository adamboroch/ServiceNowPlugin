using Newtonsoft.Json;
using System.Collections.Generic;

namespace CPMPluginTemplate.api
{
    internal class ServiceNowResponseModel
    {
        [JsonProperty("result")]
        public List<ServiceNowUserModel> Data { get; set; }
        
    }
}