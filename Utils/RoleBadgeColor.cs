using Radzen;

namespace ArchiBase.Utils;

public static class RoleBadgeColorExtensions
{
    public static BadgeStyle ToBadgeStyle(this string roleName) => roleName switch
    {
        "Admin" => BadgeStyle.Danger,
        "Editor" => BadgeStyle.Warning,
        "Local Editor" => BadgeStyle.Info,
        "Photo Moderator" => BadgeStyle.Success,
        _ => BadgeStyle.Light,
    };

}