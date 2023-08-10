using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class CustomHttpResponse
    {
        public CustomHttpResponse()
        {
            Headers = new Dictionary<string, string>();
        }

        public string HttpVersion { get; set; }

        public int StatusCode { get; set; }

        public string ReasonPhrase { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public DateTime Date
        {
            get
            {
                return Convert.ToDateTime(this.GetHeaderValue(HttpHeaders.Date));
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.Date, value);
            }
        }

        public string AcceptRanges
        {
            get
            {
                return this.GetHeaderValue(HttpHeaders.AcceptRanges);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.AcceptRanges, value);
            }
        }

        public int ContentLength
        {
            get
            {
                string contentLength = this.GetHeaderValue(HttpHeaders.ContentLength);
                return contentLength == null ? -1 : Convert.ToInt32(contentLength);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.ContentLength, value);
            }
        }

        public string Body { get; set; }

        public string Accept
        {
            get
            {
                return this.GetHeaderValue(HttpHeaders.Accept);
            }
        }

        public string Host
        {
            get
            {
                return this.GetHeaderValue(HttpHeaders.Host);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.Host, value);
            }
        }

        public DateTime? IfModifiedSince
        {
            get
            {
                var date = this.GetHeaderValue(HttpHeaders.IfModifiedSince);
                return date == null ? null : Convert.ToDateTime(date);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.IfModifiedSince, value.HasValue ? value.Value.ToUniversalTime().ToString("r") : null);
            }
        }

        public bool KeepAlive
        {
            get
            {
                var connection = this.GetHeaderValue(HttpHeaders.Connection);
                return connection == "Keep-Alive" ? true : false;
            }
            set { }
        }

        public string Connection
        {
            get
            {
                return this.GetHeaderValue(HttpHeaders.Connection);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.Connection, value);
            }
        }

        public string TransferEncoding
        {
            get
            {
                return this.GetHeaderValue(HttpHeaders.TransferEncoding);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.TransferEncoding, value);
            }
        }

        public string UserAgent
        {
            get
            {
                return this.GetHeaderValue(HttpHeaders.UserAgent);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.UserAgent, value);
            }
        }

        public string ServerName
        {
            get
            {
                return this.GetHeaderValue(HttpHeaders.ServerName);
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.ServerName, value);
            }
        }

        private string GetHeaderValue(string header)
        {
            string outValue;
            this.Headers.TryGetValue(header, out outValue);

            return outValue;
        }

        private void SetHeaderValue(string header, object value)
        {
            if (this.Headers.ContainsKey(header))
            {
                this.Headers[header] = value.ToString();
            }
            else
            {
                this.Headers.Add(header, value.ToString());
            }
        }
    }
}
