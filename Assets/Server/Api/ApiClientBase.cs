using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace Assets.Server.Api
{
    public abstract class ApiClientBase<T>
    {
        private bool _used = false;

        public Exception Error { get; private set; }
        public T Response { get; private set; }

        public abstract IEnumerator Send();
        
        protected IEnumerator Send(string method, string apiUrl, string path, IDictionary<string, string> query, string json, bool expectResponse)
        {
            // make sure it's one call only
            if (_used)
            {
                throw new Exception("Cannot reuse api client, do not call Send() more than once");
            }
            _used = true;

            string url = BuildUrl(method, apiUrl, path, query);

            using (var request = new UnityWebRequest(url, method))
            {
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                    uploadHandler.contentType = "application/json";
                    request.uploadHandler = uploadHandler;
                }

                var downloadHandler = new DownloadHandlerBuffer();
                request.downloadHandler = downloadHandler;

                // yield the call
                yield return request.SendWebRequest();

                if (request.isHttpError)
                {
                    Error = new Exception("HTTP Error: " + request.error);
                }
                else if (request.isNetworkError)
                {
                    Error = new Exception("Network ERROR: " + request.error);
                }
                else
                {
                    if (expectResponse)
                    {
                        try
                        {
                            Response = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                        }
                        catch (Exception e)
                        {
                            Error = new Exception("Failed to parse http response, see inner exception", e);
                        }
                    }
                }
            }
        }

        private static string BuildUrl(string method, string apiUrl, string path, IDictionary<string, string> query)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new Exception("method parameter cannot be null");
            }
            if (string.IsNullOrEmpty(apiUrl))
            {
                throw new Exception("apiUrl parameter cannot be null");
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("path parameter cannot be null");
            }
            if (path.StartsWith("/"))
            {
                throw new Exception("path parameter should not start with /");
            }

            var builder = new StringBuilder();

            // api address
            builder.Append(apiUrl);

            // path
            if (!apiUrl.EndsWith("/"))
            {
                builder.Append("/");
            }
            builder.Append(path);

            // query (optional)
            if (query.Count > 0)
            {
                builder.Append("?");
                bool firstQuery = true;
                foreach (var queryKeyValue in query)
                {
                    if (!firstQuery)
                    {
                        builder.Append("&");
                    }
                    firstQuery = false;
                    builder.Append(UnityWebRequest.EscapeURL(queryKeyValue.Key) + "=" + UnityWebRequest.EscapeURL(queryKeyValue.Value));
                }
            }

            return builder.ToString();
        }
    }
}
