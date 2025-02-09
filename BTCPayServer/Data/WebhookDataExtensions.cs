using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTCPayServer.Client.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SshNet.Security.Cryptography;

namespace BTCPayServer.Data
{
    public class AuthorizedWebhookEvents
    {
        public bool Everything { get; set; }

        [JsonProperty(ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public WebhookEventType[] SpecificEvents { get; set; } = Array.Empty<WebhookEventType>();
        public bool Match(WebhookEventType evt)
        {
            return Everything || SpecificEvents.Contains(evt);
        }
    }


    public class WebhookDeliveryBlob
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public WebhookDeliveryStatus Status { get; set; }
        public int? HttpCode { get; set; }
        public string ErrorMessage { get; set; }
        public byte[] Request { get; set; }
        public T ReadRequestAs<T>()
        {
            return JsonConvert.DeserializeObject<T>(UTF8Encoding.UTF8.GetString(Request), HostedServices.WebhookSender.DefaultSerializerSettings);
        }
    }
    public class WebhookBlob
    {
        public string Url { get; set; }
        public bool Active { get; set; } = true;
        public string Secret { get; set; }
        public bool AutomaticRedelivery { get; set; }
        public AuthorizedWebhookEvents AuthorizedEvents { get; set; }
    }
    public static class WebhookDataExtensions
    {
        public static WebhookBlob GetBlob(this WebhookData webhook)
        {
            return webhook.HasTypedBlob<WebhookBlob>().GetBlob();
        }
        public static void SetBlob(this WebhookData webhook, WebhookBlob blob)
        {
            webhook.HasTypedBlob<WebhookBlob>().SetBlob(blob);
        }
        public static WebhookDeliveryBlob GetBlob(this WebhookDeliveryData webhook)
        {
            return webhook.HasTypedBlob<WebhookDeliveryBlob>().GetBlob(HostedServices.WebhookSender.DefaultSerializerSettings);
        }
        public static void SetBlob(this WebhookDeliveryData webhook, WebhookDeliveryBlob blob)
        {
            webhook.HasTypedBlob<WebhookDeliveryBlob>().SetBlob(blob, HostedServices.WebhookSender.DefaultSerializerSettings);
        }
    }
}
