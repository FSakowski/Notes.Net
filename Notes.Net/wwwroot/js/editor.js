function openRTFEditor(sender) {
    if (sender) {
        var note = $(sender).closest("div[data-note]");
        if (note) {
            // remove static values for width and height to adjust the div to the size if the editor
            note.data("width", note.width());
            note.data("height", note.height());
            note.css("width", "");
            note.css("height", "");

            // set to front
            note.css("z-index", 9999);

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
            // restore orginal width and height
            note.width(note.data("width"));
            note.height(note.data("height"));
            note.removeData("width");
            note.removeData("height");

            note.css("z-index", '');

            var body = note.find(".card-body");
            var editor = body.find("textarea");

            saveNoteContent(note, editor.val());

            body.empty();
            body.append(editor.val());

            note.find("[data-mode='open']").show();
            $(sender).hide();
        }
    }
}

function saveNoteContent(note, content) {
    var id = note.data('note');

    var url = "/note/" + id + "/save";

    $.ajax({
        method: "POST",
        processData: false,
        contentType: 'text/plain',
        data: content,
        url: url
    }).fail(function () {
        showToast("Saving the note " + id + " failed!");
    }).done(function () {
        showToast("Note " + id + " has been successfully saved");
    })
}

function saveNotePos(note) {
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