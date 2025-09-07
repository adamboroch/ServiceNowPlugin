using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.CPMPluginErrorCodeStandarts;
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
                throw new ArgumentException("Username cannot be empty", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty", nameof(address));

            // Clear previous headers
            HttpClient.DefaultRequestHeaders.Clear();

            // UTF-8 encode username:password and set Basic Auth
            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Optional: accept JSON
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Build URI
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
                throw; // let the calling code handle or wrap as needed
            }
        }

        internal async Task<ServiceNowResponseModel> GetUserData(string username ,string password, string address, string usersearch)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty", nameof(address));

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

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);   // GET request

            try
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Logger.WriteLine($"ServiceNow API returned error {response.StatusCode}: {errorContent}",LogLevel.ERROR);
                    throw new ApplicationException($"ServiceNow API returned error {response.StatusCode}: {errorContent}");
                }

                try
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<ServiceNowResponseModel>(content); // create service now response model <ServiceNowResponseModel>- put the json response in the Service Now User model in the format of the list <ServiceNowUserModel>
                }
                catch (JsonException jex)
                {
                    Logger.WriteLine($"Failed to parse ServiceNow response: {jex.Message}", LogLevel.ERROR);
                    throw new ApplicationException("Invalid JSON returned by ServiceNow API.", jex);
                }

            }
            catch (HttpRequestException ex)
            {
                Logger.WriteLine($"Network error during GetUserID: {ex.Message}", LogLevel.ERROR);
                throw;
            }
        }

        internal async Task<HttpResponseMessage> ChangePasswordAsync(string username, string currentPassword, string newPassword, string address, string usersearch)
        {
            try
            {
                // Step 1: Get user data by username - get data including ID that will be used in the PUT request
                var userResponse = await GetUserData(username, currentPassword, address, usersearch);

                if (userResponse == null)
                {
                    throw new ApplicationException("ServiceNow response is null. No data returned.");
                }

                if (userResponse.Data == null)
                {
                    throw new ApplicationException("ServiceNow response.Data is null. Could not deserialize users.");
                }

                if (!userResponse.Data.Any())
                {
                    throw new ApplicationException("ServiceNow response.Data is empty. No users found.");
                }

                var user = userResponse.Data.FirstOrDefault();
                if (user == null)
                {
                    throw new ApplicationException("User object is null after deserialization.");
                }

                // Step 2: Prepare JSON for password change
                var updatedUser = user.GetUser(newPassword);

                var parsedUser = JsonConvert.SerializeObject(
                    updatedUser,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }
                );

                // Step 3: Build URI
                var uriBuilder = new UriBuilder
                {
                    Scheme = "https",
                    Host = address,
                    Path = $"/api/now/table/sys_user/{user.SysId}",
                    Query = "sysparm_input_display_value=true"
                };

                // Step 4: Build PUT request
                var request = new HttpRequestMessage(HttpMethod.Put, uriBuilder.Uri)
                {
                    Content = new StringContent(parsedUser, Encoding.UTF8, "application/json")
                };

                // Basic Auth
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{currentPassword}");
                HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Send request and return response directly
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                return response;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error during password change: {ex.Message}", LogLevel.ERROR);
                throw;
            }
        }

        /// <summary>
        /// Handle the general RC and error message.
        /// </summary>
        internal int HandleGeneralError(Exception ex, ref PlatformOutput platformOutput)
        {
            var errCodeStandards = new ErrorCodeStandards();
            Logger.WriteLine($"Received exception: {ex}.", LogLevel.ERROR);
            platformOutput.Message =
                errCodeStandards.ErrorStandardsDict[PluginErrors.STANDARD_DEFUALT_ERROR_CODE_IDX].ErrorMsg;
            return errCodeStandards.ErrorStandardsDict[PluginErrors.STANDARD_DEFUALT_ERROR_CODE_IDX].ErrorRC;
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