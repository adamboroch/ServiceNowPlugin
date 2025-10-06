using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;

namespace ServiceNowPlugin.plugin
{
    public class Change : BaseAction
    {
        #region constructor
        public Change(List<IAccount> accountList, ILogger logger) : base(accountList, logger) { }
        #endregion

        #region Setter
        /// <summary>
        /// Defines the Action name that the class is implementing - Change
        /// </summary>
        override public CPMAction ActionName => CPMAction.changepass;
        #endregion

        /// <summary>
        /// Plug-in Starting point function.
        /// </summary>
        override public int run(ref PlatformOutput platformOutput)
        {
            Logger.MethodStart();
            int RC = PluginErrors.DEFAULT; // default error code

            try
            {
                #region Fetch Account Properties
                string username = ParametersAPI.GetMandatoryParameter(USERNAME, TargetAccount.AccountProp);
                string address = ParametersAPI.GetMandatoryParameter(ADDRESS, TargetAccount.AccountProp);
                #endregion

                #region Fetch Account's Passwords
                string targetAccountPassword = TargetAccount.CurrentPassword.convertSecureStringToString();
                string targetAccountNewPassword = TargetAccount.NewPassword.convertSecureStringToString();
                #endregion

                #region Logic - Call ChangePasswordAsync
                var response = ChangePasswordAsync(username, targetAccountPassword, targetAccountNewPassword, address, username).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    if (!content.Contains("user_password"))
                    {
                        Logger.WriteLine("WARNING: Password change request returned 200 but no user_password field — change not applied.", LogLevel.WARNING);
                        Logger.WriteLine($"ServiceNow ignored the update to the 'user_password' field because the user does not have write access to this field in the 'sys_user' table. RESPONSE: {content}", LogLevel.ERROR);
    
                        throw new CpmException(PluginErrors.CHANGE_ERROR_PERM);
                    }
                    RC = PluginErrors.SUCCESS; // success = no error
                    Logger.WriteLine("Password changed successfully.", LogLevel.INFO);
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"Password change failed with status {response.StatusCode}", LogLevel.ERROR);
                    Logger.WriteLine($"Password change failed. ServiceNow returned {response.StatusCode}: {errorContent}", LogLevel.INFO);
                    Logger.WriteLine("Check account permissions", LogLevel.INFO);
                    throw new CpmException(PluginErrors.CHANGE_ERROR);
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