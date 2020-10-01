function loadScratchpads() {
    $list = $("#sel-scratch");
    $.ajax({
        url: "/scratchpad/list",
        type: "GET",
        data: { projectid: $("#sel-project").val() },
        traditional: true,
        success: function (result) {
            $list.empty();
            $.each(result, function (i, item) {
                $list.append('<option value="' + item["scratchpadId"] + '"> ' + item["title"] + ' </option>');
            });
        },
        error: function () {
            showToast("Load scratchpads failed!");
        }
    });
}

$(document).ready(function () {
    $("#sel-project").on("change", loadScratchpads);
});