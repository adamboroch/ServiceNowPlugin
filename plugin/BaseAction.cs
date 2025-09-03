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
            HttpClient = new HttpClient(); // ideally a static/shared HttpClient, but keeping instance per object for simplicity
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