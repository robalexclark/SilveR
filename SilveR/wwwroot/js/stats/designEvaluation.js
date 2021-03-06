﻿
$(function () {

    $("#FixedFactors").kendoMultiSelect({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#RandomFactors").kendoMultiSelect({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#RandomisedFrom").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#RandomisedTo").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#RLSObjects").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#RLSTerms").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#UserRLSLabels").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#UserDefinedTerms").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#SmallFontMultiplier").kendoNumericTextBox({
        "format": "0"
    });

    $("#MediumFontMultiplier").kendoNumericTextBox({
        "format": "0"
    });

    $("#LargeFontMultiplier").kendoNumericTextBox({
        "format": "0"
    });
});