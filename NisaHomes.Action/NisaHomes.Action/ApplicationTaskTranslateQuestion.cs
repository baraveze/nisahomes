

namespace NisaHomes.Action
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class ApplicationTaskTranslateQuestion : CodeActivity
    {
        [RequiredArgument]
        [Input("Subject")]
        public InArgument<string> subjectText { get; set; }

        [RequiredArgument]
        [Input("Description")]
        public InArgument<string> descriptionText { get; set; }

        [RequiredArgument]
        [Input("Entity Schema Name (prefix_entity)")]
        public InArgument<string> tableName { get; set; }
        
        [RequiredArgument]
        [Input("English Subject Schema Name")]
        public InArgument<string> englishSubjectSchemaname { get; set; }

        [RequiredArgument]
        [Input("French Subject Schema Name")]
        public InArgument<string> frenchSubjectSchemaname { get; set; }

        [RequiredArgument]
        [Input("English Description Schema Name")]
        public InArgument<string> englishDescriptionSchemaname { get; set; }
        
        [RequiredArgument]
        [Input("French Description Schema Name")]
        public InArgument<string> frenchDescriptionSchemaname { get; set; }
                
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

                string subject = subjectText.Get(executionContext);
                string description = descriptionText.Get(executionContext);

                if (subject == string.Empty && description == string.Empty)
                {
                    return;
                }

                string subject_en = string.Empty;
                string subject_fr = string.Empty;
                string description_en = string.Empty;
                string description_fr = string.Empty;

                // English Subject
                if (subject.IndexOf(englishDelimiterTag.Get(executionContext)) >= 0)
                {
                    if (subject.IndexOf(frenchDelimiterTag.Get(executionContext)) >= 0)
                    {
                        int englishSubjectOffset = englishDelimiterTag.Get(executionContext).Length;
                        int beginEnglishSubjectIndex = subject.IndexOf(englishDelimiterTag.Get(executionContext)) + englishSubjectOffset;
                        int endEnglishSubjectIndex = subject.IndexOf(frenchDelimiterTag.Get(executionContext));
                        subject_en = subject.Substring(beginEnglishSubjectIndex, endEnglishSubjectIndex - beginEnglishSubjectIndex);

                        targetEntity.Attributes.Add(englishSubjectSchemaname.Get(executionContext), subject_en);
                    }                   
                }
               
                // French Subject
                if (subject.IndexOf(frenchDelimiterTag.Get(executionContext)) >= 0)
                {
                    int frenchhSubjectOffset = frenchDelimiterTag.Get(executionContext).Length;
                    int beginFrenchSubjectIndex = subject.IndexOf(frenchDelimiterTag.Get(executionContext)) + frenchhSubjectOffset;
                    int endFrenchSubjectIndex = subject.Length;
                    subject_fr = subject.Substring(beginFrenchSubjectIndex, endFrenchSubjectIndex - beginFrenchSubjectIndex);


                    targetEntity.Attributes.Add(frenchSubjectSchemaname.Get(executionContext), subject_fr);
                }

                // English Description
                if (description.IndexOf(englishDelimiterTag.Get(executionContext)) >= 0)
                {
                    if (description.IndexOf(frenchDelimiterTag.Get(executionContext)) >= 0)
                    {
                        int englishDescriptionOffset = englishDelimiterTag.Get(executionContext).Length;
                        int beginEnglishDescriptionIndex = description.IndexOf(englishDelimiterTag.Get(executionContext)) + englishDescriptionOffset;
                        int endEnglishDescriptionIndex = description.IndexOf(frenchDelimiterTag.Get(executionContext));
                        description_en = description.Substring(beginEnglishDescriptionIndex, endEnglishDescriptionIndex - beginEnglishDescriptionIndex);

                        targetEntity.Attributes.Add(englishDescriptionSchemaname.Get(executionContext), description_en);
                    }
                }

                // French Description
                if (description.IndexOf(frenchDelimiterTag.Get(executionContext)) >= 0)
                {
                    int frenchDescriptionOffset = frenchDelimiterTag.Get(executionContext).Length;
                    int beginFrenchDescriptionIndex = description.IndexOf(frenchDelimiterTag.Get(executionContext)) + frenchDescriptionOffset;
                    int endFrenchDescriptionIndex = description.Length;
                    description_fr = description.Substring(beginFrenchDescriptionIndex, endFrenchDescriptionIndex - beginFrenchDescriptionIndex);

                    targetEntity.Attributes.Add(frenchDescriptionSchemaname.Get(executionContext), description_fr);
                }
                

                if ((subject_en != string.Empty && subject_fr != string.Empty) || (description_en != string.Empty && description_fr != string.Empty))
                {
                    service.Update(targetEntity);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }
        }
    }
