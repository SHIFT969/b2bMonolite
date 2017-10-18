using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using b2bSwgroup.Models;

namespace b2bSwgroup.App_Code
{
    public  static class Helpers
    {
        public static MvcHtmlString SetPrice(double price, Currency currency)
        {
            TagBuilder tag = new TagBuilder("span");
            tag.AddCssClass("currency");
            if (currency != null && currency.СultureInfo != "" && currency.СultureInfo != null)
            {
                IFormatProvider formatProvider = new System.Globalization.CultureInfo(currency.СultureInfo);
                tag.SetInnerText(price.ToString("C", formatProvider));
            }
            else
            {
                tag.SetInnerText(String.Format("{0:##,###.00}", price));
            }                  

            return new MvcHtmlString(tag.ToString());
        }
    }
}