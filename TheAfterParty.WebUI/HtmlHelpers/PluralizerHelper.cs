using System;
using System.Text;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.HtmlHelpers
{
    public static class PluralizerHelper
    {
        /// <summary>
        /// A helper method to determine and return the correct pluralization/singularization of a unit type (i.e. 2 inch => 2 inches)
        /// </summary>
        /// <param name="html"> A HtmlHelper class</param>
        /// <param name="unitAmount"> The amount of the unit, to determine if it should be pluralized </param>
        /// <param name="unitTypeSingular"> The unit type</param>
        /// <param name="endsInY"> Determines if the unit type requires a plural suffix of "ies" (e.g. spies) </param>
        /// <param name="hasEsPluralSuffix"> Determines if the unit type requires a plural suffix of "es" (e.g. inches) </param>
        /// <returns>A MvcHtmlString with a properly pluralized or singularized string with a lower case suffix</returns>
        public static MvcHtmlString FixPluralization(this HtmlHelper html, int unitAmount, string unitTypeSingular, bool endsInY = false, bool hasEsPluralSuffix = false)
        {
            String result = unitTypeSingular.Trim().ToString();

            if (unitAmount != 1 && unitAmount != -1)
            {
                if (endsInY == true)
                {
                    result = result.Substring(0, result.Length - 1);
                    result += "ies";
                }
                else if (hasEsPluralSuffix == true)
                {
                    result += "es";
                }
                else
                {
                    result += "s";
                }
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}