using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CPMPluginTemplate.plugin
{
    public class CpmException : Exception
    {
        public int ErrorCode { get; }

        public CpmException()
            : base(PluginErrors.PluginErrorMessages[PluginErrors.DEFAULT])
        {
        }

        public CpmException(int errorCode): base(PluginErrors.PluginErrorMessages[errorCode])
        {
            ErrorCode = errorCode;
        }

        public CpmException(int errorCode, Exception innerException): base(PluginErrors.PluginErrorMessages[errorCode], innerException)
        {
            ErrorCode = errorCode;
        }

        public CpmException(int errorCode, string errorMessage): base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public CpmException(int errorCode, string errorMessage, Exception innerException): base(errorMessage, innerException)
        {
            ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return $"[ErrorCode: {ErrorCode}] {Message}";
        }
    }

    public static class PluginErrors
    {
        public static readonly int SUCCESS = 0;
        public static readonly int DEFAULT = 9999;
        public static readonly int UNKNOWN_ERROR = 1000;
        public static readonly int VERIFY_ERROR = 1002;
        public static readonly int INVALID_CREDENTIALS = 1003;
        public static readonly int INVALID_CREDENTIALS_PRERECON = 1005;
        public static readonly int USERSEARCH_INVALID_RESPONSE = 1006;
        public static readonly int USERSEARCH_NOT_FOUND = 1007;
        public static readonly int LOGON_ERROR = 1008;
        public static readonly int CHANGE_ERROR = 1009;
        public static readonly int CHANGE_ERROR_HISTORY = 1010; // optional
        public static readonly int PRERECON_ERROR = 1011;
        public static readonly int RECON_ERROR = 1012;
        public static readonly int RECON_ERROR_HISTORY = 1013; // optional
        public static readonly int INVALID_JSON_RESPONSE = 1014;
        public static readonly int INVALID_JSON_PARSE = 1015; // for parsing-specific failures
        public static readonly int NETWORK_UNAVAILABLE = 1019;
        public static readonly int INVALID_PARAMETER = 1027;

        public static readonly ReadOnlyDictionary<int, string> PluginErrorMessages =
            new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()
            {
                [SUCCESS] = "Operation completed successfully",
                [DEFAULT] = "General plugin error, please refer to the logs for more information",
                [UNKNOWN_ERROR] = "An unknown error has occurred",
                [VERIFY_ERROR] = "Failed to verify credentials, please refer to the logs for more information",
                [INVALID_CREDENTIALS] = "Authentication failed: invalid username, password, or token. Please refer to the logs for more information",
                [INVALID_CREDENTIALS_PRERECON] = "Invalid reconcile username or password",
                [USERSEARCH_INVALID_RESPONSE] = "Received invalid response on user search, please refer to the logs for more information",
                [USERSEARCH_NOT_FOUND] = "Cannot find user, please refer to the logs for more information",
                [LOGON_ERROR] = "Failed to verify credentials before change action, please refer to the logs for more information",
                [CHANGE_ERROR] = "Failed to change the password, please refer to the logs for more information",
                [CHANGE_ERROR_HISTORY] = "Cannot change password - new password is present in password history",
                [PRERECON_ERROR] = "Failed to verify reconcile account credentials, please refer to the logs for more information",
                [RECON_ERROR] = "Failed to reconcile the password, please refer to the logs for more information",
                [RECON_ERROR_HISTORY] = "Cannot reconcile password - new password is present in password history",
                [INVALID_JSON_RESPONSE] = "Unexpected JSON format in response. Check logs for details.",
                [INVALID_JSON_PARSE] = "Error parsing JSON response. Check logs for details.",
                [NETWORK_UNAVAILABLE] = "Network connectivity is unavailable. Check logs for details.",
                [INVALID_PARAMETER] = "Invalid parameter value passed to plugin. Check logs for details."
            });
    }
}
