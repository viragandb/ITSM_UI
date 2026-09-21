using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class State
    {
        [JsonProperty(PropertyName = "opened")]
        public bool Opened { get; set; }

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled { get; set; }

        [JsonProperty(PropertyName = "selected")]
        public bool Selected { get; set; }
    }
    public class JsTreeViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "parent")]
        public string Parent { get; set; }

        [JsonProperty(PropertyName = "state")]
        public State State { get; set; }
    }
}
