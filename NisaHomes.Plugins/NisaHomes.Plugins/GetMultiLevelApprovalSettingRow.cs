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

    public class ApprovalCategoryType
    {
       public int category;
       public int approvalType;
         
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
                    
                    // Step 1: Validate if we have all necessary fields: Assistance Type / Total Amount
                    Entity FinancialRequest = service.Retrieve("cr566_financialassistancerequest", Target.Id, new ColumnSet("cr566_assistancetype", "cr566_distributionmethod", "cr566_totalamountitems", "ownerid"));
                   
                    if (FinancialRequest != null 
                     && FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_assistancetype") != null )                    
                    {
                        // Step 2: Search Category and Approval type of the request we are looking for
                        int _assistanceType     = FinancialRequest.GetAttributeValue<OptionSetValue>("cr566_assistancetype").Value;
                         
                        //int _category1 = 734190000;
                        //int _category2 = 734190001;
                        //int _selectedCategory = 0;
                        //Guid _ownerid = FinancialRequest.GetAttributeValue<EntityReference>("ownerid").Id;
                        Decimal _totalAmount = 0;
                        Queries getConfiguRowQuery = new Queries(service);

                        if (FinancialRequest.GetAttributeValue<Money>("cr566_totalamountitems") != null) 
                        {
                            _totalAmount = FinancialRequest.GetAttributeValue<Money>("cr566_totalamountitems").Value;
                        }

                        // Category selection
                        ApprovalCategoryType approvalFlowType = getConfiguRowQuery.getApprovalFlowLogic(_assistanceType, _totalAmount);
                        if (approvalFlowType == null)
                        {
                            throw new InvalidPluginExecutionException("It does not exist a combination for the Assistance Type and amount provided. " +
                            "Please, check configuration on \"Approval Flow Logic Assignment\" table.");
                        }

                        Guid _multiLevelApprovalSetting = getConfiguRowQuery.getMultiLevelApprovalConfiguration(approvalFlowType.category, approvalFlowType.approvalType);

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
                            throw new InvalidPluginExecutionException($"You can not complete this action because any Multi-Level Approval Setting was found based on Category and Approval Type. Contact an Administrator to checkout configuration.");
                        }
                    }
                    else 
                    {
                        throw new InvalidPluginExecutionException("Empty values. There are important fields without value. Please check into Financial Assistance Request record and" +
                                                                   "see if Assistance Type has value.");
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
