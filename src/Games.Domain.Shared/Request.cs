using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Games.Domain.Shared;

public static class Request
{
    public static T Get<T>(string url, object query = null, WebHeaderCollection headers = null) {
        url += _buildQueryString(query);
        using (var client = new WebClient()) {
            client.Encoding = Encoding.UTF8;
            if (headers != null) client.Headers.Add(headers);

            var json = client.DownloadString(url);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
    public static T Post<T>(string url, object data) {
        using (var client = new WebClient()) {
            client.Encoding = Encoding.UTF8;
            client.Headers.Set(HttpRequestHeader.ContentType, "application/json");
            string json = "";
            var obj = JsonConvert.SerializeObject(data);
            try {
                json = client.UploadString(url, obj);
                return JsonConvert.DeserializeObject<T>(json);
            }

            catch (WebException ex) {
                throw;
            }
        }
    }
    public static T Put<T>(string url, object data = null) {
        using (var client = new WebClient()) {
            client.Encoding = Encoding.UTF8;
            client.Headers.Set(HttpRequestHeader.ContentType, "application/json");
            string json = "";
            var obj = JsonConvert.SerializeObject(data);
            try {
                if (data == null) {
                    json = client.UploadString(url, "PUT", "");
                }
                else {
                    json = client.UploadString(url, "PUT", obj);
                }
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (WebException ex) {
                throw;
            }
        }
    }

    public static T Delete<T>(string url, object data = null, WebHeaderCollection headers = null) {
        using (var client = new WebClient()) {
            client.Encoding = Encoding.UTF8;
            client.Headers.Set(HttpRequestHeader.ContentType, "application/json");

            if (headers != null)
                client.Headers.Add(headers);

            string json = "";
            string body = data != null ? JsonConvert.SerializeObject(data) : string.Empty;

            try {
                json = client.UploadString(url, "DELETE", body);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (WebException ex) {
                using (var stream = ex.Response?.GetResponseStream())
                using (var reader = new StreamReader(stream ?? Stream.Null)) {
                    var errorBody = reader.ReadToEnd();
                    throw new Exception($"Erro DELETE: {ex.Message} - {errorBody}", ex);
                }
            }
        }
    }


    #region Private Methods
    private static string _buildQueryString(object data) {
        if (data == null)
            return "";
        var queries = new List<string>();
        foreach (var prop in data.GetType().GetProperties()) {
            var key = prop.Name;
            var value = prop.GetValue(data, null);
            queries.Add($"{key}={value}");
        }
        return "?" + string.Join("&", queries);
    }
    #endregion
}
