using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;
using System.Net.Http;

namespace ServiceNowPlugin.plugin
{
    public class Prereconcile : BaseAction
    {
        #region constructor
        public Prereconcile(List<IAccount> accountList, ILogger logger) : base(accountList, logger) { }
        #endregion 

        #region Setter
        override public CPMAction ActionName => CPMAction.prereconcilepass;
        #endregion

        override public int run(ref PlatformOutput platformOutput)
        {
            Logger.MethodStart();
            int RC = PluginErrors.DEFAULT; // default error

            try
            {
                #region Fetch Account Properties
                string username = ParametersAPI.GetMandatoryParameter(USERNAME, ReconcileAccount.AccountProp);
                string address = ParametersAPI.GetMandatoryParameter(ADDRESS, ReconcileAccount.AccountProp);
                string reconcileAccountPassword = ReconcileAccount.CurrentPassword.convertSecureStringToString();
                #endregion

                #region Logic
                HttpResponseMessage response = VerifyCredsAsync(username, reconcileAccountPassword, address).GetAwaiter().GetResult();
                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    Logger.WriteLine("PreReconcile verify succeeded", LogLevel.INFO);
                    RC = PluginErrors.SUCCESS;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Logger.WriteLine("Authentication failed", LogLevel.ERROR);
                    Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);

                    throw new CpmException(PluginErrors.AUTH_ERROR_PRERECON);
                }
                else
                {
                    Logger.WriteLine($"PreReconcile verify failed. Status code: {response.StatusCode}", LogLevel.ERROR);
                    Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);

                    throw new CpmException(PluginErrors.PRERECON_ERROR);
                }
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