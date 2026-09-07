// ===============================
// CKEDITOR SIMPLE GLOBAL INIT
// ===============================

window._ckEditors = window._ckEditors || new Map();

// INIT
// window.initCkEditors = function (container) {

//     console.log("INIT CKEDITOR CALLED");

//     if (typeof ClassicEditor === "undefined") {
//         console.error("❌ CKEditor CDN NOT LOADED");
//         return;
//     }

//     const scope = container || document;

//     scope.querySelectorAll('.editor-full').forEach(element => {

//         if (window._ckEditors.has(element)) return;

//         ClassicEditor
//             .create(element, {

                
//                 toolbar: [
//                     'undo', 'redo',
//                     '|',
//                     'bold', 'italic', 'underline',
//                     '|',
//                     'link', 'insertImage', 'insertTable',
//                     '|',
//                     'bulletedList', 'numberedList',
//                     '|',
//                     'alignment',
//                     '|',
//                     'sourceEditing' // 🔥 IMPORTANT
//                 ]
//             })
//             .then(editor => {
//                 window._ckEditors.set(element, editor);
//                 console.log("✅ CKEditor initialized");
//             })
//             .catch(err => {
//                 console.log(window.ClassicEditor)
//                 console.error("CKEditor error:", err);
//             });

//     });
// };



window.initCkEditors = function (container) {

    console.log("INIT CKEDITOR CALLED");

    const scope = container || document;

    const Editor = window.ClassicEditor || (window.CKEDITOR && window.CKEDITOR.ClassicEditor);

    if (!Editor) {
        console.error("❌ CKEditor NOT LOADED");
        return;
    }

    scope.querySelectorAll('.editor-full').forEach(element => {

        if (window._ckEditors.has(element)) return;

        Editor.create(element, {
            toolbar: [
                'undo', 'redo',
                '|',
                'bold', 'italic', 'underline',
                '|',
                'link', 'insertImage', 'insertTable',
                '|',
                'bulletedList', 'numberedList',
                '|',
                'alignment',
                '|',
                'sourceEditing' // ✅ WORKS ONLY WITH SUPER BUILD
            ]
        })
        .then(editor => {
            window._ckEditors.set(element, editor);
            console.log("✅ CKEditor initialized with SOURCE");
        })
        .catch(err => console.error(err));

    });
};





// ===============================
// SYNC CKEDITOR → TEXTAREA
// ===============================
window.syncCkEditorsToSource = function (container) {
    const scope = container || document;

    scope.querySelectorAll('.editor-full').forEach(el => {
        const editor = window._ckEditors.get(el);
        if (editor) {
            el.value = editor.getData();
        }
    });
};

// ===============================
// DESTROY (FOR MODAL)
// ===============================
window.destroyCkEditors = function (container) {
    const scope = container || document;

    scope.querySelectorAll('.editor-full').forEach(el => {
        const editor = window._ckEditors.get(el);
        if (editor) {
            editor.destroy().then(() => {
                window._ckEditors.delete(el);
            });
        }
    });
};

// ===============================
// AUTO INIT MAIN PAGE
// ===============================
document.addEventListener("DOMContentLoaded", function () {

    console.log("DOM READY");

    // ✅ SUPPORT BOTH CLASSIC + SUPER BUILD
    const Editor = window.ClassicEditor || (window.CKEDITOR && window.CKEDITOR.ClassicEditor);

    if (!Editor) {
        console.error("❌ CKEDITOR NOT LOADED AT ALL");
        return;
    }

    console.log("✅ CKEditor FOUND");

    // 🔥 Pass editor to your init
    window.initCkEditors(document);
});