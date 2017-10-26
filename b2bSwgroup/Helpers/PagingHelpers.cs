using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using b2bSwgroup.Models;

namespace b2bSwgroup.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            if (pageInfo.TotalPages > 7 && pageInfo.PageNumber>4)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(1));
                tag.InnerHtml = "1..";
                tag.AddCssClass("btn btn-default");
                result.Append(tag.ToString());
            }
            for (int i = 1; i <= pageInfo.TotalPages; i++)
            {
                
                if(i - pageInfo.PageNumber < 4 && i - pageInfo.PageNumber > -4)
                {
                    TagBuilder tag = new TagBuilder("a");
                    tag.MergeAttribute("href", pageUrl(i));
                    tag.InnerHtml = i.ToString();
                    // если текущая страница, то выделяем ее,
                    // например, добавляя класс
                    if (i == pageInfo.PageNumber)
                    {
                        tag.AddCssClass("selected");
                        tag.AddCssClass("btn-primary");
                    }
                    tag.AddCssClass("btn btn-default");

                    result.Append(tag.ToString());
                }
            }
            if (pageInfo.TotalPages > 7 && (pageInfo.TotalPages - pageInfo.PageNumber) > 3)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(pageInfo.TotalPages));
                tag.InnerHtml = ".."+pageInfo.TotalPages.ToString();
                tag.AddCssClass("btn btn-default");
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}