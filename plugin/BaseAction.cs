using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.CPMPluginErrorCodeStandarts;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.CPMParametersValidation;
using System;
using Newtonsoft.Json;
using System.Net.Http;

// Change the Template name space
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

        internal HttpResponseMessage VerifyCreds(string username, string password, string address)
        {             
            HttpClient.DefaultRequestHeaders.Clear();
            var byteArray = System.Text.Encoding.ASCII.GetBytes($"{username}:{password}");
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = address,
                Path = "/api/now/table/sys_user",
                Query = "sysparm_limit=1"
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return HttpClient.SendAsync(request).Result;
        }

        /// <summary>
        /// Handle the general RC and error message.
        /// <summary>
        /// <param name="ex"></param>
        /// <param name="platformOutput"></param>
        internal int HandleGeneralError(Exception ex, ref PlatformOutput platformOutput)
        {
            ErrorCodeStandards errCodeStandards = new ErrorCodeStandards();
            Logger.WriteLine(string.Format("Received exception: {0}.", ex), LogLevel.ERROR);
            platformOutput.Message = errCodeStandards.ErrorStandardsDict[PluginErrors.STANDARD_DEFUALT_ERROR_CODE_IDX].ErrorMsg;
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
