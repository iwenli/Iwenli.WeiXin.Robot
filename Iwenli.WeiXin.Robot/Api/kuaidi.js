//查询前单号验证
function validateKuaidiNumber() {
    if ($("#queryWait").is(":visible")) { // 正在查询中->返回
        return false;
    }
    gKuaidiNumber = $("#postid").val().Trim();
    if (gCompanyCode == "rufengda" && checkRegOfcompany(gKuaidiNumber, "^\\d{16}$")) {
        gKuaidiNumber = "DD" + gKuaidiNumber;
    }
    $("#postid").val(gKuaidiNumber);
    gValiCode = $("#valicode").val().Trim(); // 验证
    var errorListName = "";
    if ($("#companyListType").val() != null && $("#companyListType").val() == "wuliuCompanyList") {
        errorListName = "物流";
    } else {
        errorListName = "快递";
    }
    if (gCompanyCode == "") {
        $("#errorTips").show();
        if (gQueryType == 13 || gQueryType == 14) {
            $("#errorMessage").html("请您在上方选择一家" + errorListName + "公司");
        }
        else {
            $("#errorMessage").html("请您在左侧列表中选择一家" + errorListName + "公司");
        }
        return false;
    }
    if (gKuaidiNumber == "" || gKuaidiNumber == $("#postid").attr("placeholder")) {
        $("#errorTips").show();
        $("#errorMessage").html("请您填写" + errorListName + "单号。");
        $("#postid").focus();
        return false;
    }
    if (!isNumberLetterFuhao(gKuaidiNumber)) {
        $("#errorTips").show();
        $("#errorMessage").html("单号仅能由数字、字母和特殊符号组合，请您查证。");
        $("#postid").focus();
        return false;
    }
    if (gKuaidiNumber.length < 5) {
        $("#errorTips").show();
        $("#errorMessage").html("单号不能小于5个字符，请您查证。");
        $("#postid").focus();
        return false;
    }
    if (gKuaidiNumber.length > 40) {
        $("#errorTips").show();
        $("#errorMessage").html("单号不能超过40个字符，请您查证。");
        $("#postid").focus();
        return false;
    }
    if (gCheckStr != '' && gCheckStr != null) {
        if (!checkRegOfcompany(gKuaidiNumber, gCheckStr)) {
            $("#errorTips").show();
            $("#errorMessage").html(gCheckInfo);
            $("#postid").focus();
            return false;
        }
    }
    return true;
}

//查询
function getResult(companyCode, kuaidiNumber) {
    var url = "/query";
    if (gHasVali == "1" || gHasVali == "2") {
        url = "/queryvalid";
    }
    gCompanyCode = companyCode;
    gKuaidiNumber = kuaidiNumber;

    if (queryurl != "" && isavailable == 1) {
        queryFromUrl();
        return false;
    }

    var agrs = "type=" + gCompanyCode + "&postid=" + gKuaidiNumber + "&id=" + gQueryType + "&valicode=" + gValiCode + "&temp=" + Math.random();
    url = url + "?" + agrs;
    $("#queryWait").show();
    $("#companyNum").html(gKuaidiNumber);
    $(".logo-model").show();
    $("#resultHeader").hide();
    $("#comList").hide();
    $("#example").hide();

    gLoading = 1;
    gAjaxGet = $.ajax({
        type: "GET",
        url: url,
        timeout: gTimeout,
        success: function (responseText) {
            $("#postid").select();
            gLoading = 0;
            $("#queryWait").hide();
            $(".logo-model").hide();
            $("#sendHistory").hide();
            $("#resultHeader").show();
            gIsCheck = 0;
            if (responseText.length == 0) {
                $("#notFindTip").show();
                return;
            }
            var resultJson = eval("(" + responseText + ")");
            gResultJson = resultJson;
            gQueryResult = resultJson.status;
            if (resultJson.status == 200) { //成功
                //$(".merry-snow").hide();//主题-圣诞-snow
                var sortStatus = getcookie("sortStatus");
                var resultData = resultJson.data;
                gResultData = resultData;
                gIsCheck = resultJson.ischeck;
                if (sortStatus == 0) {
                    gSortStatus = 0;
                    sortToggle();
                }
                else if (sortStatus == 1) {
                    gSortStatus = 1;
                    sortToggle();
                }
                $("#example").hide();
                $("#queryContext").show();
                $("#shareBtn").show();
                $("#queryPs").show();
                //  getTimecost();
                getQueryQr();
                $("#inputTips").hide();
                //$("#postid").select();
            }
            else if (resultJson.status == 408) { // 验证码错误
                $("#errorTips").show();
                $("#shareBtn").hide();
                $("#resultHeader").hide();
                if (gQueryType == 2) {
                    $("#errorMessage").html("需要验证码，请到快递查询页面输入验证码查询！");
                }
                else {
                    $("#errorMessage").html("您输入的验证码错误，请重新输入！");
                }
                if ($("#valideBox").is(":visible")) {
                    $("#valicode").focus();
                }
            }
            else if (resultJson.status == 201) { // 单号没查到
                $("#notFindTip").show();
                $("#shareBtn").hide();
                $("#example").hide();
                $("#resultHeader").hide();
            }
            else if (resultJson.status == 700) {
                queryFromUrl();
            }
            else {
                $("#notFindTip").show();
                $("#shareBtn").hide();
                $("#example").hide();
                $("#resultHeader").hide();
            }
            if (gHasVali == "1") {
                refreshCode();
            }
            queryHistoryFrame.attr("src", "//cache.kuaidi100.com/index.html?option=add&gCompanyCode=" + gCompanyCode + "&gKuaidiNumber=" + gKuaidiNumber + "&gIsCheck" + gIsCheck);
        },
        error: function (xmlHttpRequest, error) {
            gLoading = 0;
            if (error == "timeout") {
                onTimeout();
            }
        }
    });
    if (window.location.href.indexOf("www.kuaidi100.com/") != -1 && window.location.href.indexOf("www.kuaidi100.com/all/") == -1) {
        getReaddImg();
    }
}