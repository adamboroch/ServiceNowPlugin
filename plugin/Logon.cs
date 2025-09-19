using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;
using System.Net.Http;

namespace CPMPluginTemplate.plugin
{
    public class Logon : BaseAction
    {
        #region constructor
        public Logon(List<IAccount> accountList, ILogger logger) : base(accountList, logger) { }
        #endregion

        #region Setter
        override public CPMAction ActionName => CPMAction.logon;
        #endregion

        override public int run(ref PlatformOutput platformOutput)
        {
            Logger.MethodStart();
            int RC = 9999; // default error

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
                // Call VerifyCredsAsync and block since run() is synchronous
                HttpResponseMessage response = VerifyCredsAsync(username, targetAccountPassword, address).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"Logon succeeded. Response content: {content}", LogLevel.INFO);

                    RC = 0;
                    platformOutput.Message = "Logon passed RC = 0;";
                }
                else
                {
                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"Logon failed. Status code: {response.StatusCode}, Content: {content}", LogLevel.WARNING);

                    RC = 8600; // example error code
                    platformOutput.Message = $"Logon failed. Status code: {response.StatusCode}";
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
