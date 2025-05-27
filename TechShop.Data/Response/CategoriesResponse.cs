// "-----------------------------------------------------------------------
//  <copyright file="CategoriesResponse.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace TechShop.Data.Response
{
    public class CategoriesResponse
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Href { get; set; }
        public string? Type { get; set; }
        public List<Article>? Articles { get; set; }
    }
    public class Article
    {
        public Guid Id { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ContentHtml { get; set; }
        public string? Href { get; set; }
    }
}


