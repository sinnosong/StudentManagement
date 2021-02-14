using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace StudentManagement.Extensions
{
    /// <summary>
    /// 获取枚举的显示名字
    /// </summary>
    public static class EnumExtension
    {
        public static string GetDisplayName(this System.Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memberInfos = type.GetMember(en.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                object[] attrs = memberInfos[0].GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DisplayAttribute)attrs[0]).Name;
                }
            }
            return en.ToString();
        }
    }
}
