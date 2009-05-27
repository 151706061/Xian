using System;
using Castle.DynamicProxy;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common.Caching;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Client-side advice to implement transparent caching on the client side,
	/// where the server returns an appropriate <see cref="ResponseCachingDirective"/>.
	/// </summary>
    public class ResponseCachingClientAdvice : IInterceptor
    {
        #region IInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {
            object service = invocation.InvocationTarget;
            string region = string.Format("{0}.{1}",
                invocation.Method.DeclaringType.FullName, invocation.Method.Name);
            object request = args[0];
            object response;

			// check for a cached response
			// must do this even if this operation does not support caching, because have no way of knowing
			// whether it does or does not support caching
            string cacheKey = GetCacheKey(request);
            ICacheClient cacheClient = Cache.CreateClient("ResponseCache");
            if (cacheKey != null && cacheClient.RegionExists(region))
            {
                // check cache, and return if available
                response = cacheClient.Get(cacheKey, new CacheGetOptions(region));
                if (response != null)
                    return response;
            }

            // no cached response is available, so invoke service operation
            using (new OperationContextScope(service as IContextChannel))
            {
                // invoke the operation
                response = invocation.Proceed(args);

                // read caching directive from headers
                ResponseCachingDirective directive = ReadCachingDirectiveHeader();

                // cache the response if directed
                if (directive != null && directive.EnableCaching)
                {
                    // if we didn't succeed in getting a cache key, this is an error
                    if (cacheKey == null)
                        throw new InvalidOperationException(
                            string.Format("{0} is cacheable but the request class does not implement IDefinesCacheKey.", response.GetType().FullName));

					// cache the response for future use
                    cacheClient.Put(cacheKey, response, new CachePutOptions(region, directive.TimeToLive, false));
                }
            }

            return response;

        }

		/// <summary>
		/// Obtains the cache key for the specified request object.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
        private static string GetCacheKey(object request)
        {
            if (request is IDefinesCacheKey)
            {
                return (request as IDefinesCacheKey).GetCacheKey();
            }
            // for now, the request must implement an interface to get a cache key
            // in future, we could add some automatic serialization to turn it into a cache key
            return null;
        }

		/// <summary>
		/// Attempts to read the cache directive header from the message, returning null if the header doesn't exist.
		/// </summary>
		/// <returns></returns>
        private static ResponseCachingDirective ReadCachingDirectiveHeader()
        {
            int h = OperationContext.Current.IncomingMessageHeaders.FindHeader(
                        ResponseCachingDirective.HeaderName,
                        ResponseCachingDirective.HeaderNamespace);
            if (h > -1)
            {
                return OperationContext.Current.IncomingMessageHeaders
                     .GetHeader<ResponseCachingDirective>(h);
            }

            return null;
        }

        #endregion
    }
}
