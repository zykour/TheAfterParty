using System;
using System.Text;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.HtmlHelpers
{
    public static class PagingHelpers
    {
        /// <summary>
        /// A helper method to create pagination form submits using transparent buttons
        /// </summary>
        /// <param name="html"> A HtmlHelper class</param>
        /// <param name="pagingModel"> A view model that contains the pagination information</param>
        /// <param name="pageRange">How many page links should be displayed adjacent to the current page</param>
        /// <returns>A MvcHtmlString of form submits</returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, NavModel pagingModel, int pageRange = 2)
        {
            StringBuilder result = new StringBuilder();
            
            if (pageRange < 1)
            {
                pageRange = 1;
            }

            if (pageRange > 5)
            {
                pageRange = 5;
            }

            // if there is is only one page, don't show pagination
            if (pagingModel.MaxPage() == 1)
            {
                return MvcHtmlString.Create(String.Empty);
            }

            // always include the first page
            if ((pagingModel.CurrentPage - pageRange) > 1)
            {
                result.Append(CreateTag(false, 1));

                if ((pagingModel.CurrentPage - pageRange) > 2)
                {
                    result.Append(CreateEllipsis());
                }
            }

            int min = pagingModel.CurrentPage - pageRange;
            
            if (min < 1)
            {
                min = 1;
            }

            int max = pagingModel.CurrentPage + pageRange;

            if (max > pagingModel.MaxPage())
            {
                max = pagingModel.MaxPage();
            }

            for (int i = min; i <= max; i++)
            {
                result.Append(CreateTag(pagingModel.CurrentPage == i, i));
            }

            // always include the final page 
            if ((pagingModel.CurrentPage + pageRange) < pagingModel.MaxPage())
            {
                if ((pagingModel.CurrentPage + pageRange) < (pagingModel.MaxPage() - 1))
                {
                    result.Append(CreateEllipsis());
                }

                result.Append(CreateTag(false, pagingModel.MaxPage()));
            }
            
            return MvcHtmlString.Create(result.ToString());
        }

        private static string CreateEllipsis()
        {
            TagBuilder tag = new TagBuilder("span");

            tag.AddCssClass("text-lblue col-text noclick page-pad");

            tag.SetInnerText("..");

            return tag.ToString();
        }

        private static string CreateTag(bool isSelected, int pageNum)
        {
            TagBuilder tag = new TagBuilder("input");

            if (isSelected)
            {
                tag = new TagBuilder("span");
                tag.AddCssClass("text-primary noclick col-text selected-page page-pad");
                tag.SetInnerText(pageNum.ToString());

                return tag.ToString();
            }
            else
            {
                //tag = new TagBuilder("input");
                tag.MergeAttribute("name", "SelectedPage");
                tag.MergeAttribute("value", pageNum.ToString());
                tag.MergeAttribute("type", "submit");
                tag.AddCssClass("btn btn-transparent col-text text-lblue page-pad");

                return tag.ToString(TagRenderMode.SelfClosing);
            }
        }
    }
}