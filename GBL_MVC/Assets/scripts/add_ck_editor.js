var ck = '.editor-full';
let editors = [];

import {
    ClassicEditor,
    Essentials,
    Paragraph,
    Bold,
    Italic,
    Font,
    SourceEditing,
    GeneralHtmlSupport,
    Image,
    ImageUpload,
    ImageToolbar,
    ImageCaption,
    ImageStyle,
    ImageResize,
    SimpleUploadAdapter,
    MediaEmbed
} from 'ckeditor5';

function getEditorConfig() {
    return {
        plugins: [
            Essentials,
            Paragraph,
            Bold,
            Italic,
            Font,
            SourceEditing,
            GeneralHtmlSupport,
            Image,
            ImageUpload,
            ImageToolbar,
            ImageCaption,
            ImageStyle,
            ImageResize,
            SimpleUploadAdapter,
            MediaEmbed
        ],
        toolbar: [
            'undo', 'redo', '|', 'bold', 'italic', '|',
            'fontSize', 'fontFamily', 'fontColor', 'fontBackgroundColor', 'sourceEditing', '|',
            'insertImage', 'imageStyle:alignLeft', 'imageStyle:alignCenter', 'imageStyle:alignRight', 'mediaEmbed'
        ],
        mediaEmbed: {
            previewsInData: true
        },
        image: {
            toolbar: [
                'imageTextAlternative',
                'toggleImageCaption',
                '|',
                'imageStyle:alignLeft',
                'imageStyle:alignCenter',
                'imageStyle:alignRight',
                '|',
                'resizeImage:25', 'resizeImage:50', 'resizeImage:75', 'resizeImage:original'
            ],
            styles: [
                'alignLeft',
                'alignCenter',
                'alignRight'
            ],
            resizeOptions: [
                {
                    name: 'resizeImage:original',
                    value: null,
                    label: 'Original'
                },
                {
                    name: 'resizeImage:25',
                    value: '25',
                    label: '25%'
                },
                {
                    name: 'resizeImage:50',
                    value: '50',
                    label: '50%'
                },
                {
                    name: 'resizeImage:75',
                    value: '75',
                    label: '75%'
                }
            ],
            upload: {
                types: ['jpeg', 'png', 'gif', 'bmp', 'webp', 'svg']
            }
        },
        simpleUpload: {
            uploadUrl: '/UploadFile/UploadImage'
        },
        htmlSupport: {
            allow: [
                {
                    name: /.*/,
                    classes: true,
                    attributes: true
                },
                {
                    name: 'div',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'p',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'span',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'a',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'table',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'tbody',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'thead',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'tr',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'td',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'th',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'ul',
                    classes: true,
                    attributes: true
                },
                {
                    name: 'li',
                    classes: true,
                    attributes: true
                }
            ]
        }
    };
}

function initEditorElement(element) {
    if (!element || element.editorInstance) {
        return;
    }
    // Avoid duplicate ClassicEditor.create when initCkEditors is invoked repeatedly before .then runs
    if (element.dataset.ckInitPending === "1") {
        return;
    }
    element.dataset.ckInitPending = "1";

    ClassicEditor.create(element, getEditorConfig())
        .then((editor) => {
            element.editorInstance = editor;
            editors[element.id] = editor;
            delete element.dataset.ckInitPending;
        })
        .catch((error) => {
            delete element.dataset.ckInitPending;
            console.error(error);
        });
}

// Same as before: init all .editor-full on first page load
document.querySelectorAll(ck).forEach((element) => {
    initEditorElement(element);
});

/** Init editors inside a container (e.g. AJAX-loaded modal). Skips already-initialized fields. */
window.initCkEditors = function (root) {
    const scope = root && root.querySelectorAll ? root : document;
    scope.querySelectorAll(ck).forEach((element) => {
        initEditorElement(element);
    });
};

/** Sync CKEditor HTML back into textareas before form submit / serialize */
window.syncCkEditorsToSource = function (root) {
    const scope = root && root.querySelectorAll ? root : document;
    scope.querySelectorAll(ck).forEach((element) => {
        const ed = element.editorInstance;
        if (!ed) return;
        if (typeof ed.updateSourceElement === "function") {
            ed.updateSourceElement();
        } else {
            element.value = ed.getData();
        }
    });
};

/** Destroy editors in a subtree (e.g. before removing modal HTML) */
window.destroyCkEditors = function (root) {
    if (!root || !root.querySelectorAll) return;
    root.querySelectorAll(ck).forEach((element) => {
        delete element.dataset.ckInitPending;
        if (element.editorInstance) {
            const ed = element.editorInstance;
            element.editorInstance = null;
            if (typeof ed.destroy === 'function') {
                ed.destroy().catch(() => {});
            }
        }
    });
};
