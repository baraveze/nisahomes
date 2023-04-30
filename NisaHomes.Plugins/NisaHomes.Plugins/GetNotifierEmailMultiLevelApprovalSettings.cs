using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NisaHomes.Plugins
{
    public class GetNotifierEmailMultiLevelApprovalSettings : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);


            try
            {
                Entity Target = (Entity)context.InputParameters["Target"];
                if (Target.Contains("cr566_notifierid"))
                {

                    Queries queryEmail = new Queries(service);
                    Guid notifierId = Target.GetAttributeValue<EntityReference>("cr566_notifierid").Id;
                    string notifieremail = queryEmail.getApprover1Email(notifierId);
                    Target.Attributes.Clear();
                    Target.Attributes.Add("cr566_notifieremail", notifieremail);
                    service.Update(Target);

                }

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException("Error on GetNotifierEmailMultiLevelApprovalSettings: " + e.Message);
            }
        }
    }
}
