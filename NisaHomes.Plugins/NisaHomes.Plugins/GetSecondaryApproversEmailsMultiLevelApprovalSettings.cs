namespace NisaHomes.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
       
            
            try
            {

                EntityReference targetEntity = null;
                EntityReference relatedEntity = null;
                Entity MultiLevelSettings = null;
                Entity Users = null;
                string relationshipname = "";

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference) {

      

                    targetEntity = (EntityReference) context.InputParameters["Target"];
                
                }
                if (context.InputParameters.Contains("Relationship")) {

                    relationshipname = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                   
                }

                if (relationshipname != "cr566_cr566_multilevelapprovalsettings_systemus") {
                    return;
                }
                if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection) {

                    EntityReferenceCollection relatedEntitieCollection = (EntityReferenceCollection) context.InputParameters["RelatedEntities"];
                    relatedEntity = relatedEntitieCollection[0];
                }

                MultiLevelSettings = service.Retrieve(targetEntity.LogicalName, targetEntity.Id, new ColumnSet("cr566_secondaryapproversemails"));
                Users = service.Retrieve(relatedEntity.LogicalName, relatedEntity.Id, new ColumnSet(false));

                //Retrieve contacts emails related to Multilevelsettings
                QueryExpression query = new QueryExpression("cr566_cr566_multilevelapprovalsettings_system");
                query.Criteria.AddCondition("cr566_multilevelapprovalsettingsid", ConditionOperator.Equal, MultiLevelSettings.Id);
                query.ColumnSet = new ColumnSet("systemuserid");
                EntityCollection results = service.RetrieveMultiple(query);
                string secondaryApproversEmails = "";
                foreach (Entity result in results.Entities)
                {
                   //Take contactid and search its email 
                    Guid userguid = result.GetAttributeValue<Guid>("systemuserid");
                    Entity user = service.Retrieve("systemuser",userguid , new ColumnSet("internalemailaddress"));
                   
                    secondaryApproversEmails += user.GetAttributeValue<String>("internalemailaddress") + ";";
                    
                }


                if (context.MessageName.ToLower() == "associate" || context.MessageName.ToLower() == "disassociate") {
                    
                    MultiLevelSettings.Attributes["cr566_secondaryapproversemails"] = secondaryApproversEmails;
                    service.Update(MultiLevelSettings);

                }

              

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException("Error Details : " + e.Message);
            }
        }
    }
}
