using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;

// Change the Template namespace
namespace CPMPluginTemplate.plugin
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
            int RC = 9999; // default error code

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
                var response = ChangePasswordAsync(username,targetAccountPassword,targetAccountNewPassword,address,username).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    RC = 0; // success
                    Logger.WriteLine("Password change successful.", LogLevel.INFO);
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"Password change failed. ServiceNow returned {response.StatusCode}: {errorContent}", LogLevel.ERROR);

                    platformOutput.Message = $"Password change failed with status {response.StatusCode}";
                    RC = 8001;
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
