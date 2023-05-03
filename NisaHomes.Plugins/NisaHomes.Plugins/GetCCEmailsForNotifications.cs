using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NisaHomes.Plugins
{
    public class GetCCEmailsForNotifications: IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);


            try
            {

                EntityReference targetEntity = null;
                EntityReference relatedEntity = null;
                Entity FinancialRequest = null;
                Entity Users = null;
                string relationshipname = "";

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {



                    targetEntity = (EntityReference)context.InputParameters["Target"];

                }
                if (context.InputParameters.Contains("Relationship"))
                {

                    relationshipname = ((Relationship)context.InputParameters["Relationship"]).SchemaName;

                }

                if (relationshipname != "cr566_cr566_financialassistancerequest_systemus")
                {
                    return;
                }
                if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                {

                    EntityReferenceCollection relatedEntitieCollection = (EntityReferenceCollection)context.InputParameters["RelatedEntities"];
                    relatedEntity = relatedEntitieCollection[0];
                }

                FinancialRequest = service.Retrieve(targetEntity.LogicalName, targetEntity.Id, new ColumnSet("cr566_ccnotificationemails"));
                Users = service.Retrieve(relatedEntity.LogicalName, relatedEntity.Id, new ColumnSet(false));

                //Retrieve contacts emails related to FinancialRequest
                QueryExpression query = new QueryExpression("cr566_cr566_financialassistancerequest_system");
                query.Criteria.AddCondition("cr566_financialassistancerequestid", ConditionOperator.Equal, FinancialRequest.Id);
                query.ColumnSet = new ColumnSet("systemuserid");
                EntityCollection results = service.RetrieveMultiple(query);
                string CCForNotificationsEmails = "";
                foreach (Entity result in results.Entities)
                {
                    //Take contactid and search its email 
                    Guid userguid = result.GetAttributeValue<Guid>("systemuserid");
                    Entity user = service.Retrieve("systemuser", userguid, new ColumnSet("internalemailaddress"));

                    CCForNotificationsEmails += user.GetAttributeValue<String>("internalemailaddress") + ";";

                }


                if (context.MessageName.ToLower() == "associate" || context.MessageName.ToLower() == "disassociate")
                {

                    FinancialRequest.Attributes["cr566_ccnotificationemails"] = CCForNotificationsEmails;
                    service.Update(FinancialRequest);

                }



            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException("Error Details : " + e.Message);
            }
        }

    }
}
