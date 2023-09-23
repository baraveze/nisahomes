namespace NisaHomes.Action
{
    using Microsoft.Xrm.Sdk.Workflow;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Activities;
    using System.Linq;

    public class ApplicationTaskTemplateConcatenateLanguages : CodeActivity
    {
        [RequiredArgument]
        [Input("Generic Subject Schema Name")]
        public InArgument<string> subjectSchemaName { get; set; }

        [RequiredArgument]
        [Input("Generic Description Schema Name")]
        public InArgument<string> descriptionSchemaName { get; set; }

        [RequiredArgument]
        [Input("Entity Schema Name (prefix_entity)")]
        public InArgument<string> tableName { get; set; }

        [RequiredArgument]
        [Input("English Subject Text (map content)")]
        public InArgument<string> englishSubjectText { get; set; }

        [RequiredArgument]
        [Input("French Subject Text (map content)")]
        public InArgument<string> frenchSubjectText { get; set; }

        [RequiredArgument]
        [Input("English Description Text (map content)")]
        public InArgument<string> englishDescriptionText { get; set; }

        [RequiredArgument]
        [Input("French Description Text (map content)")]
        public InArgument<string> frenchDescriptionText { get; set; }

        [RequiredArgument]
        [Input("French language delimiter tag")]
        public InArgument<string> frenchDelimiterTag { get; set; }

        [RequiredArgument]
        [Input("English language delimiter tag")]
        public InArgument<string> englishDelimiterTag { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracerService = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {

                Entity targetEntity = new Entity(tableName.Get(executionContext));
                targetEntity.Id = context.PrimaryEntityId;

                
                string englishSubject = englishSubjectText.Get(executionContext) != null? englishSubjectText.Get(executionContext) : string.Empty;
                string englishDescription = englishDescriptionText.Get(executionContext) != null ? englishDescriptionText.Get(executionContext) : string.Empty;
                string frenchSubject = frenchSubjectText.Get(executionContext) != null ? frenchSubjectText.Get(executionContext) : string.Empty;
                string frenchDescription = frenchDescriptionText.Get(executionContext) != null ? frenchDescriptionText.Get(executionContext) : string.Empty;
                string englishDelimiter = englishDelimiterTag.Get(executionContext) != null ? englishDelimiterTag.Get(executionContext) : string.Empty;
                string frenchDelimiter = frenchDelimiterTag.Get(executionContext) != null ? frenchDelimiterTag.Get(executionContext) : string.Empty;
                string genericSubject = string.Empty;
                string genericDescription = string.Empty;

                if(englishDelimiter != string.Empty) 
                {
                    genericSubject = englishDelimiter.Trim() +" "+englishSubject.Trim() +" ";
                    genericDescription = englishDelimiter.Trim() + " " + englishDescription.Trim() +" ";
                }
                if (frenchDelimiter != string.Empty)
                {
                    genericSubject += frenchDelimiter.Trim() + " " + frenchSubject.Trim() + " ";
                    genericDescription += frenchDelimiter.Trim() + " " + frenchDescription.Trim() +" ";
                }

                targetEntity.Attributes.Add(subjectSchemaName.Get(executionContext), genericSubject);
                targetEntity.Attributes.Add(descriptionSchemaName.Get(executionContext), genericDescription);

                service.Update(targetEntity);
                
            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }
    }
}
