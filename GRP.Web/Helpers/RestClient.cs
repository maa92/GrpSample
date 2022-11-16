using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GrpSample.Web.Helpers
{
    public class RestClient<T> where T : class
    {
        private string _baseAddress;
        private int _clientTimeoutMinutes;
        private IOwinContext _context;

        public RestClient()
        {
            _baseAddress = ConfigurationManager.AppSettings["apiUrl"];
            int.TryParse(ConfigurationManager.AppSettings["restClientTimeoutMin"], out _clientTimeoutMinutes);
            if (_clientTimeoutMinutes == 0)
                _clientTimeoutMinutes = 5;
        }
        public RestClient(string BaseAddress)
        {
            _baseAddress = BaseAddress;
            int.TryParse(ConfigurationManager.AppSettings["restClientTimeoutMin"], out _clientTimeoutMinutes);
            if (_clientTimeoutMinutes == 0)
                _clientTimeoutMinutes = 5;
        }

        public RestClient(IOwinContext Context) : this (ConfigurationManager.AppSettings["apiUrl"])
        {
            _context = Context;
        }

        public async Task<T> AuthenticatePost(string apiMethod, HttpContent postContent, string AuthHeader, string AuthHeaderValue, Dictionary<string, string> customHeaders)
        {
            T result = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();

                if (customHeaders != null && customHeaders.Count > 0)
                    foreach (string key in customHeaders.Keys)
                        client.DefaultRequestHeaders.Add(key, customHeaders[key]);

                var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthHeaderValue));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthHeader, authorizationHeader);

                var response = await client.PostAsync(apiMethod, postContent).ConfigureAwait(false);//, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                //response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;

                    result = JsonConvert.DeserializeObject<T>(x.Result);
                });
            }

            return result;
        }

        private void SetupClient(HttpClient client, Dictionary<string,string> CustomHeaders)
        {
            client.Timeout = TimeSpan.FromMinutes(_clientTimeoutMinutes);
            client.BaseAddress = new Uri(_baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string at = _context.Authentication.User.Claims.First(c => c.Type == "uat").Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", at);

            if (CustomHeaders != null && CustomHeaders.Count > 0)
                foreach (string key in CustomHeaders.Keys)
                    client.DefaultRequestHeaders.Add(key, CustomHeaders[key]);
        }

        public async Task<T> PostRequest(string apiMethod, Object postContent, Dictionary<string, string> customHeaders = null)
        {
            T result = null;

            using (var client = new HttpClient())
            {
                SetupClient(client, customHeaders);

                //ObjectContent jc = new ObjectContent(postContent.GetType(), postContent, new JsonMediaTypeFormatter());

                var response = await client.PostAsync(apiMethod, postContent, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                    await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                    {
                        if (x.IsFaulted)
                            throw x.Exception;

                        if (typeof(T) == typeof(string))
                            result = (T)Convert.ChangeType(x.Result, typeof(T));
                        else
                            result = JsonConvert.DeserializeObject<T>(x.Result);
                    });
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _context.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        //_context.Response.Redirect("/User/Login");
                    }
                }
            }

            return result;
        }

        public async Task<T> PostRequest(string apiMethod, T postObject, Dictionary<string, string> customHeaders = null)
        {
            T result = null;

            using (var client = new HttpClient())
            {
                SetupClient(client, customHeaders);

                var response = await client.PostAsync(apiMethod, postObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                    await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                    {
                        if (x.IsFaulted)
                            throw x.Exception;

                        result = JsonConvert.DeserializeObject<T>(x.Result);
                    });
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _context.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        //_context.Response.Redirect("/User/Login");
                    }
                }
            }

            return result;
        }

        public async Task<T> GetSingleItemRequest(string apiMethod, Dictionary<string, string> customHeaders = null)
        {
            T result = null;

            using (var client = new HttpClient())
            {
                SetupClient(client, customHeaders);

                var response = await client.GetAsync(apiMethod).ConfigureAwait(false);

                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                    await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;

                    result = JsonConvert.DeserializeObject<T>(x.Result);
                });
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _context.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        //_context.Response..Redirect("/User/Login");
                    }
                }
            }

            return result;
        }

        
        public async Task<T[]> GetMultipleItemsRequest(string apiMethod, Dictionary<string, string> customHeaders = null)
        {
            T[] result = null;

            using (var client = new HttpClient())
            {
                SetupClient(client, customHeaders);

                var response = await client.GetAsync(apiMethod).ConfigureAwait(false);

                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                    await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                    {
                        if (x.IsFaulted)
                            throw x.Exception;

                        result = JsonConvert.DeserializeObject<T[]>(x.Result);
                    });
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _context.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        //_context.Response.Redirect("/User/Login");
                    }
                }
            }

            return result;
        }

        ///// <summary>
        ///// For creating a new item over a web api using POST
        ///// </summary>
        ///// <param name="apiUrl">Added to the base address to make the full url of the 
        ///// api post method, e.g. "products" to add products</param>
        ///// <param name="postObject">The object to be created</param>
        ///// <returns>The item created</returns>
        //public async Task<T> PostRequest(string apiUrl, T postObject)
        //{
        //    T result = null;

        //    using (var client = new HttpClient())
        //    {
        //        SetupClient(client, "POST", apiUrl, postObject);

        //        var response = await client.PostAsync(apiUrl, postObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

        //        response.EnsureSuccessStatusCode();

        //        await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
        //        {
        //            if (x.IsFaulted)
        //                throw x.Exception;

        //            result = JsonConvert.DeserializeObject<T>(x.Result);

        //        });
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// For updating an existing item over a web api using PUT
        ///// </summary>
        ///// <param name="apiUrl">Added to the base address to make the full url of the 
        ///// api put method, e.g. "products/3" to update product with id of 3</param>
        ///// <param name="putObject">The object to be edited</param>
        //public async Task PutRequest(string apiUrl, T putObject)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        SetupClient(client, "PUT", apiUrl, putObject);

        //        var response = await client.PutAsync(apiUrl, putObject);//, new JsonMediaTypeFormatter()).ConfigureAwait(false);

        //        response.EnsureSuccessStatusCode();
        //    }
        //}

        ///// <summary>
        ///// For deleting an existing item over a web api using DELETE
        ///// </summary>
        ///// <param name="apiUrl">Added to the base address to make the full url of the 
        ///// api delete method, e.g. "products/3" to delete product with id of 3</param>
        //public async Task DeleteRequest(string apiUrl)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        SetupClient(client, "DELETE", apiUrl);

        //        var response = await client.DeleteAsync(apiUrl).ConfigureAwait(false);

        //        response.EnsureSuccessStatusCode();
        //    }
        //}
    }
}

#region Commented for now code Hashing.

//This is the source for the ActionFilter:


//using System;
//using System.Configuration;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Web.Http.Filters;

//namespace WebApiAuthentication
//{
//    /// <summary>
//    /// Can be used to decorate a web api controller or controller method. 
//    /// 
//    /// If HmacSecret is false or not specified it will simply check if the header contains 
//    /// a SecretToken value that is the  same as what is held in the item with the name 
//    /// contained in SharedSecretName in the web.config appsettings
//    /// 
//    /// If HmacSecret is true it takes things further by checking the header of the
//    /// message contains a SecretToken value that is a HMAC of the message generated
//    /// using the value in the SharedSecretName in the web.config appsettings as the key.
//    /// </summary>
//    public class SecretAuthenticationFilter : ActionFilterAttribute
//    {
//        // The name of the web.config item where the shared secret is stored
//        public string SharedSecretName { get; set; }
//        public bool HmacSecret { get; set; }

//        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
//        {
//            // We can only validate if the action filter has had this passed in
//            if (!string.IsNullOrWhiteSpace((SharedSecretName)))
//            {
//                // Name of meta data to appear in header of each request
//                const string secretTokenName = "SecretToken";

//                var goodRequest = false;

//                // The request should have the secretTokenName in the header containing the shared secret
//                if (actionContext.Request.Headers.Contains(secretTokenName))
//                {
//                    var messageSecretValue = actionContext.Request.Headers.GetValues(secretTokenName).First();
//                    var sharedSecretValue = ConfigurationManager.AppSettings[SharedSecretName];

//                    if (HmacSecret)
//                    {
//                        Stream reqStream = actionContext.Request.Content.ReadAsStreamAsync().Result;
//                        if (reqStream.CanSeek)
//                        {
//                            reqStream.Position = 0;
//                        }

//                        //now try to read the content as string
//                        string content = actionContext.Request.Content.ReadAsStringAsync().Result;
//                        var contentMD5 = content == "" ? "" : Hashing.GetHashMD5OfString(content);
//                        var datePart = "";
//                        var requestDate = DateTime.Now.AddDays(-2);
//                        if (actionContext.Request.Headers.Date != null)
//                        {
//                            requestDate = actionContext.Request.Headers.Date.Value.UtcDateTime;
//                            datePart = requestDate.ToString(CultureInfo.InvariantCulture);
//                        }
//                        var methodName = actionContext.Request.Method.Method;
//                        var fullUri = actionContext.Request.RequestUri.ToString();

//                        var messageRepresentation =
//                            methodName + "\n" +
//                            contentMD5 + "\n" +
//                            datePart + "\n" +
//                            fullUri;

//                        var expectedValue = Hashing.GetHashHMACSHA256OfString(messageRepresentation, sharedSecretValue);

//                        // Are the hmacs the same, and have we received it within +/- 5 mins (sending and
//                        // receiving servers may not have exactly the same time)
//                        if (messageSecretValue == expectedValue
//                            && requestDate > DateTime.UtcNow.AddMinutes(-5)
//                            && requestDate < DateTime.UtcNow.AddMinutes(5))
//                            goodRequest = true;
//                    }
//                    else
//                    {
//                        if (messageSecretValue == sharedSecretValue)
//                            goodRequest = true;
//                    }
//                }

//                if (!goodRequest)
//                {
//                    var request = actionContext.Request;
//                    var actionName = actionContext.ActionDescriptor.ActionName;
//                    var controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
//                    var moduleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

//                    var errorMessage = string.Format(
//                        "Error validating request to {0}:{1}:{2}",
//                        moduleName, controllerName, actionName);

//                    var errorResponse = request.CreateErrorResponse(HttpStatusCode.Forbidden, errorMessage);

//                    // Force a wait to make a brute force attack harder
//                    Thread.Sleep(2000);

//                    actionContext.Response = errorResponse;
//                }
//            }

//            base.OnActionExecuting(actionContext);
//        }
//    }
//}


//This is the source for the utility hashing functions:

#endregion