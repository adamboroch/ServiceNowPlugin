using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.CPMParametersValidation;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CPMPluginTemplate.api;
using System.Linq;

// Change the Template namespace
namespace CPMPluginTemplate.plugin
{
    /*
     * Base Action class should contain common plug-in functionality and parameters.
     * For specific action functionality and parameters use the action classes.
     */
    abstract public class BaseAction : AbsAction
    {
        public static readonly string USERNAME = "username";
        public static readonly string ADDRESS = "address";

        internal ParametersManager ParametersAPI { get; private set; }

        internal HttpClient HttpClient { get; private set; }

        #region constructor
        /// <summary>
        /// BaseAction Ctor. Do not change anything unless you would like to initialize local class members
        /// The Ctor passes the logger module and the plug-in account's parameters to base.
        /// Do not change Ctor's definition not create another.
        /// <param name="accountList"></param>
        /// <param name="logger"></param>
        public BaseAction(List<IAccount> accountList, ILogger logger) : base(accountList, logger)
        {
            // Init ParametersManager
            ParametersAPI = new ParametersManager();
            HttpClient = new HttpClient();
        }
        #endregion

        /// <summary>
        /// Verify credentials against a REST API using Basic Auth (UTF-8 + Base64)
        /// </summary>
        internal async Task<HttpResponseMessage> VerifyCredsAsync(string username, string password, string address)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Logger.WriteLine("Username cannot be empty", LogLevel.WARNING);
                throw new CpmException(PluginErrors.INVALID_CREDENTIALS);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Logger.WriteLine($"Password cannot be empty. Username: {username}, Address: {address}", LogLevel.WARNING);
                throw new CpmException(PluginErrors.INVALID_CREDENTIALS);
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                Logger.WriteLine("Address cannot be empty", LogLevel.WARNING);
                throw new CpmException(PluginErrors.INVALID_PARAMETER);
            }

            HttpClient.DefaultRequestHeaders.Clear();

            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = address,
                Path = "/api/now/table/sys_user",
                Query = "sysparm_limit=1"
            };

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            try
            {
                return await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Logger.WriteLine($"Network error during VerifyCredsAsync: {ex.Message}", LogLevel.ERROR);
                throw new CpmException(PluginErrors.NETWORK_UNAVAILABLE, ex);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Unexpected error during VerifyCredsAsync: {ex.Message}", LogLevel.ERROR);
                throw new CpmException(PluginErrors.UNKNOWN_ERROR, ex);
            }
        }

        internal async Task<ServiceNowResponseModel> GetUserData(string username, string password, string address, string usersearch)
        {
            if (string.IsNullOrWhiteSpace(usersearch))
            {
                Logger.WriteLine("User search value cannot be empty", LogLevel.WARNING);
                throw new CpmException(PluginErrors.USERSEARCH_NOT_FOUND);
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                Logger.WriteLine("Username cannot be empty", LogLevel.WARNING);
                throw new CpmException(PluginErrors.INVALID_CREDENTIALS);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Logger.WriteLine($"Password cannot be empty. Username: {username}, Address: {address}", LogLevel.WARNING);
                throw new CpmException(PluginErrors.INVALID_CREDENTIALS);
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                Logger.WriteLine("Address cannot be empty", LogLevel.WARNING);
                throw new CpmException(PluginErrors.INVALID_PARAMETER);
            }

            HttpClient.DefaultRequestHeaders.Clear();

            // Basic Auth
            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = address,
                Path = "/api/now/table/sys_user",
                Query = $"sysparm_limit=10&user_name={usersearch}"
            };

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri); //GET request

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Logger.WriteLine($"Network error during GET request operation: {ex.Message}", LogLevel.ERROR);
                throw new CpmException(PluginErrors.NETWORK_UNAVAILABLE, ex);
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Check if response is valid JSON
            bool isJson = content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("[");
            if (!isJson)
            {
                Logger.WriteLine("Received invalid JSON response from ServiceNow", LogLevel.ERROR);
                Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);
                throw new CpmException(PluginErrors.INVALID_JSON_RESPONSE);
            }

            if (!response.IsSuccessStatusCode)
            {
                Logger.WriteLine($"ServiceNow API returned error {response.StatusCode}", LogLevel.ERROR);
                Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);
                throw new CpmException(PluginErrors.CHANGE_ERROR);
            }

            try
            {
                // Create ServiceNow response model <ServiceNowResponseModel> - 
                // put the JSON response in the ServiceNow User model in the format of the list <ServiceNowUserModel>
                return JsonConvert.DeserializeObject<ServiceNowResponseModel>(content);
            }
            catch (JsonException jex)
            {
                Logger.WriteLine($"Failed to parse ServiceNow JSON response: {jex.Message}", LogLevel.ERROR);
                Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);
                throw new CpmException(PluginErrors.INVALID_JSON_PARSE, jex);
            }
        }

        internal async Task<HttpResponseMessage> ChangePasswordAsync(string username, string currentPassword, string newPassword, string address, string usersearch)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                Logger.WriteLine("New password cannot be empty", LogLevel.WARNING);
                throw new CpmException(PluginErrors.INVALID_CREDENTIALS);
            }
            // Step 1: Get user data by username
            var userResponse = await GetUserData(username, currentPassword, address, usersearch);

            if (userResponse == null || userResponse.Data == null || !userResponse.Data.Any())
            {
                Logger.WriteLine("No data was returned from the GET API user search operation.", LogLevel.INFO);
                throw new CpmException(PluginErrors.USERSEARCH_INVALID_RESPONSE);
            }

            var user = userResponse.Data.FirstOrDefault();
            if (user == null)
            {
                Logger.WriteLine("User found, but user search data is empty. Returned null.", LogLevel.INFO);
                throw new CpmException(PluginErrors.USERSEARCH_NOT_FOUND);
            }

            // Step 2: Prepare JSON for password change
            var updatedUser = user.GetUser(newPassword);
            var parsedUser = JsonConvert.SerializeObject(updatedUser, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Step 3: Build PUT request
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = address,
                Path = $"/api/now/table/sys_user/{user.SysId}",
                Query = "sysparm_input_display_value=true"
            };

            var request = new HttpRequestMessage(HttpMethod.Put, uriBuilder.Uri)
            {
                Content = new StringContent(parsedUser, Encoding.UTF8, "application/json")
            };

            // Basic Auth
            var byteArray = Encoding.UTF8.GetBytes($"{username}:{currentPassword}");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                return await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Logger.WriteLine($"Network error during ChangePasswordAsync: {ex.Message}", LogLevel.ERROR);
                throw new CpmException(PluginErrors.NETWORK_UNAVAILABLE, ex);
            }
        }

        /// <summary>
        /// Handle the general RC and error message.
        /// </summary>
        internal int HandleError(Exception ex, ref PlatformOutput platformOutput)
        {
            Logger.WriteLine($"Received exception: {ex}", LogLevel.ERROR);

            // Jeśli wyjątek jest już typu CpmException, zwracamy jego kod
            if (ex is CpmException cpmEx)
            {
                platformOutput.Message = PluginErrors.PluginErrorMessages.ContainsKey(cpmEx.ErrorCode)
                    ? PluginErrors.PluginErrorMessages[cpmEx.ErrorCode]
                    : PluginErrors.PluginErrorMessages[PluginErrors.UNKNOWN_ERROR];

                return cpmEx.ErrorCode;
            }

            // Inne wyjątki — zwracamy default
            platformOutput.Message = PluginErrors.PluginErrorMessages[PluginErrors.DEFAULT];
            return PluginErrors.DEFAULT;
        }

        ~BaseAction()
        {
            if (HttpClient != null)
            {
                HttpClient.Dispose();
            }
        }
    }
}