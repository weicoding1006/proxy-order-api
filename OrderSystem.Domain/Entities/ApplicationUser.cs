using Microsoft.AspNetCore.Identity;

namespace OrderSystem.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    /// <summary>名字</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>姓氏</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>帳號建立時間</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>該使用者的所有訂單（一對多）</summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>該使用者的購物車（一對一）</summary>
    public Cart? Cart { get; set; }
}
