function openRTFEditor(sender) {
    if (sender && $(sender).data('note')) {
        var id = $(sender).data('note');
        var note = $("#note-" + id);
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
    if (sender && $(sender).data('note')) {
        var id = $(sender).data('note');
        var note = $("#note-" + id);
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