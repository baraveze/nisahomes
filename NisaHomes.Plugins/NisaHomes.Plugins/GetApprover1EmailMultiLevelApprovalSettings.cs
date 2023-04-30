namespace NisaHomes.Plugins
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class GetApprover1EmailMultiLevelApprovalSettings : IPlugin
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
                if (Target.Contains("cr566_approver1id")) {

                    Queries queryEmail = new Queries(service);
                    Guid approver1Id = Target.GetAttributeValue<EntityReference>("cr566_approver1id").Id;
                    string approver1email = queryEmail.getApprover1Email(approver1Id);
                    Target.Attributes.Clear();
                    Target.Attributes.Add("cr566_approver1email", approver1email);
                    service.Update(Target);
                    
                }
                
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException("Error on GetApprover1EmailMultiLevelApprovalSettings: " + e.Message);
            }
        }
    }
}
