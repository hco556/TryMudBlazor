using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared.Models.TestModels;
    public class ContactsApiResponse
    {
        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }

        [JsonPropertyName("value")]
        public List<Contact> Contacts{ get; set; }
    }

