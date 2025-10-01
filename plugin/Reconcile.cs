using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;
using System.Net.Http;

namespace ServiceNowPlugin.plugin
{
    public class Reconcile : BaseAction
    {
        #region constructor
        public Reconcile(List<IAccount> accountList, ILogger logger) : base(accountList, logger) { }
        #endregion

        #region Setter
        override public CPMAction ActionName => CPMAction.reconcilepass;
        #endregion

        override public int run(ref PlatformOutput platformOutput)
        {
            Logger.MethodStart();
            int RC = PluginErrors.DEFAULT;

            try
            {
                #region Fetch Account Properties
                string reconUsername = ParametersAPI.GetMandatoryParameter(USERNAME, ReconcileAccount.AccountProp);
                string username = ParametersAPI.GetMandatoryParameter(USERNAME, TargetAccount.AccountProp);
                string address = ParametersAPI.GetMandatoryParameter(ADDRESS, TargetAccount.AccountProp);
                #endregion

                #region Fetch Account's Passwords
                string reconcileAccountPassword = ReconcileAccount.CurrentPassword.convertSecureStringToString();
                string targetAccountNewPassword = TargetAccount.NewPassword.convertSecureStringToString();
                #endregion

                #region Logic - Call ChangePasswordAsync
                var response = ChangePasswordAsync(reconUsername, reconcileAccountPassword, targetAccountNewPassword, address, username).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    RC = PluginErrors.SUCCESS;
                    Logger.WriteLine("Reconcile password change successful.", LogLevel.INFO);
                }
                else
                {
                    string errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"Reconcile password change failed. ServiceNow returned {response.StatusCode}", LogLevel.ERROR);
                    Logger.WriteLine($"Full response content: {errorContent}", LogLevel.INFO);

                    throw new CpmException(PluginErrors.RECON_ERROR);
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