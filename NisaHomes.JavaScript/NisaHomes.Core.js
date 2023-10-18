if (typeof (NisaHomes) === "undefined") {
    NisaHomes = { __namespace: true }
}


NisaHomes.Core = {
    properties: {
        releaseVersion: 1.0,
        executionContext: null,
        formContext: null,
        clientUrl: null,
        APIUrl:null,
        logErrors:true
    },

    errorLog: {
        message: ""
    },


    InitializeContext: function (executionContext) {
        var _properties = this.properties;
        _properties.executionContext = executionContext;
        _properties.formContext = NisaHomes.Core.GetFormContext(executionContext);

        _properties.clientUrl = NisaHomes.Core.RetrieveClientUrl();
        _properties.APIUrl = NisaHomes.Core.RetrieveAPIUrl();

    },


    //**************************************************
    //* GENERIC FUNCTIONS
    //**************************************************

    StripBraces: function (strGuid) {
        if (strGuid !== null) {
            return strGuid.replace('{', '').replace('}', '');
        }
        return null;
    },

    AppendErrorLog: function (functionName, message) {
        if (logErrors) {
            NisaHomes.Core.errorLog.message += "Error: " + functionName + ": " + message + "\n";
        }
    },

	//**************************************************
	//* FIELD LEVEL FUNCTIONS
	//**************************************************




	GetFieldValue: function (fieldName) {
        var attribute = NisaHomes.Core.GetAttribute(fieldName);
        if (attribute !== null) {
            return attribute.getValue();
        }
        return null;
    },

    GetFieldOptionsetText: function (fieldName) {
        var attribute = NisaHomes.Core.GetAttribute(fieldName);
        if (attribute !== null) {
            return attribute.getSelectedOption().text;
        }
        return null;
    },






    //**************************************************
	//* API DEPENDANT FUNCTIONS
	//**************************************************


    RefreshRollupField: function (entityName, recordId, rollup_fieldName) {

        //************************************
        //* USAGE
        //************************************
        //var entityName = 'opportunities'; // Plural name of Entity
        //var rollup_fieldName = 'arg_pcrevenuetotal'; // Rollup Field
        //var recordId = formContext.data.entity.getId().replace('{', '').replace('}', ''); // GUID of current record
        //refreshRollupField(ExecutionContext, entityName, entityId, rollup_fieldName); // Function Call to update value
        //************************************

        const CALLTYPE_ASYNCRONOUS = true;
        const CALLTYPE_SYNCRONOUS = !CALLTYPE_ASYNCRONOUS;


        // Method Calling and defining parameter
        var rollupAPIMethod = NisaHomes.Core.properties.APIUrl + "/CalculateRollupField(Target=@entityname,FieldName=@fieldname)?";
        // Passing Parameter Values
        rollupAPIMethod += "@entityname={'@odata.id':'" + entityName + "(" + recordId + ")'}&" +
            "@fieldname='" + rollup_fieldName + "'";
        var req = new XMLHttpRequest();
        req.open("GET", NisaHomes.Core.properties.clientUrl + rollupAPIMethod, CALLTYPE_ASYNCRONOUS);
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    NisaHomes.Core.AppendErrorLog("RefreshRollupField(" + entityName + "(" + recordId + ")." + rollup_fieldName + ")", "recalcualted successfully");
                }
                else {
                    NisaHomes.Core.AppendErrorLog("RefreshRollupField(" + entityName + "(" + recordId + ")." + rollup_fieldName + ")", "error recalculating");
                }
            }
        };
        req.send();
    },



    OpenQuickCreateFromCurrentRecord: function (entityLogicalName,successHandler,errorHandler) {

        var params = { name: "New Record" };

        var thisRecord = {
            entityType: NisaHomes.Core.GetEntityName(),
            id: NisaHomes.Core.GetRecordId()
        };

        Xrm.Utility.openQuickCreate(entityLogicalName, thisRecord, params).then(successHandler,errorHandler);
    },

    ShowFormControl: function (controlName) {
        var controlObject = NisaHomes.Core.GetControl(controlName);
        if (controlObject !== null) {
            controlObject.setVisible(true);
        }
        else {
            NisaHomes.Core.AppendErrorLog("ShowFormControl(" + controlName + ")", "control object is null");
        }
    },

    HideFormControl:function(controlName) {
        var controlObject = NisaHomes.Core.GetControl(controlName);
        if (controlObject !== null) {
            controlObject.setVisible(false);
        }
        else {
            NisaHomes.Core.AppendErrorLog("HideFormControl(" + controlName + ")", "control object is null");
        }
    },

    ShowFormSection: function (tabName,sectionName) {
        var sectionObject = NisaHomes.Core.GetFormSection(tabName, sectionName);
        if (sectionObject !== null) {
            sectionObject.setVisible(true);
        }
        else {
            NisaHomes.Core.AppendErrorLog("ShowFormSection(" + tabName + "," + sectionName + ")", "section object is null");
        }
    },

    HideFormSection: function (tabName, sectionName) {
        var sectionObject = NisaHomes.Core.GetFormSection(tabName, sectionName);
        if (sectionObject !== null) {
            sectionObject.setVisible(false);
        }
        else {
            NisaHomes.Core.AppendErrorLog("HideFormSection(" + tabName + "," + sectionName + ")", "section object is null");
        }
    },


    GetFormSection: function (tabName,sectionName) {
        var tabObject = NisaHomes.Core.GetFormTab(tabName);
        if (tabObject !== null) {
            var sectionObject = tabObject.sections.get(sectionName);
            if (sectionObject !== null) {
                return sectionObject;
            }
            else {
                NisaHomes.Core.AppendErrorLog("GetFormSection(" + tabName + "," + sectionName + ")", "section object is null");
            }
        }
        else {
            NisaHomes.Core.AppendErrorLog("GetFormSection(" + tabName + ","+sectionName+")", "tab object is null");
        }
        return null;
    },

    ShowFormTab: function (tabName) {
        var tabObject = NisaHomes.Core.GetFormTab(tabName);
        if (tabObject !== null) {
            tabObject.setVisible(true);
        }
        else {
            NisaHomes.Core.AppendErrorLog("ShowFormTab(" + tabName + ")", "tab object is null");
        }
    },

    HideFormTab: function (tabName) {
        var tabObject = NisaHomes.Core.GetFormTab(tabName);
        if (tabObject !== null) {
            tabObject.setVisible(false);
        }
        else {
            NisaHomes.Core.AppendErrorLog("ShowFormTab(" + tabName + ")", "tab object is null");
        }
    },

    GetFormTab: function (tabName) {
        if (NisaHomes.Core.properties.formContext !== null) {
            return NisaHomes.Core.properties.formContext.ui.tabs.get(tabName);
        }
        else {
            NisaHomes.Core.AppendErrorLog("GetFormTab(" + tabName + ")", "formContext is null");
        }
        return null;
    },

    GetControl: function (fieldLogicalName) {
        if (NisaHomes.Core.properties.formContext !== null) {
            return NisaHomes.Core.properties.formContext.getControl(fieldLogicalName);
        }
        else {
            NisaHomes.Core.AppendErrorLog("GetControl(" + fieldLogicalName + ")", "formContext is null");
        }
        return null;
    },


    GetAttribute: function (fieldLogicalName) {
        if (NisaHomes.Core.properties.formContext !== null) {
            return NisaHomes.Core.properties.formContext.getAttribute(fieldLogicalName);
        }
        else {
            NisaHomes.Core.AppendErrorLog("GetAttribute(" + fieldLogicalName +")", "formContext is null");
        }
        return null;
    },


    RetrieveClientUrl: function () {
        if (NisaHomes.Core.properties.formContext !== null) {
            return NisaHomes.Core.properties.formContext.context.getClientUrl();
        }
        else {
            NisaHomes.Core.AppendErrorLog("RetrieveClientUrl()", "formContext is null");
        }
        return null;
    },


    RetrieveAPIUrl: function () {
        var apiVersion = Xrm.Utility.getGlobalContext().getVersion();
        return "/api/data/v" + apiVersion.substring(3, apiVersion.indexOf(".") - 1);

    },

    SaveRecord: function () {
        if (NisaHomes.Core.properties.formContext !== null) {
            NisaHomes.Core.properties.formContext.data.entity.save();
        }
        else {
            NisaHomes.Core.AppendErrorLog("SaveRecord()", "formContext is null");
        }        
    },


    GetEntityName: function () {
        if (NisaHomes.Core.properties.formContext !== null) {
            return NisaHomes.Core.properties.formContext.data.entity.getEntityName();
        }
        else {
            NisaHomes.Core.AppendErrorLog("GetEntityName()", "formContext is null");
        }

        return null;
    },

    GetRecordId: function () {
        if (NisaHomes.Core.properties.formContext !== null) {

            var recordId = NisaHomes.Core.properties.formContext.data.entity.getId();
            return NisaHomes.Core.StripBraces(recordId);
        }
        else {
            NisaHomes.Core.AppendErrorLog("GetRecordId()", "formContext is null");
        }        

        return null;
    },


    GetFormContext: function (executionContext) {
        if (executionContext !== null) {
            return executionContext.getFormContext();
        }
        else {
            NisaHomes.Core.AppendErrorLog("GetFormContext", "Execution Context is null");
        }
        return null;
    }

}
