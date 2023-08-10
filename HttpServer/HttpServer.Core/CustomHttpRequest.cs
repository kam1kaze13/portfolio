using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class CustomHttpRequest
    {
        public CustomHttpRequest()
        {
            this.Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public string Method { get; set; }

        public string RequestUri { get; set; }

        public string HttpVersion { get; set; }

        public Dictionary<string,string> Headers { get; set; }

        public RangeInfo Range
        {
            get
            {
                var range = this.GetHeaderValue(HttpHeaders.Range);
                long begin, end;
                if (range != null)
                {
                    try
                    {
                        begin = Convert.ToInt64(range.Substring(range.IndexOf("=") + 1, range.IndexOf("-") - range.IndexOf("=") - 1));
                    }
                    catch
                    {
                        begin = -1;
                    }

                    try
                    {
                        end = Convert.ToInt64(range.Substring(range.IndexOf("-") + 1));
                    }
                    catch
                    {
                        end = -1;
                    }

                    return new RangeInfo { Begin = begin, End = end };
                }
                else
                    return null;                                
            }
            set
            {
                this.SetHeaderValue(HttpHeaders.Range, value);
            }
        }

        public class RangeInfo
        {
            public long Begin { get; set; }

            public long End { get; set; }
        }


        public int ContentLength 
        {
            get
            {
                string contentLength = this.GetHeaderValue(HttpHeaders.ContentLength);
                return contentLength == null ? -1 : Convert.ToInt32(contentLength);
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
                switch (this.HttpVersion)
                {
                    case "1.0":
                        {
                            var connection = this.GetHeaderValue(HttpHeaders.Connection);
                            return connection == "Keep-Alive" ? true : false;
                        }
                    case "1.1":
                        {
                            return true;
                        }
                }
                return false;
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

        private string GetHeaderValue(string header)
        {
            string outValue;
            this.Headers.TryGetValue(header, out  outValue);

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
