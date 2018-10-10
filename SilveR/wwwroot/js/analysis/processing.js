window.onload = function poll() {
    setTimeout(function () {
        const url = "/Analyses/AnalysisCompleted?analysisGuid=" + analysisGuid;

        $.ajax({
            url: url,
            dataType: 'json',
            type: "post",
            success: function (response) {
                if (response) {
                    const resultPath = "../Analyses/ViewResults?analysisGuid=" + analysisGuid;
                    window.location = resultPath;
                }
                else {
                    poll();
                }
            },
            dataType: "json"
        })
    }, 3000)
};