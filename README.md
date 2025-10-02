# ServiceNowPlugin
## ServiceNow CPM Plugin

This is a CyberArk CPM API-based plugin for managing ServiceNow accounts passwords using the following operations.

---

## Supported Operations

- **Verify** – Validates credentials of the target account  
- **Change** – Changes the password of the target account  
- **Reconcile** – Uses the reconcile account to reset the target account password

---
## Permissions Note

- **Verify / Change:** Any user can perform these operations on their **own account** via the API.
- **Reconcile:** Requires a **Reconcile account** with sufficient privileges (typically an admin account) to reset other users’ passwords.
  - Using a non-admin or insufficiently privileged account for Reconcile will cause the plugin to fail with:

    ```
    Execution error. Authentication failed: invalid reconcile username, password, token OR insufficient permissions. Check logs for details.
    ```
    **Error code:** 1004
    
---
## Prerequisites

- **.NET Framework:** 4.8 (`net48`)  
  - [Download .NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
- **.NET SDK:** 14.6.0.14  
  - [CyberArk .NET SDK on Marketplace](https://community.cyberark.com/marketplace/s/#a3550000000EkA0AAK-a3950000000jjoOAAQ)  
  - Package includes e.g.: `CANetPluginInvoker.exe` – used in the API plugin
- **CyberArk Password Manager (CPM):** Version 14.6.1 (14.06.01.26)  - currently (02.10.2025) the latest versin on which CPM plugin has been tested and confirmed to be a working solution
- **Visual Studio:** Any edition that supports .NET Framework 4.8  
- **Required DLLs:**
  - `CyberArk.Extensions.Plugins.Models.dll` v20.0.7.28  (included in the project)  *In case you are using different CPM version it is reccomended to replace those files with the files from your CPM.
  - `CyberArk.Extensions.Utilties.dll` v20.0.0.3         (included in the project)  *In case you are using different CPM version it is reccomended to replace those files with the files from your CPM.
  - Both DLLs are part of the CPM installation and can be found in:  
    `C:\Program Files (x86)\CyberArk\Password Manager\bin`
  - `Newtonsoft.Json.dll` (Version 13.0.4)  
    - Json.NET has different libraries for various .NET Framework versions:  
      - `net45` → for .NET 4.5+  
    - Currently used / copied to CPM: 13.04.30916  
    - [Release info](https://github.com/JamesNK/Newtonsoft.Json/releases)  
    - [Download Newtonsoft.Json](https://www.newtonsoft.com/json)

> **Note:** `Newtonsoft.Json.dll` can be installed via NuGet:  
> ```powershell
> Install-Package Newtonsoft.Json
> ```  
> Or manually download and create a folder structure in your project like:  
> `packages\Newtonsoft.Json.13.0.4\lib\net45\Newtonsoft.Json.dll`  
> and add a reference to it.

---

### CyberArk & ServiceNow Integration (Zurich, Version 38)

Developed a CyberArk CPM plugin that connects directly to ServiceNow via REST API, enabling automated password verification, password changes, and reconciliation of accounts stored in ServiceNow.  
This plugin extends CyberArk’s automatic password management to ServiceNow users in a seamless and API-driven way.

General API Documentation:  
https://www.servicenow.com/docs/bundle/zurich-api-reference/page/integrate/inbound-rest/concept/c_RESTAPI.html

APIs used for plugin development (POST/GET):  
https://www.servicenow.com/docs/bundle/zurich-api-reference/page/integrate/inbound-rest/concept/c_TableAPI.html

Worked with Zurich version 38, ensuring compatibility with latest platform features.

For testing and exploration of REST endpoints, used REST API Explorer on the ServiceNow instance:  
All -> System Web Services -> REST -> REST API Explorer

---

# Deploying plugin to CyberArk CPM

## 1. Deploy DLL to CPM Server

- Copy the plugin DLL to the CPM server:  
C:\Program Files (x86)\CyberArk\Password Manager\bin<place dll files here>

   **Example directory** (dedicated plugin path created by you in \bin folder):  *\Plugins\ServiceNowUsers
   
- ⚠️ **Important – Unlock the DLL before use**  
When DLLs are downloaded from external sources, Windows marks them as "blocked" for security reasons.  
- Right-click the downloaded `ServiceNowPlugin.dll`  
- Select **Properties**  
- At the bottom of the **General** tab, check for **Security → "This file came from another computer and might be blocked"**  
- If the **Unblock** checkbox is visible, tick it and click **Apply → OK**  
- Only then copy the DLL into the plugin folder.  
- If you skip this step, the plugin may fail to load or execute in CPM.

- Ensure that ALL dependencies are in place:  
- `CyberArk.Extensions.*` DLLs → must remain in the CPM **bin** folder
- `Newtonsoft.Json.dll` → must be accessible to the plugin (placing it alongside the plugin DLL is fine)  

---

## 2. Configure Plug-in in PVWA

- Define or duplicate the platform that will use this plugin  
- **Recommended:** Duplicate an **Application-type platform** → all values are prefilled, only replace the DLL reference  
- Go to **Target Account Platform > Automatic Password Management > CPM Plug-in**  
- Point to the compiled DLL  

| Property Name | Value |
|---------------|-------|
| **DllName**   | `Plugins\ServiceNowUsers\ServiceNowPlugin.dll` |
| **ExeName**   | `CANetPluginInvoker.exe` |
| **XMLFile**   | `Yes` |

⚠️ Other properties (ScriptName, ScriptEngine, ActivationMethod, ProtocolValidationEx, etc.) can remain as default.  


---

## 3. Save & Validate
1. Save the platform configuration.  
2. Confirm the plugin DLL exists in the correct path: \Plugins\ServiceNowUsers\ServiceNowPlugin.dll 
3. Restart the **CPM service**.
4. Point onboarded ServiceNow acocunts to the platform including reconcile account (if in place). 
5. Test by running a password change on a target account → verify in logs that the `ServiceNowPlugin.dll` is invoked successfully.  

---

## Troubleshooting
- If the plugin fails to load:  
- Verify the DLL was **unblocked** before deployment.  
- Check the DLL path is correct in platform settings.  
- Ensure all dependencies (`CyberArk.Extensions.*`, `Newtonsoft.Json.dll`) are present.  
- Restart CPM after any change to reload the configuration.
- Enable Debug mode on the platform level 

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
- After building the project, all required DLL files can be found in the following folder Relese or Debug : CyberArk and Newtonsoft.Json dlls
- **Output DLL path:**
  C:\Users\ **YourUserName** \source\repos\ServiceNowPlugin\bin\ **Relese** \ServiceNowPlugin.dll ( Relese | Debug - depends on what option did you choose during Project compile)
-  **Copy DLL to CPM plugin folder:** C:\Program Files (x86)\CyberArk\Password Manager\bin<your plugin folder> 
-   **Example directory** (created by you):  *\Plugins\ServiceNowUsers  (including Newtonsoft.Json.dll)
-   ## ServiceNow API Plug-in from CyberArk Marketplace
CyberArk provides a **certified ServiceNow API plug-in**, separate from DLL deployment.  

- Marketplace link: [ServiceNow via API – CyberArk Marketplace](https://community.cyberark.com/marketplace/s/#a35Ht000000riy0IAA-a39Ht000001kKJ4IAM)  
- Developed by CyberArk, certified, 469+ downloads.  
- Uses **REST API logic chains**, different from DLL method.  
- Supports **Password Change** and **Password Reconciliation**. 
---

## Quick Project Setup in Visual Studio - GUIDE to build other plugin from Scratch (optional - for developers)

1. **Create a new project**:  
   - Type: **Class Library (.NET Framework)**  
   - Framework: **.NET Framework 4.8**  
   - Name: e.g., `CPMPluginServiceNow`

2. **Add References**:  
   - Place the dll files according to your project struture 
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
   - ( etc.`Prereconcile.cs`, `Logon.cs`, `BaseAction.cs` depending on logic)

> **Examples**: Full sample implementation files (`CyberArk.Extensions.Plugin.Template.zip`) can be found here on CyberArk Marketplace:  
> [CyberArk .NET SDK – Plugin Template](https://community.cyberark.com/marketplace/s/#a3550000000EkA0AAK-a3950000000jjoOAAQ)

 <img width="468" height="333" alt="image" src="https://github.com/user-attachments/assets/1b14a412-dc0c-4435-b716-efbddb5731fe" />


