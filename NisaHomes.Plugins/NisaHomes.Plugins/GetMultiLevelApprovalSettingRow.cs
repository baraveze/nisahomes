namespace NisaHomes.Plugins
{ 

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    public class GetMultiLevelApprovalSettingRow : IPlugin
    {   /// <summary>
        /// Assistance Type 
            ///Rent	734190000
            ///Food	734190001
            ///Kitchen supplies	734190002
            ///Moving truck	734190003
            ///Necessary furniture	734190004
            ///Basic Necessities	734190005
            ///Bill payment	734190006
            ///Transportation	734190007
            ///Small debts(payday loans, personal loans)  734190008
            ///Hotel/motel accommodation	734190009
            ///Other	734190010
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <exception cref="InvalidPluginExecutionException"></exception>
        
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            Entity Target = (Entity)context.InputParameters["Target"];

            try
            {   //If status code is equal to Pending Approval
                if (Target.Contains("statuscode") && Target.GetAttributeValue<OptionSetValue>("statuscode").Value == 734190000)  {


                    // Step 1: Validate if we have all necessary fields: caseworker / Assistance Type / Distribution method / Total Amount
                    Entity FinancialRequest = service.Retrieve("cr566_financialassistancerequest", Target.Id, new ColumnSet("cr566_assistancetype", "cr566_distributionmethod", "cr566_totalamountitems", "ownerid"));
                    if (FinancialRequest != null &&
                       FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_assistancetype") != null &&
                       FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_distributionmethod") != null &&
                       FinancialRequest.GetAttributeValue<EntityReference>("ownerid") != null)
                    {

                        // Step 2: Establish the Category of the request we are looking for
                        int _assistanceType     = FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_assistancetype").Value;
                        int _distributionMethod = FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_distributionmethod").Value;
                        int _category1 = 734190000;
                        int _category2 = 734190001;
                        int _selectedCategory = 0;
                        Guid _ownerid = FinancialRequest.GetAttributeValue<EntityReference>("ownerid").Id;
                        Decimal _totalAmount = 0;
                        Queries getConfiguRowQuery = new Queries(service);
                        if (FinancialRequest.GetAttributeValue<Money>("cr566_totalamountitems") != null) {
                            _totalAmount = FinancialRequest.GetAttributeValue<Money>("cr566_totalamountitems").Value;
                        }

                        if (_totalAmount < 200 && (_assistanceType == 734190007 || _distributionMethod == 734190003))
                        { _selectedCategory = _category1; }
                        else { _selectedCategory = _category2; }

                        Guid _multiLevelApprovalSetting = getConfiguRowQuery.getMultiLevelApprovalConfiguration(_selectedCategory, _ownerid);

                        if( _multiLevelApprovalSetting != Guid.Empty ) {

                            // Step 3: Update Financial Request with the Configuration found and put in True the field that trigger powerautomate approval process 

                            Entity FinancialRequestUpdate = new Entity("cr566_financialassistancerequest");
                            FinancialRequestUpdate.Id = Target.Id;
                            FinancialRequestUpdate.Attributes.Add("cr566_relatedmultilevelapprovalsettingid", new EntityReference("cr566_multilevelapprovalsettings", _multiLevelApprovalSetting));
                            
                            // Add Logic to establish CC emails
                            service.Update(FinancialRequestUpdate);


                        }
                        else {
                            string categoryString = "Category 1";
                            if (_selectedCategory != 734190000) { categoryString = "Category 2"; }

                            Entity caseworker = service.Retrieve("systemuser", _ownerid, new ColumnSet("fullname"));
                            string caseworkername = caseworker.GetAttributeValue<String>("fullname");
                            throw new InvalidPluginExecutionException("You can not complete this task because any Multi-Level Approval Setting was found with this combination of Case Worker: "+caseworkername +" and Category: "+categoryString+ "."+
                                " Please, contact a System Administrator to include this configuration. Then try this action again.");
                        }

                    }
                    else {
                        throw new InvalidPluginExecutionException("Empty values. There are important fields without value. Please check into Financial Assistance Request record and" +
                                                                   "see if Assistance Type, Distribution Method, Caseworker has value");
                    }

                }
                

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException("Error on GetMultiLevelApprovalSettingRow Plugin: " + e.Message);
            }
        }
    }
}
