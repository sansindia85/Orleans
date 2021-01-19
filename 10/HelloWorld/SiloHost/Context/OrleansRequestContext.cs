using Orleans.Runtime;
using System;

namespace SiloHost.Context
{
    public class OrleansRequestContext : IOrleansRequestContext
    {
        public Guid TraceId => RequestContext.Get("traceId") == null ? Guid.Empty : (Guid) RequestContext.Get("traceId");
       
    }
}
