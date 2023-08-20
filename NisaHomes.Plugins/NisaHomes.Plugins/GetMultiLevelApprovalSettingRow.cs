namespace NisaHomes.Plugins
{ 

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

  public  enum assistanceType : int
    {
        Rent = 734190000,
        Food = 734190001,
        Kitchen_Supplies = 734190002,
        Moving_Truck = 734190003,
        Necessary_Furniture = 734190004,
        Basic_Necessities = 734190005,
        Bill_Payment = 734190006,
        Transportation = 734190007,
        Small_debts = 734190008,
        Hotel_Accommodation = 734190009,
        Other = 734190010,
        Welcome_Package = 734190011,
        Grocery_Support = 734190012
    }
    public enum distributionMethod : int
    {
        Cheque = 734190000,
        Direct_Deposit = 734190001,
        Bank_Draft  = 734190002,
        Electronic_Gift_Card = 734190003,
        Physical_Gift_Card = 734190004,
        Manager_CC = 734190005
    }
    public class GetMultiLevelApprovalSettingRow : IPlugin
    {   /// <summary>  
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
            {
                //If status code is equal to Pending Approval
                if (Target.Contains("statuscode") && Target.GetAttributeValue<OptionSetValue>("statuscode").Value == 734190000)  
                {
                    
                    // Step 1: Validate if we have all necessary fields: caseworker / Assistance Type / Distribution method / Total Amount
                    Entity FinancialRequest = service.Retrieve("cr566_financialassistancerequest", Target.Id, new ColumnSet("cr566_assistancetype", "cr566_distributionmethod", "cr566_totalamountitems", "ownerid"));
                    if (FinancialRequest != null 
                     && FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_assistancetype") != null 
                     && FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_distributionmethod") != null 
                     && FinancialRequest.GetAttributeValue<EntityReference>("ownerid") != null)
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
                        if (FinancialRequest.GetAttributeValue<Money>("cr566_totalamountitems") != null) 
                        {
                            _totalAmount = FinancialRequest.GetAttributeValue<Money>("cr566_totalamountitems").Value;
                        }

                        // Category selection
                        if (_assistanceType == (int)assistanceType.Welcome_Package)
                        {
                            _selectedCategory = _category1;                            
                        }
                        else
                        {
                            if (_assistanceType == (int)assistanceType.Transportation
                             || _assistanceType == (int)assistanceType.Food
                             || _assistanceType == (int)assistanceType.Grocery_Support
                             || _distributionMethod == (int)distributionMethod.Electronic_Gift_Card
                             || _distributionMethod == (int)distributionMethod.Physical_Gift_Card
                             || _distributionMethod == (int)distributionMethod.Manager_CC)
                            {
                                if (_totalAmount <= 200)
                                {
                                    _selectedCategory = _category1;
                                }
                                else 
                                {
                                    _selectedCategory = _category2;
                                }                                
                            }                          
                        }

                        Guid _multiLevelApprovalSetting = getConfiguRowQuery.getMultiLevelApprovalConfiguration(_selectedCategory, _ownerid);

                        if( _multiLevelApprovalSetting != Guid.Empty ) 
                        {
                            // Step 3: Update Financial Request with the Configuration found and put in True the field that trigger powerautomate approval process 
                            Entity FinancialRequestUpdate = new Entity("cr566_financialassistancerequest");
                            FinancialRequestUpdate.Id = Target.Id;
                            FinancialRequestUpdate.Attributes.Add("cr566_relatedmultilevelapprovalsettingid", new EntityReference("cr566_multilevelapprovalsettings", _multiLevelApprovalSetting));
                            
                            // Add Logic to establish CC emails
                            service.Update(FinancialRequestUpdate);
                        }
                        else 
                        {
                            string categoryString = "Category 1";
                            if (_selectedCategory != 734190000) 
                            { 
                                categoryString = "Category 2"; 
                            }

                            Entity caseworker = service.Retrieve("systemuser", _ownerid, new ColumnSet("fullname"));
                            string caseworkername = caseworker.GetAttributeValue<String>("fullname");
                            throw new InvalidPluginExecutionException($"You can not complete this task because any Multi-Level Approval Setting was found with this combination of Case Worker: {caseworkername} and Category: {categoryString}."+
                                " Please, contact a System Administrator to include this configuration. Then try this action again.");
                        }
                    }
                    else 
                    {
                        throw new InvalidPluginExecutionException("Empty values. There are important fields without value. Please check into Financial Assistance Request record and" +
                                                                   "see if Assistance Type, Distribution Method, Caseworker has value");
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException($"Error on GetMultiLevelApprovalSettingRow Plugin: {e.Message}");
            }
        }
    }
}
