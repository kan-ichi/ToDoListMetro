using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.InteractivityExtension.MahAppsPack
{
    public enum Themes
    {
        BaseDark,
        BaseLight,
    }

    public static class ThemesExtensions
    {
        public static string ToStringFromEnum(this Themes themes)
        {
            switch (themes)
            {
                case Themes.BaseDark: return "BaseDark";
                case Themes.BaseLight: return "BaseLight";
            }
            return "";
        }
    }

}
