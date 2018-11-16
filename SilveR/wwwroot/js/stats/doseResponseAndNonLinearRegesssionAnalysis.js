
$(function () {
    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Dose").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Offset").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#QCResponse").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#QCDose").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#SamplesResponse").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });


    $("#MinCoeff").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#MaxCoeff").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#SlopeCoeff").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#EDICCoeff").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#MinStartValue").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#MaxStartValue").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#SlopeStartValue").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#EDICStartValue").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });


    $("#EquationYAxis").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#EquationXAxis").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("[value='FourParameter']").click(function () {
        disableEquation();
    });

    $("[value='Equation']").click(function () {
        disableFourParameter();
    });

    function disableEquation() {
        $('#FourParameterPanel').removeClass("disabledpanel");
        $('#EquationPanel').addClass("disabledpanel");

        $("#EquationYAxis").data("kendoDropDownList").value("");
        $("#EquationXAxis").data("kendoDropDownList").value("");
    }

    function disableFourParameter() {
        $('#EquationPanel').removeClass("disabledpanel");
        $('#FourParameterPanel').addClass("disabledpanel");

        $("#Response").data("kendoDropDownList").value("");
        $("#Dose").data("kendoDropDownList").value("");
        $("#QCResponse").data("kendoDropDownList").value("");
        $("#QCDose").data("kendoDropDownList").value("");
        $("#SamplesResponse").data("kendoDropDownList").value("");
    }

    if (theModel.analysisType === 0) { //0 is FourParameter,1 is Equation
        disableEquation();
    }
    else {
        disableFourParameter();
    }
});