using NServiceBus.Pipeline;
using NServiceBus.Sagas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lead.IntegrationTests
{
    public static class InvokeHandlerContextExtension
    {
        public static ActiveSagaInstance GetSagaInstance(this IInvokeHandlerContext context)
        {
            return context.Extensions.TryGet(out ActiveSagaInstance saga) ? saga : null;
        }
    }
}
