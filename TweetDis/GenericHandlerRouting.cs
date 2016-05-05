using System;
using System.Web;
using System.Web.Routing;
// Source: http://stackoverflow.com/a/3380249/3816975

/// <summary>
/// For info on subclassing RouteBase, check Pro Asp.NET MVC Framework, page 252.
/// Google books link: http://books.google.com/books?id=tD3FfFcnJxYC&pg=PA251&lpg=PA251&dq=.net+RouteBase&source=bl&ots=IQhFwmGOVw&sig=0TgcFFgWyFRVpXgfGY1dIUc0VX4&hl=en&ei=z61UTMKwF4aWsgPHs7XbAg&sa=X&oi=book_result&ct=result&resnum=6&ved=0CC4Q6AEwBQ#v=onepage&q=.net%20RouteBase&f=false
/// 
/// It explains how the asp.net runtime will call GetRouteData() for every route in the route table.
/// GetRouteData() is used for inbound url matching, and should return null for a negative match (the current requests url doesn't match the route).
/// If it does match, it returns a RouteData object describing the handler that should be used for that request, along with any data values (stored in RouteData.Values) that
/// that handler might be interested in.
/// 
/// The book also explains that GetVirtualPath() (used for outbound url generation) is called for each route in the route table, but that is not my experience,
/// as mine used to simply throw a NotImplementedException, and that never caused a problem for me.  In my case, I don't need to do outbound url generation,
/// so I don't have to worry about it in any case.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericHandlerRoute<T> : RouteBase where T : IHttpHandler, new()
{
	public string RouteUrl { get; set; }


	public GenericHandlerRoute(string routeUrl)
	{
		RouteUrl = routeUrl;
	}


	public override RouteData GetRouteData(HttpContextBase httpContext)
	{
		// See if the current request matches this route's url
		string baseUrl = httpContext.Request.CurrentExecutionFilePath;
		int ix = baseUrl.IndexOf(RouteUrl);
		if (ix == -1)
			// Doesn't match this route.  Returning null indicates to the asp.net runtime that this route doesn't apply for the current request.
			return null;

		baseUrl = baseUrl.Substring(0, ix + RouteUrl.Length);

		// This is kind of a hack.  There's no way to access the route data (or even the route url) from an IHttpHandler (which has a very basic interface).
		// We need to store the "base" url somewhere, including parts of the route url that are constant, like maybe the name of a method, etc.
		// For instance, if the route url "myService/myMethod/{myArg}", and the request url were "http://localhost/myApp/myService/myMethod/argValue",
		// the "current execution path" would include the "myServer/myMethod" as part of the url, which is incorrect (and it will prevent your UriTemplates from matching).
		// Since at this point in the exectuion, we know the route url, we can calculate the true base url (excluding all parts of the route url).
		// This means that any IHttpHandlers that use this routing mechanism will have to look for the "__baseUrl" item in the HttpContext.Current.Items bag.
		// TODO: Another way to solve this would be to create a subclass of IHttpHandler that has a BaseUrl property that can be set, and only let this route handler
		// work with instances of the subclass.  Perhaps I can just have RestHttpHandler have that property.  My reticence is that it would be nice to have a generic
		// route handler that works for any "plain ol" IHttpHandler (even though in this case, you have to use the "global" base url that's stored in HttpContext.Current.Items...)
		// Oh well.  At least this works for now.
		httpContext.Items["__baseUrl"] = baseUrl;

		GenericHandlerRouteHandler<T> routeHandler = new GenericHandlerRouteHandler<T>();
		RouteData rdata = new RouteData(this, routeHandler);

		return rdata;
	}


	public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
	{
		// This route entry doesn't generate outbound Urls.
		return null;
	}
}



public class GenericHandlerRouteHandler<T> : IRouteHandler where T : IHttpHandler, new()
{
	public IHttpHandler GetHttpHandler(RequestContext requestContext)
	{
		return new T();
	}
}