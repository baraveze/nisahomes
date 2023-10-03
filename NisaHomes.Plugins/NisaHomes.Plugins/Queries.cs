using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NisaHomes.Plugins
{
    public class Queries
    {
        private IOrganizationService _service;

        public Queries(IOrganizationService service) 
        {            
            _service = service;
        }
        public string getApprover1Email(Guid systemuserid) {

            string _approverEmail = "";

            if (systemuserid != Guid.Empty)
            {
                string _fetchquery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                        "<entity name='systemuser'>" +
                                        "<attribute name='systemuserid' />" +
                                        "<attribute name='internalemailaddress' />" +
                                        "<order attribute='internalemailaddress' descending='false' />" +
                                        "<filter type='and'>" +
                                        "<condition attribute='systemuserid' operator='eq' value='{" + systemuserid + "}' />" +
                                        "</filter>" +
                                        "</entity>" +
                                        "</fetch>";

             EntityCollection fetchqueryCollection = _service.RetrieveMultiple(new FetchExpression(_fetchquery));
                if(fetchqueryCollection.Entities.Count>0) {
                    _approverEmail = fetchqueryCollection.Entities[0].GetAttributeValue<String>("internalemailaddress");
                }
            }          
            return _approverEmail;
        }

        public ApprovalCategoryType getApprovalFlowLogic(int assistanceType, decimal amount)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                              "<entity name='cr566_approvalflowlogicassignment'>" +
                                "<attribute name='cr566_approvalflowlogicassignmentid' />" +
                                "<attribute name='cr566_name' />" +
                                "<attribute name='cr566_category' />" +
                                "<attribute name='cr566_approvaltype' />" +
                                "<order attribute='cr566_name' descending='false' />" +
                                "<filter type='and'>" +
                                  "<filter type='or'>" +
                                    "<filter type='and'>" +
                                      "<condition attribute='cr566_assistancetype' operator='eq' value='"+assistanceType+"' />" +
                                      "<condition attribute='cr566_anyamount' operator='eq' value='1' />" +
                                      "<condition attribute='statecode' operator='eq' value='0' />" +
                                    "</filter>" +
                                    "<filter type='and'>" +
                                      "<condition attribute='cr566_assistancetype' operator='eq' value='"+assistanceType+"' />" +
                                      "<condition attribute='cr566_anyamount' operator='eq' value='0' />" +
                                      "<condition attribute='statecode' operator='eq' value='0' />" +
                                      "<condition attribute='cr566_from' operator='le' value='"+amount+"' />" +
                                      "<condition attribute='cr566_to' operator='ge' value='"+amount+"' />" +
                                    "</filter>" +
                                  "</filter>" +
                                "</filter>" +
                              "</entity>" +
                            "</fetch>";

            try
            {
                EntityCollection queryCollection = _service.RetrieveMultiple(new FetchExpression(query));
                if (queryCollection.Entities.Count > 0)
                {
                    if (queryCollection.Entities.Count > 1)
                    {
                        throw new InvalidPluginExecutionException("Please, check configuration on \"Approval Flow Logic Assignment\" table. there are more than one combination for Assystance Type and Amount." +
                            "So the process cannot pick an unique value to find appropriate Approval configuration.");
                    }
                    return new ApprovalCategoryType
                    {
                        category = queryCollection.Entities[0].GetAttributeValue<OptionSetValue>("cr566_category").Value,
                        approvalType = queryCollection.Entities[0].GetAttributeValue<OptionSetValue>("cr566_approvaltype").Value
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Internal query getApprovalFlowLogic: " + ex.Message);
            }
        }
        public Guid getMultiLevelApprovalConfiguration(int approvalType, Guid caseworker)
        {

            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='cr566_multilevelapprovalsettings'>" +
                            "<attribute name='cr566_multilevelapprovalsettingsid' />" +
                            "<attribute name='cr566_name' />" +
                            "<order attribute='cr566_name' descending='false' />" +
                            "<filter type='and'>" +
                                "<condition attribute='cr566_caseworkerid' operator='eq' value='{" + caseworker + "}' />" +
                                 "<condition attribute='cr566_approvaltype' operator='eq' value='" + approvalType + "' />" +
                            "</filter>" +
                            "</entity>" +
                        "</fetch>";
            try
            {
                EntityCollection queryCollection = _service.RetrieveMultiple(new FetchExpression(query));
                if (queryCollection.Entities.Count > 0)
                {
                    if (queryCollection.Entities.Count > 1)
                    {
                        throw new InvalidPluginExecutionException("Please, check configuration on \"Multi-level Approval Setting\" table. there are more than one combination for Assystance Type and Approval type." +
                             "So the process cannot pick an unique value to find appropriate Multi-level Approval Setting.");
                    }
                    return queryCollection.Entities[0].Id;
                }
                return Guid.Empty;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Query getMultiLevelApprovalConfiguration: " + ex.Message);
            }


        }
        /// <summary>
        /// Deprecated method
        /// <remarks>Deprecated method since new logic in the process. But will remain in the source if we need to retake this way again.</remarks>
        /// </summary>
        /// <param name="category"></param>
        /// <param name="approvalType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        public Guid getMultiLevelApprovalConfiguration( int category, int approvalType) {

            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>"+
                            "<entity name='cr566_multilevelapprovalsettings'>"+
                            "<attribute name='cr566_multilevelapprovalsettingsid' />"+
                            "<attribute name='cr566_name' />"+
                            "<order attribute='cr566_name' descending='false' />"+
                            "<filter type='and'>"+
                                "<condition attribute='cr566_approvaltype' operator='eq' value='" + approvalType+"' />"+
                                "<condition attribute='cr566_category' operator='eq' value='"+category+"' />"+
                                "<condition attribute='statecode' operator='eq' value='0' />" +
                            "</filter>" +
                            "</entity>"+
                        "</fetch>";
            try
            {
                EntityCollection queryCollection = _service.RetrieveMultiple(new FetchExpression(query));
                if (queryCollection.Entities.Count > 0)
                {
                    if (queryCollection.Entities.Count > 1)
                    {
                        throw new InvalidPluginExecutionException("Please, check configuration on \"Multi-level Approval Setting\" table. there are more than one combination for Assystance Type and Category." +
                             "So the process cannot pick an unique value to find appropriate Multi-level Approval Setting.");
                    }
                    return queryCollection.Entities[0].Id;
                }
                return Guid.Empty;
            }catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Internal query getMultiLevelApprovalConfiguration: " +ex.Message);
            }            
        }
    }
}