using System.Collections.Generic;
using CyberArk.Extensions.Plugins.Models;
using CyberArk.Extensions.Utilties.Logger;
using CyberArk.Extensions.Utilties.Reader;
using System;
using System.Net.Http;

// Change the Template namespace
namespace CPMPluginTemplate.plugin
{
    public class Prereconcile : BaseAction
    {
        #region constructor
        public Prereconcile(List<IAccount> accountList, ILogger logger) : base(accountList, logger) { }
        #endregion 

        #region Setter
        /// <summary>
        /// Defines the Action name that the class is implementing - PreReconcile
        /// </summary>
        override public CPMAction ActionName => CPMAction.prereconcilepass;
        #endregion

        /// <summary>
        /// Plug-in Starting point function.
        /// </summary>
        override public int run(ref PlatformOutput platformOutput)
        {
            Logger.MethodStart();

            int RC = 9999; // default error

            try
            {
                #region Fetch Account Properties

                string username = ParametersAPI.GetMandatoryParameter(USERNAME, ReconcileAccount.AccountProp);
                string address = ParametersAPI.GetMandatoryParameter(ADDRESS, ReconcileAccount.AccountProp);

                #endregion

                #region Fetch Reconcile Account Password

                string reconcileAccountPassword = ReconcileAccount.CurrentPassword.convertSecureStringToString();

                #endregion

                #region Logic

                // Call VerifyCredsAsync and block to get result
                HttpResponseMessage response = VerifyCredsAsync(username, reconcileAccountPassword, address).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"PreReconcile VerifyCreds succeeded. Response content: {content}", LogLevel.INFO);

                    RC = 0; // success
                    platformOutput.Message = "PreReconcile verify passed RC = 0";
                }
                else
                {
                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.WriteLine($"PreReconcile VerifyCreds failed. Status code: {response.StatusCode}, Content: {content}", LogLevel.WARNING);

                    RC = 8600; // example error code
                    platformOutput.Message = $"PreReconcile verification failed. Status code: {response.StatusCode}";
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
