using Newtonsoft.Json;

namespace WebApiProject1.Application.System.Services
{
    // 你的原有UserInfoDetail（用户信息）
    public class UserInfoDetail
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("sex")]
        public string Sex { get; set; }
        [JsonProperty("age")]
        public int Age { get; set; }
        [JsonProperty("isActive")]
        public int IsActive { get; set; }
    }

    // 新增：RoleDetail（角色信息，结构可按需调整）
    public class RoleDetail
    {
        [JsonProperty("roleId")]
        public int RoleId { get; set; }
        [JsonProperty("roleName")]
        public string RoleName { get; set; }
        [JsonProperty("roleCode")]
        public string RoleCode { get; set; }
        [JsonProperty("isDefault")]
        public int IsDefault { get; set; }
    }

    // 内层Data结构（扩展：同时包含code + UserInfo + Role）
    public class InnerResultData
    {

        [JsonProperty("UserInfo")] // 用户信息模块
        public UserInfoWrapper UserInfo { get; set; }

        [JsonProperty("Role")] // 新增：角色信息模块
        public RoleWrapper Role { get; set; }
    }

    // UserInfo包装类（原有：内部嵌套data=UserInfoDetail）
    public class UserInfoWrapper
    {
        [JsonProperty("data")]
        public UserInfoDetail Data { get; set; }
    }

    // 新增：Role包装类（内部嵌套data=RoleDetail）
    public class RoleWrapper
    {
        [JsonProperty("data")]
        public RoleDetail Data { get; set; }
    }

}