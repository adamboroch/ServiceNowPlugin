using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;
using System.Net.Http;

// Change the Template namespace
namespace CPMPluginTemplate.plugin
{
    public class Verify : BaseAction
    {
        #region constructor
        /// <summary>
        /// Logon Ctor. Do not change anything unless you would like to initialize local class members
        /// </summary>
        public Verify(List<IAccount> accountList, ILogger logger) : base(accountList, logger) { }
        #endregion

        #region Setter
        /// <summary>
        /// Defines the Action name that the class is implementing - Verify
        /// </summary>
        override public CPMAction ActionName => CPMAction.verifypass;
        #endregion

        /// <summary>
        /// Plug-in Starting point function.
        /// </summary>
        /// <param name="platformOutput"></param>
        override public int run(ref PlatformOutput platformOutput)
        {
            Logger.MethodStart();

            int RC = 9999; // default error

            try
            {
                #region Fetch Account Properties (FileCategories)

                // Fetch mandatory username
                string username = ParametersAPI.GetMandatoryParameter(USERNAME, TargetAccount.AccountProp);

                // Fetch mandatory address (target host)
                string address = ParametersAPI.GetMandatoryParameter(ADDRESS, TargetAccount.AccountProp);

                #endregion

                #region Fetch Account's Passwords

                string targetAccountPassword = TargetAccount.CurrentPassword.convertSecureStringToString();

                #endregion

                #region Logic

                // Call VerifyCredsAsync and block to get result (because run() is sync)
                HttpResponseMessage response = VerifyCredsAsync(username, targetAccountPassword, address).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"VerifyCreds succeeded. Response content: {content}", LogLevel.INFO);

                    // Success RC
                    RC = 0;
                    platformOutput.Message = "Verify passed RC = 0"; // optional
                }
                else
                {
                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"VerifyCreds failed. Status code: {response.StatusCode}, Content: {content}", LogLevel.WARNING);

                    RC = 8600; // example error code
                    platformOutput.Message = $"Verification failed. Status code: {response.StatusCode}";
                }

                #endregion Logic
            }
            catch (Exception ex)
            {
                RC = HandleGeneralError(ex, ref platformOutput);
            }
            finally
            {
                Logger.MethodEnd();
            }

            return RC;
        }
    }
}