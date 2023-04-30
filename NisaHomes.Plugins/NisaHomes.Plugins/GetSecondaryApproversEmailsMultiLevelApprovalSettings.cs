namespace NisaHomes.Plugins
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetSecondaryApproversEmailsMultiLevelApprovalSettings :IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            Entity Target = (Entity)context.InputParameters["Target"];

            try
            {

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException("Mensaje de error" + e.Message);
            }
        }
    }
}
