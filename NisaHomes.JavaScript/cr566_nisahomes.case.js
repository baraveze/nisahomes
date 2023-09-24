
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
    FormOnLoad: function (executionContext)
    {
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

    OnSave: function (executionContext)
    {
        import("/WebResources/cr566_nisahomes.core.js").then(
            function () {
                //Pass Context to Core Library
                NisaHomes.Core.InitializeContext(executionContext);

                //Call Business Action
                NisaHomes.Case.ShowHidePageTabs();
            },

            //Error handler
            function (error) {
                NisaHomes.Core.AppendErrorLog("OnSave()", error);
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
        let incidentApplicationStage = NisaHomes.Core.GetFieldOptionsetText("dcg_applicationstage");
   
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
            && incidentApplicationStage === "Check List Timeline")
        {
            NisaHomes.Core.ShowFormTab("TAB_CHILDCARE_PROGRAM");
        }
        else
        {
            NisaHomes.Core.HideFormTab("TAB_CHILDCARE_PROGRAM");          
        }

        switch (incidentApplicationStage)
        {
            case "Inquiry":
                NisaHomes.Core.ShowFormTab("TAB_INQUIRY");
                NisaHomes.Core.HideFormTab("TAB_WAITLIST");
                NisaHomes.Core.HideFormTab("TAB_RINA");
                NisaHomes.Core.HideFormTab("TAB_REMOTE_PROGRAM");
                NisaHomes.Core.HideFormTab("TAB_SUPPORT");
                NisaHomes.Core.HideFormTab("TAB_WRAP_UP");
                break;

            case "Waitlist":
                NisaHomes.Core.ShowFormTab("TAB_WAITLIST");
                NisaHomes.Core.HideFormTab("TAB_RINA");
                NisaHomes.Core.HideFormTab("TAB_REMOTE_PROGRAM");
                NisaHomes.Core.HideFormTab("TAB_SUPPORT");
                NisaHomes.Core.HideFormTab("TAB_WRAP_UP");
                break;

            case "Remote":

                NisaHomes.Core.ShowFormTab("TAB_RINA");
                NisaHomes.Core.ShowFormTab("TAB_REMOTE_PROGRAM");
                NisaHomes.Core.ShowFormTab("TAB_SUPPORT");
                NisaHomes.Core.ShowFormTab("TAB_WRAP_UP");

                let rinaFinished = NisaHomes.Core.GetFieldValue("cr566_remoteintakeneedsassessmentfinished");
                let remoteProgramFinished = NisaHomes.Core.GetFieldValue("cr566_remoteprogramfinished");
                let supportFinished = NisaHomes.Core.GetFieldValue("cr566_supportstagefinished");
                let wrapUpFinished = NisaHomes.Core.GetFieldValue("cr566_wrapupstagefinished");

                if (rinaFinished === true) {
                    NisaHomes.Core.ShowFormTab("TAB_REMOTE_PROGRAM");
                }
                else {
                    NisaHomes.Core.HideFormTab("TAB_REMOTE_PROGRAM");
                }

                if (remoteProgramFinished === true) {
                    NisaHomes.Core.ShowFormTab("TAB_SUPPORT");
                }
                else {
                    NisaHomes.Core.HideFormTab("TAB_SUPPORT");
                }

                if (supportFinished === true) {
                    NisaHomes.Core.ShowFormTab("TAB_WRAP_UP");
                }
                else {
                    NisaHomes.Core.HideFormTab("TAB_WRAP_UP");
                }

                break;

        }

    }
}