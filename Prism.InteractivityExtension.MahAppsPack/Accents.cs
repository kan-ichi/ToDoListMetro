using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.InteractivityExtension.MahAppsPack
{
    public enum Accents
    {
        Amber,
        Blue,
        Brown,
        Cobalt,
        Crimson,
        Cyan,
        Emerald,
        Green,
        Indigo,
        Lime,
        Magenta,
        Mauve,
        Olive,
        Orange,
        Pink,
        Purple,
        Red,
        Sienna,
        Steel,
        Taupe,
        Teal,
        Violet,
        Yellow,
    }

    public static class AccentsExtensions
    {
        public static string ToStringFromEnum(this Accents accents)
        {
            switch (accents)
            {
                case Accents.Amber: return "Amber";
                case Accents.Blue: return "Blue";
                case Accents.Brown: return "Brown";
                case Accents.Cobalt: return "Cobalt";
                case Accents.Crimson: return "Crimson";
                case Accents.Cyan: return "Cyan";
                case Accents.Emerald: return "Emerald";
                case Accents.Green: return "Green";
                case Accents.Indigo: return "Indigo";
                case Accents.Lime: return "Lime";
                case Accents.Magenta: return "Magenta";
                case Accents.Mauve: return "Mauve";
                case Accents.Olive: return "Olive";
                case Accents.Orange: return "Orange";
                case Accents.Pink: return "Pink";
                case Accents.Purple: return "Purple";
                case Accents.Red: return "Red";
                case Accents.Sienna: return "Sienna";
                case Accents.Steel: return "Steel";
                case Accents.Taupe: return "Taupe";
                case Accents.Teal: return "Teal";
                case Accents.Violet: return "Violet";
                case Accents.Yellow: return "Yellow";
            }
            return "";
        }
    }
}
