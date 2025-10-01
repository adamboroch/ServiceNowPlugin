using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;
using System.Net.Http;

namespace ServiceNowPlugin.plugin
{
    public class Logon : BaseAction
    {
        #region constructor
        public Logon(List<IAccount> accountList, ILogger logger) : base(accountList, logger) { }
        #endregion

        #region Setter
        override public CPMAction ActionName => CPMAction.logon;
        #endregion

        public override int run(ref PlatformOutput platformOutput)
        {
            Logger.MethodStart();
            int RC = PluginErrors.DEFAULT; // default error

            try
            {
                #region Fetch Account Properties
                string username = ParametersAPI.GetMandatoryParameter(USERNAME, TargetAccount.AccountProp);
                string address = ParametersAPI.GetMandatoryParameter(ADDRESS, TargetAccount.AccountProp);
                #endregion

                #region Fetch Account Password
                string targetAccountPassword = TargetAccount.CurrentPassword.convertSecureStringToString();
                #endregion

                #region Logic
                HttpResponseMessage response = VerifyCredsAsync(username, targetAccountPassword, address).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                // Check if response is valid JSON
                bool isJson = content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("[");
                if (!isJson)
                {
                    Logger.WriteLine("Received invalid JSON response", LogLevel.ERROR);
                    Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);

                    throw new CpmException(PluginErrors.INVALID_JSON_RESPONSE);
                }

                // If response is not successful
                if (!response.IsSuccessStatusCode)
                {
                    Logger.WriteLine($"Logon failed. Status code: {response.StatusCode}", LogLevel.ERROR);
                    Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);

                    throw new CpmException(PluginErrors.LOGON_ERROR);
                }

                // Success
                Logger.WriteLine("Logon succeeded", LogLevel.INFO);
                RC = PluginErrors.SUCCESS;
                #endregion
            }
            catch (Exception ex)
            {
                RC = HandleError(ex, ref platformOutput);
            }
            finally
            {
                Logger.MethodEnd();
            }

            return RC;
        }
    }
}