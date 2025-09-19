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

        public CpmException(int errorCode)
            : base(PluginErrors.PluginErrorMessages[errorCode])
        {
            ErrorCode = errorCode;
        }

        public CpmException(int errorCode, Exception innerException)
            : base(PluginErrors.PluginErrorMessages[errorCode], innerException)
        {
            ErrorCode = errorCode;
        }

        public CpmException(int errorCode, string errorMessage)
            : base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public CpmException(int errorCode, string errorMessage, Exception innerException)
            : base(errorMessage, innerException)
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
        public static readonly int HTTP_REQUEST = 1001;
        public static readonly int VERIFY_ERROR = 1002;
        public static readonly int INVALID_CREDENTIALS = 1003;
        public static readonly int INVALID_CREDENTIALS_LOGON = 1004;
        public static readonly int INVALID_CREDENTIALS_PRERECON = 1005;
        public static readonly int USERSEARCH_INVALID_RESPONSE = 1006;
        public static readonly int USERSEARCH_NOT_FOUND = 1007;
        public static readonly int LOGON_ERROR = 1008;
        public static readonly int CHANGE_ERROR = 1009;
        public static readonly int CHANGE_ERROR_HISTORY = 1010;
        public static readonly int PRERECON_ERROR = 1011;
        public static readonly int RECON_ERROR = 1012;
        public static readonly int RECON_ERROR_HISTORY = 1013;
        public static readonly int INVALID_JSON_RESPONSE = 1014;
        public static readonly int TIMEOUT_ERROR = 1015;
        public static readonly int ACCOUNT_LOCKED = 1016;
        public static readonly int CONFIGURATION_ERROR = 1017;
        public static readonly int NETWORK_UNAVAILABLE = 1018;
        public static readonly int PERMISSION_DENIED = 1019;
        public static readonly int DATA_PARSE_ERROR = 1020;
        public static readonly int OPERATION_CANCELLED = 1021;
        public static readonly int SERVICE_UNAVAILABLE = 1022;
        public static readonly int RATE_LIMIT_EXCEEDED = 1023;
        public static readonly int SESSION_EXPIRED = 1024;
        public static readonly int DEPENDENCY_FAILED = 1025;
        public static readonly int INVALID_PARAMETER = 1026;

        public static readonly ReadOnlyDictionary<int, string> PluginErrorMessages =
            new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()
            {
                [SUCCESS] = "Operation completed successfully",
                [DEFAULT] = "General plugin error, please refer to the logs for more information",
                [UNKNOWN_ERROR] = "An unknown error has occurred",
                [HTTP_REQUEST] = "Error performing HTTP request, please refer to the logs for more information",
                [VERIFY_ERROR] = "Failed to verify credentials, please refer to the logs for more information",
                [INVALID_CREDENTIALS] = "Authentication failed: invalid username, password, or token. Please refer to the logs for more information",
                [INVALID_CREDENTIALS_LOGON] = "Invalid username or password - logon",
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
                [TIMEOUT_ERROR] = "The operation timed out",
                [ACCOUNT_LOCKED] = "The target account is locked",
                [CONFIGURATION_ERROR] = "Plugin configuration is missing or invalid",
                [NETWORK_UNAVAILABLE] = "Network connectivity is unavailable",
                [PERMISSION_DENIED] = "Insufficient permissions for the operation",
                [DATA_PARSE_ERROR] = "Error parsing response or input data",
                [OPERATION_CANCELLED] = "Operation was cancelled by user or system",
                [SERVICE_UNAVAILABLE] = "Target service or API is unavailable",
                [RATE_LIMIT_EXCEEDED] = "API or service rate limit exceeded",
                [SESSION_EXPIRED] = "Session expired during operation",
                [DEPENDENCY_FAILED] = "Required dependency failed (DB or external service)",
                [INVALID_PARAMETER] = "Invalid parameter value passed to plugin"
            });
    }
}
