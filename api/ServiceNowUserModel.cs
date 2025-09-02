using Newtonsoft.Json;

namespace CPMPluginTemplate.api
{
    public class ServiceNowUserModel
    {
        // 🔹 Root identifier
        [JsonProperty("sys_id")]
        public string Id { get; set; }

        // 🔹 Grouped properties
        public Meta Meta { get; set; }
        public Personal Personal { get; set; }
        public Contact Contact { get; set; }
        public Organization Organization { get; set; }
        public Security Security { get; set; }
        public Preferences Preferences { get; set; }

        // ===================== //
        // Nested Internal Classes
        // ===================== //

        internal class Meta
        {
            [JsonProperty("sys_created_on")]
            public string CreatedOn { get; set; }

            [JsonProperty("sys_created_by")]
            public string CreatedBy { get; set; }

            [JsonProperty("sys_updated_on")]
            public string UpdatedOn { get; set; }

            [JsonProperty("sys_updated_by")]
            public string UpdatedBy { get; set; }

            [JsonProperty("sys_mod_count")]
            public string ModificationCount { get; set; }

            [JsonProperty("sys_domain_path")]
            public string DomainPath { get; set; }

            [JsonProperty("sys_class_name")]
            public string ClassName { get; set; }
        }

        internal class Personal
        {
            [JsonProperty("name")]
            public string FullName { get; set; }

            [JsonProperty("first_name")]
            public string FirstName { get; set; }

            [JsonProperty("middle_name")]
            public string MiddleName { get; set; }

            [JsonProperty("last_name")]
            public string LastName { get; set; }

            [JsonProperty("gender")]
            public string Gender { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("employee_number")]
            public string EmployeeNumber { get; set; }

            [JsonProperty("active")]
            public string Active { get; set; }

            [JsonProperty("vip")]
            public string Vip { get; set; }
        }

        internal class Contact
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }

            [JsonProperty("mobile_phone")]
            public string MobilePhone { get; set; }

            [JsonProperty("home_phone")]
            public string HomePhone { get; set; }

            [JsonProperty("fax")]
            public string Fax { get; set; }

            [JsonProperty("street")]
            public string Street { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("zip")]
            public string Zip { get; set; }

            [JsonProperty("location")]
            public Reference Location { get; set; }

            [JsonProperty("photo")]
            public string Photo { get; set; }

            [JsonProperty("avatar")]
            public string Avatar { get; set; }
        }

        internal class Organization
        {
            [JsonProperty("company")]
            public Reference Company { get; set; }

            [JsonProperty("department")]
            public Reference Department { get; set; }

            [JsonProperty("cost_center")]
            public Reference CostCenter { get; set; }

            [JsonProperty("manager")]
            public Reference Manager { get; set; }

            [JsonProperty("building")]
            public string Building { get; set; }
        }

        internal class Security
        {
            [JsonProperty("user_name")]
            public string UserName { get; set; }

            [JsonProperty("user_password")]
            public string UserPassword { get; set; }

            [JsonProperty("password_needs_reset")]
            public string PasswordNeedsReset { get; set; }

            [JsonProperty("failed_attempts")]
            public string FailedAttempts { get; set; }

            [JsonProperty("locked_out")]
            public string LockedOut { get; set; }

            [JsonProperty("enable_multifactor_authn")]
            public string EnableMultifactorAuthn { get; set; }

            [JsonProperty("web_service_access_only")]
            public string WebServiceAccessOnly { get; set; }

            [JsonProperty("ldap_server")]
            public string LdapServer { get; set; }

            [JsonProperty("last_login")]
            public string LastLogin { get; set; }

            [JsonProperty("last_login_time")]
            public string LastLoginTime { get; set; }

            [JsonProperty("roles")]
            public string Roles { get; set; }

            [JsonProperty("internal_integration_user")]
            public string InternalIntegrationUser { get; set; }
        }

        internal class Preferences
        {
            [JsonProperty("preferred_language")]
            public string PreferredLanguage { get; set; }

            [JsonProperty("time_zone")]
            public string TimeZone { get; set; }

            [JsonProperty("time_format")]
            public string TimeFormat { get; set; }

            [JsonProperty("date_format")]
            public string DateFormat { get; set; }

            [JsonProperty("notification")]
            public string Notification { get; set; }

            [JsonProperty("schedule")]
            public string Schedule { get; set; }

            [JsonProperty("calendar_integration")]
            public string CalendarIntegration { get; set; }

            [JsonProperty("introduction")]
            public string Introduction { get; set; }
        }

        internal class Reference
        {
            [JsonProperty("link")]
            public string Link { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}