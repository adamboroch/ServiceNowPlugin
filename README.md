# ServiceNowPlugin
## ServiceNow CPM Plugin

This is a CyberArk CPM API-based plugin for managing ServiceNow account passwords using **Change / Reconcile / Verify** operations.

---

## Prerequisites

- **.NET Framework:** 4.8 (`net48`)  
- **Visual Studio:** Any edition that supports .NET Framework 4.8  
- **Required DLLs:**
  - `CyberArk.Extensions.Plugins.Models.dll`
  - `CyberArk.Extensions.Utilties.dll`
  - `Newtonsoft.Json.dll` (version 13.0.3)

> **Note:** `Newtonsoft.Json.dll` can be installed via NuGet:  
> ```powershell
> Install-Package Newtonsoft.Json -Version 13.0.3
> ```  
> Or manually download and: create a folder structure in your project like  
> `packages\Newtonsoft.Json.13.0.3\lib\net45\Newtonsoft.Json.dll`  
> and add a reference to it.

---

# Deploying plugin to CyberArk CPM

## 1. Deploy DLL to CPM Server

- Copy the plugin DLL to the CPM server:  
C:\Program Files (x86)\CyberArk\Password Manager\bin<place dll files here>

   **Example directory** (created by you):  *\Plugins\ServiceNowUsers
   
- ⚠️ **Important – Unlock the DLL before use**  
When DLLs are downloaded from external sources, Windows marks them as "blocked" for security reasons.  
- Right-click the downloaded `ServiceNowPlugin.dll`  
- Select **Properties**  
- At the bottom of the **General** tab, check for **Security → "This file came from another computer and might be blocked"**  
- If the **Unblock** checkbox is visible, tick it and click **Apply → OK**  
- Only then copy the DLL into the plugin folder.  
- If you skip this step, the plugin may fail to load or execute in CPM.

- Ensure dependencies are in place:  
- `CyberArk.Extensions.*` DLLs → must remain in the CPM **bin** folder or GAC  
- `Newtonsoft.Json.dll` → must be accessible to the plugin (placing it alongside the plugin DLL is fine)  

---

## 2. Configure Plug-in in PVWA
- Go to **PVWA → CPM → Platform Management → Add/Update Plugin**  
- Upload the compiled DLL  
- Define or duplicate the platform that will use this plugin  
- **Recommended:** Duplicate an **Application-type platform** → all values are prefilled, only replace the DLL reference  

Set or verify these key properties under **General Properties**:

| Property Name | Value |
|---------------|-------|
| **DllName**   | `Plugins\ServiceNowUsers\ServiceNowPlugin.dll` |
| **XMLFile**   | `Yes` |
| **ActivationMethod** | `Basic` |
| *(others)*    | Leave blank unless explicitly required (ScriptName, ScriptEngine, ProtocolValidationEx, etc.) |

⚠️ Note: `DllName` is not relevant for group-type policies.  

---

## 3. Save, Deploy & Validate
1. Save the platform configuration.  
2. Confirm the plugin DLL exists in the correct path: \Plugins\ServiceNowUsers\ServiceNowPlugin.dll
3. Ensure the **CPM service account** has read/execute rights on the DLL.  
4. Restart the **CPM service**.  
5. Test by running a password change on a target account → verify in logs that the `ServiceNowPlugin.dll` is invoked successfully.  

---

## Troubleshooting
- If the plugin fails to load:  
- Verify the DLL was **unblocked** before deployment.  
- Check the DLL path is correct in platform settings.  
- Ensure all dependencies (`CyberArk.Extensions.*`, `Newtonsoft.Json.dll`) are present.  
- Restart CPM after any change to reload the configuration.  

---
---

## Supported Operations

- **Verify** – Validates credentials of the target account  
- **Change** – Changes the password of the target account  
- **Reconcile** – Uses the reconcile account to reset the target account password

---

## Handled Error Codes

All errors are handled using the `PluginErrors` class. Each error sets `PlatformOutput.Message` in PVWA with descriptive text:

| Code  | Meaning |
|-------|---------|
| 0     | Operation completed successfully |
| 9999  | General plugin error, please refer to the logs for more information |
| 1000  | An unknown error has occurred |
| 1002  | Failed to verify credentials, please refer to the logs for more information |
| 1003  | Authentication failed: Invalid username, password, token OR Insufficient permissions OR Password needs reset OR Locked out. |
| 1004  | Authentication failed: invalid reconcile username, password, token OR insufficient permissions. Check logs for details. |
| 1005  | GET API returned no users or invalid response. Check logs for details. |
| 1006  | Failed to verify credentials before change action, please refer to the logs for more information |
| 1007  | Failed to change the password, please refer to the logs for more information |
| 1008  | Failed to verify reconcile account credentials, please refer to the logs for more information |
| 1009  | Failed to reconcile the password, please refer to the logs for more information |
| 1010  | Unexpected JSON format in response. Check logs for details. |
| 1011  | Error parsing JSON response. Check logs for details. |
| 1012  | Network connectivity is unavailable. Check logs for details. |
| 1013  | Invalid parameter value passed to plugin. Check logs for details. |

---

## Notes

- Plugin uses `System.Net.Http` for API calls (built-in in .NET Framework)  
- Plugin uses `Newtonsoft.Json` for JSON parsing
- **Compilation:** You must compile the project in Visual Studio to produce `ServiceNowPlugin.dll`.
- **Output DLL path:**
  C:\Users\ **YourUserName** \source\repos\ServiceNowPlugin\bin\ **Relese** \ServiceNowPlugin.dll ( Relese | Debug - depends on what option did you choose during Project compile)
-  **Copy DLL to CPM plugin folder:** C:\Program Files (x86)\CyberArk\Password Manager\bin<your plugin folder> 
-   **Example directory** (created by you):  *\Plugins\ServiceNowUsers  (including Newtonsoft.Json.dll)
---

## Quick Project Setup in Visual Studio (optional - for developers)

1. **Create a new project**:  
   - Type: **Class Library (.NET Framework)**  
   - Framework: **.NET Framework 4.8**  
   - Name: e.g., `CPMPluginServiceNow`

2. **Add References**:  
   - Right-click **References → Add Reference**  
   - Add:
     - `CyberArk.Extensions.Plugins.Models.dll`
     - `CyberArk.Extensions.Utilties.dll`
     - `Newtonsoft.Json.dll` (via NuGet or manually)

3. **Add Source Files like**:
   - `Change.cs`
   - `Reconcile.cs`
   - `Verify.cs`
   - `PluginErrors.cs`


