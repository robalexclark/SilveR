$(function () {

   
    $("#LowerBoundAbsolute").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        step: 0.01
    });

    $("#UpperBoundAbsolute").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        step: 0.01
    });

    $("#LowerBoundPercentageChange").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        min: 0,
        max: 1000
    });

    $("#UpperBoundPercentageChange").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        min: 0,
        max: 1000
    });


    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    enableDisableControlLevels();
  
});