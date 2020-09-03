function openRTFEditor(sender) {
    if (sender) {
        var note = $(sender).closest("div[data-note]");
        if (note) {
            var body = note.find(".card-body");
            var content = body.html();
            body.empty();

            $("<textarea></textarea>").appendTo(body).text(content).richText();
            note.find("[data-mode='save']").show();
            $(sender).hide();
        }
    }
}

function closeRTFEditor(sender) {
    if (sender) {
        var note = $(sender).closest("div[data-note]");
        if (note) {
            var body = note.find(".card-body");
            var editor = body.find("textarea");

            body.empty();
            body.append(editor.val());

            note.find("[data-mode='open']").show();
            $(sender).hide();
        }
    }
}

function saveNotePos(note) {
    var body = note.find(".card-body");

    var id = note.data('note');
    var posx = note.position().left;
    var posy = note.position().top;

    var url = "/note/" + id + "/savepos/@" + posx + "," + posy;

    $.ajax({
        method: "POST",
        url: url
    }).fail(function () {
        showToast("Saving position of note " + id + " failed!");
    }).done(function () {
    })
}

function saveNoteSize(note) {
    var body = note.find(".card-body");

    var id = note.data('note');
    var width = note.width();
    var height = note.height();

    var url = "/note/" + id + "/savesize/" + width + "x" + height;

    $.ajax({
        method: "POST",
        url: url
    }).fail(function () {
        showToast("Saving size of note " + id + " failed!");
    }).done(function () {
    })
}

$(document).ready(function () {
    $(".card").draggable({
        handle: ".card-header",
        cursor: "move",
        grid: [5, 5],
        distance: 20,
        stop: function (event, ui) {
            saveNotePos($(ui.helper));
        }
    });

    $(".card").resizable({
        grid: [5, 5],
        stop: function (event, ui) {
            saveNoteSize($(ui.helper));
        }
    });
});