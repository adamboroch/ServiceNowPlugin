using Newtonsoft.Json;

namespace ServiceNowPlugin.api
{
    internal class ServiceNowUserModel
    {
        public ServiceNowUserModel GetUser(string newPassword)
        {
            var userJson = JsonConvert.DeserializeObject<ServiceNowUserModel>(JsonConvert.SerializeObject(this));
            userJson.UserPassword = newPassword;
            //clear information that should not be sent during a password change
            userJson.LastLogin = null;
            userJson.LastLoginTime = null;
            userJson.SysCreatedOn = null;
            userJson.SysCreatedBy = null;
            userJson.SysUpdatedOn = null;
            userJson.SysUpdatedBy = null;
            //set default values for certain fields during CPM password change
            userJson.PasswordNeedsReset = "false";

            return userJson;
        }

        [JsonProperty("calendar_integration")]
        public string CalendarIntegration { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("user_password")]
        public string UserPassword { get; set; }

        [JsonProperty("last_login_time")]
        public string LastLoginTime { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("sys_updated_on")]
        public string SysUpdatedOn { get; set; }

        [JsonProperty("building")]
        public string Building { get; set; }

        [JsonProperty("web_service_access_only")]
        public string WebServiceAccessOnly { get; set; }

        [JsonProperty("notification")]
        public string Notification { get; set; }

        [JsonProperty("enable_multifactor_authn")]
        public string EnableMultifactorAuthn { get; set; }

        [JsonProperty("sys_updated_by")]
        public string SysUpdatedBy { get; set; }

        [JsonProperty("sys_created_on")]
        public string SysCreatedOn { get; set; }

        [JsonProperty("sys_domain")]
        public Reference SysDomain { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("vip")]
        public string Vip { get; set; }

        [JsonProperty("sys_created_by")]
        public string SysCreatedBy { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("home_phone")]
        public string HomePhone { get; set; }

        [JsonProperty("time_format")]
        public string TimeFormat { get; set; }

        [JsonProperty("last_login")]
        public string LastLogin { get; set; }

        [JsonProperty("default_perspective")]
        public string DefaultPerspective { get; set; }

        [JsonProperty("active")]
        public string Active { get; set; }

        [JsonProperty("sys_domain_path")]
        public string SysDomainPath { get; set; }

        [JsonProperty("cost_center")]
        public Reference CostCenter { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("employee_number")]
        public string EmployeeNumber { get; set; }

        [JsonProperty("password_needs_reset")]
        public string PasswordNeedsReset { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("failed_attempts")]
        public string FailedAttempts { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("roles")]
        public string Roles { get; set; }

        [JsonProperty("manager_hp1")]
        public string ManagerHp1 { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("sys_class_name")]
        public string SysClassName { get; set; }

        [JsonProperty("sys_id")]    // Primary Key
        public string SysId { get; set; }

        [JsonProperty("federated_id")]
        public string FederatedId { get; set; }

        [JsonProperty("internal_integration_user")]
        public string InternalIntegrationUser { get; set; }

        [JsonProperty("ldap_server")]
        public string LdapServer { get; set; }

        [JsonProperty("mobile_phone")]
        public string MobilePhone { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("company")]
        public Reference Company { get; set; }

        [JsonProperty("department")]
        public Reference Department { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("introduction")]
        public string Introduction { get; set; }

        [JsonProperty("preferred_language")]
        public string PreferredLanguage { get; set; }

        [JsonProperty("manager")]
        public Reference Manager { get; set; }

        [JsonProperty("locked_out")]
        public string LockedOut { get; set; }

        [JsonProperty("sys_mod_count")]
        public string SysModCount { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }

        [JsonProperty("sys_tags")]
        public string SysTags { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("schedule")]
        public string Schedule { get; set; }

        [JsonProperty("date_format")]
        public string DateFormat { get; set; }

        [JsonProperty("location")]
        public Reference Location { get; set; }
    }
    internal class Reference
    {
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}