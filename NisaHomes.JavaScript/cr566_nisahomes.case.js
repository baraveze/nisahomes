
if (typeof (NisaHomes) === "undefined") {
    NisaHomes = { __namespace: true }
}



NisaHomes.Case = {
    properties: {
        releaseVersion: 1.0,
        CaseTypeName: "",
        CaseStatus: ""        
    },




    //**************************************************
    //* FORM EVENT HANDLERS
    //**************************************************

    FormOnLoad: function (executionContext) {
        import("/WebResources/cr566_nisahomes.core.js").then(
            function () {
                //Pass Context to Core Library
                NisaHomes.Core.InitializeContext(executionContext);

                //Call Business Action
                NisaHomes.Case.ShowHidePageTabs();
              
                
                /*
                var cr566_primarysectorLookup = NisaHomes.Core.GetFieldValue("cr566_primarysectorid");
                if(cr566_primarysectorLookup != null && cr566_primarysectorLookup[0].name === "05 Hospitality + Gaming")
                    NisaHomes.Core.ShowFormControl("cr566_brandid");
                else
                    NisaHomes.Core.HideFormControl("cr566_brandid");
                        */
            },
            //Error handler
            function (error) {
                NisaHomes.Core.AppendErrorLog("FormOnLoad()", error);
            }
        );
    },


    CaseTypeOnChange: function (executionContext) {

        NisaHomes.Case.ShowHidePageTabs();

    },

   

    //**************************************************
    //* BUSINESS ACTIONS
    //**************************************************

   
    ShowHidePageTabs: function () {


        //get CaseType
        NisaHomes.Case.properties.CaseTypeName = NisaHomes.Core.GetFieldOptionsetText("casetypecode");
   
        //SHOW APPROPRIATE TABS BASED ON CaseTYPE
      

                if(NisaHomes.Case.properties.CaseTypeName === "NH Residential" ||
                   NisaHomes.Case.properties.CaseTypeName === "NH Remote"){
                    NisaHomes.Core.ShowFormTab("TAB_INCOME_VS_EXPENSES");
                    NisaHomes.Core.ShowFormTab("TAB_FINANCIAL_ASSISTANCE_REQUEST");
                   }
                else{
                    NisaHomes.Core.HideFormTab("TAB_INCOME_VS_EXPENSES");
                    NisaHomes.Core.HideFormTab("TAB_FINANCIAL_ASSISTANCE_REQUEST");
                }
                    
    }


}