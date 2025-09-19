using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;
using System.Net.Http;

namespace CPMPluginTemplate.plugin
{
    public class Verify : BaseAction
    {
        #region Constructor
        /// <summary>
        /// Logon constructor
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

            int RC = PluginErrors.DEFAULT; // default error

            try
            {
                #region Fetch Account Properties

                string username = ParametersAPI.GetMandatoryParameter(USERNAME, TargetAccount.AccountProp);
                string address = ParametersAPI.GetMandatoryParameter(ADDRESS, TargetAccount.AccountProp);

                #endregion

                #region Fetch Account's Passwords

                string targetAccountPassword = TargetAccount.CurrentPassword.convertSecureStringToString();

                #endregion

                #region Logic

                HttpResponseMessage response = VerifyCredsAsync(username, targetAccountPassword, address)
                                                .GetAwaiter().GetResult();
                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    Logger.WriteLine($"VerifyCreds succeeded. Response content: {content}", LogLevel.INFO);
                    RC = PluginErrors.SUCCESS;
                    platformOutput.Message = "Verify passed RC = 0";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Logger.WriteLine("Authentication failed", LogLevel.WARNING);
                    Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);

                    throw new CpmException(PluginErrors.INVALID_CREDENTIALS);
                }
                else
                {
                    // Check if response is valid JSON
                    bool isJson = content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("[");
                    if (!isJson)
                    {
                        Logger.WriteLine("Received invalid JSON response", LogLevel.WARNING);
                        Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);

                        throw new CpmException(PluginErrors.INVALID_JSON_RESPONSE);
                    }

                    Logger.WriteLine($"VerifyCreds failed. Status code: {response.StatusCode}", LogLevel.WARNING);
                    Logger.WriteLine($"Full response content: {content}", LogLevel.INFO);

                    throw new CpmException(PluginErrors.VERIFY_ERROR);
                }

                #endregion
            }
            catch (CpmException ex)
            {
                // Already a known plugin exception
                RC = ex.ErrorCode;
                platformOutput.Message = ex.Message;
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
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
