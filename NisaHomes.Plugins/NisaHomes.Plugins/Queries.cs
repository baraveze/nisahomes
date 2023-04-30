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

        public Queries(IOrganizationService service) { 
            
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
    }
}
