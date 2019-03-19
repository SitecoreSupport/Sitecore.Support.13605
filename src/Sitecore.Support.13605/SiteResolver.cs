namespace Sitecore.Support.XA.Foundation.Multisite.Pipelines.HttpRequest
{
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines.HttpRequest;
  using Sitecore.Sites;
  using System;
  using System.Web;

  public class SiteResolver : Sitecore.Pipelines.HttpRequest.SiteResolver
  {
    protected new string GetFilePath(HttpRequestArgs args, SiteContext context)
    {
      return this.GetPath(context.PhysicalFolder, args.Url.FilePath, context);
    }

    protected new string GetItemPath(HttpRequestArgs args, SiteContext context)
    {
      return this.GetPath(context.StartPath, args.Url.ItemPath, context);
    }

    protected new string GetPath(string basePath, string path, SiteContext context)
    {
      path = base.GetPath(basePath, path, context);
      if (!path.StartsWith("/", StringComparison.Ordinal))
      {
        return "/" + path;
      }
      return path;
    }

    protected override void UpdatePaths(HttpRequestArgs args, SiteContext site)
    {     
      args.StartPath = site.StartPath;
      args.Url.ItemPath = this.GetItemPath(args, site);
      site.Request.ItemPath = args.Url.ItemPath;
      args.Url.FilePath = this.GetFilePath(args, site);
      site.Request.FilePath = args.Url.FilePath;
    }

    protected override SiteContext ResolveSiteContext(HttpRequestArgs args)
    {
      if (HttpContext.Current != null && (HttpContext.Current.Request.Url.Query ?? "").Contains("404;"))
      {
        Uri url = HttpContext.Current.Request.Url;
        url = new Uri(url, args.Context.Request.RawUrl);
        SiteContext siteContext = base.SiteContextFactory.GetSiteContext(url.Host, url.AbsolutePath, url.Port);
        Assert.IsNotNull(siteContext, "Site from host name and path was not found. Host: " + url.Host + ", path: " + args.Context.Request.RawUrl);
        return siteContext;
      }
      return base.ResolveSiteContext(args);
    }
  }

}