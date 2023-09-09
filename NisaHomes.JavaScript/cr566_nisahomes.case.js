
if (typeof (NisaHomes) === "undefined") 
{
    NisaHomes = { __namespace: true }
}

NisaHomes.Case = 
{
    properties: 
    {
        releaseVersion: 1.0,
        CaseTypeName: "",
        CaseStatus: "",
        ApplicationStage:""
    },

    //**************************************************
    //* FORM EVENT HANDLERS
    //**************************************************
    FormOnLoad: function (executionContext) {
        import("/WebResources/cr566_nisahomes.core.js").then(
            function () 
            {
                //Pass Context to Core Library
                NisaHomes.Core.InitializeContext(executionContext);

                //Call Business Action
                NisaHomes.Case.ShowHidePageTabs();            
            },

            //Error handler
            function (error) 
            {
                NisaHomes.Core.AppendErrorLog("FormOnLoad()", error);
            }
        );
    },

    CaseTypeOnChange: function (executionContext) 
    {
        NisaHomes.Case.ShowHidePageTabs();
    },

    ApplicationStageOnChange: function (executionContext) 
    {
        NisaHomes.Case.ShowHidePageTabs();
    },
    //**************************************************
    //* BUSINESS ACTIONS
    //**************************************************
    ShowHidePageTabs: function () 
    {
        //get CaseType
        NisaHomes.Case.properties.CaseTypeName = NisaHomes.Core.GetFieldOptionsetText("casetypecode");

        //get Application Stage
        NisaHomes.Case.properties.ApplicationStage = NisaHomes.Core.GetFieldOptionsetText("dcg_applicationstage");
   
        //SHOW APPROPRIATE TABS BASED ON CaseTYPE     
        if(NisaHomes.Case.properties.CaseTypeName === "NH Residential" 
            || NisaHomes.Case.properties.CaseTypeName === "NH Remote")
        {
            NisaHomes.Core.ShowFormTab("TAB_INCOME_VS_EXPENSES");
            NisaHomes.Core.ShowFormTab("TAB_FINANCIAL_ASSISTANCE_REQUEST");
        }
        else
        {
            NisaHomes.Core.HideFormTab("TAB_INCOME_VS_EXPENSES");
            NisaHomes.Core.HideFormTab("TAB_FINANCIAL_ASSISTANCE_REQUEST");
        }
                    
        // If case type is Residential and Application Stage == Checklist Timeline Show TAB_CHILDCARE_PROGRAM
        if(NisaHomes.Case.properties.CaseTypeName === "NH Residential" 
            && NisaHomes.Case.properties.ApplicationStage === "Check List Timeline")
        {
            NisaHomes.Core.ShowFormTab("TAB_CHILDCARE_PROGRAM");
        }
        else
        {
            NisaHomes.Core.HideFormTab("TAB_CHILDCARE_PROGRAM");          
        }
    }
}