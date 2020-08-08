var currentSearch;
var currentFileType;
let urlBrowser,
    itemsPerPage,
    selectionType,
    multipleSelectGridId,
    singleDivId,
    ckEditorId,
    urlGetMediaInfo,
    isEditorReadOnly;


let InsertToArr = {
    Editor: 1,
    Single: 2,
    Multiple: 3
};

let mediaTypes = {
    image: "img",
    sound: "sound",
    video: "video",
    other: "other"
}

let InsertToWhere = "";
var selectedItemsArr = [];

let AMZGalleryBrowser = {

    Init: function initializeAMZGallery(options) {
        urlBrowser = options.url;
        multipleSelectGridId = options.multipleGridId;
        itemsPerPage = options.itemsPerPage;

        selectionType = options.selectionType;
        singleDivId = options.singleDiv;
        ckEditorId = options.ckeditorId;

        isEditorReadOnly = options.isEditorReadOnly;

        urlGetMediaInfo = options.urlGetMediaInfo;

        if (selectionType === "multiple") {
            printPlaceHolderFirstTime(multipleSelectGridId);
        }


        if (options.isEditorReadOnly) {
            $(`#${ckEditorId}`).ckeditor({
                readOnly: true
            });
            console.log(isEditorReadOnly);
        } else {
            bindCkEditor(ckEditorId);
        }
    }
}




function printPlaceHolderFirstTime(id) {
    let container =
        `<div class="row" id="gallery-selected-items-grid">
            <div class="col-12">
                <div id="gallery-selected-imgs" class="row"></div>
            </div>
            <div class="col-12">
                <div id="gallery-selected-allOther" class="row"></div>
            </div>
        </div>`;
    document.getElementById(id).innerHTML = container;
}


function generateGallery(currentMediaType, pageNum, searchTerm, currentSearchTerm) {
    $.ajax({
        url: urlBrowser,
        dataType: "json",
        data: {
            fileType: currentMediaType,
            pageNumber: pageNum,
            searchString: searchTerm,
            currentFilter: currentSearchTerm,
            pageSize: itemsPerPage
        },
        success: function (data) {
            PrintAjaxResult(data);
        }
    });
}


function PrintAjaxResult(data) {
    var galleryGrid = $("#gallery-grid");
    var galleryTable = $("#gallery-table");

    galleryGrid.empty();
    galleryTable.empty();

    //if result is not empty
    if (data.data.length !== 0) {

        let fileType = data.mediaType;

        if (fileType === mediaTypes.image) {
            for (let i = 0; i < data.data.length; i++) {
                let name = data.data[i].name;
                let link = "/media/img/" + name;

                let img = document.createElement("Img");
                img.setAttribute("src", link);
                img.setAttribute("data-id", data.data[i].id);

                let divCol = document.createElement("Div");
                divCol.setAttribute("class", "col-lg-3 col-md-4 col-sm-6 col-12");

                let galleryItem = document.createElement("Div");
                galleryItem.setAttribute("class", "gallery-grid-item");

                let captionSpan = document.createElement("Span");
                captionSpan.setAttribute("class", "gallery-caption");
                captionSpan.innerHTML = name;

                galleryItem.append(img);
                galleryItem.append(captionSpan);

                divCol.append(galleryItem);
                galleryGrid.append(divCol);

            }
        } else {


            for (let i = 0; i < data.data.length; i++) {

                let name = data.data[i].name;

                let img = document.createElement("Img");
                if (fileType === mediaTypes.sound) {
                    img.setAttribute("src", "/img/panel/gallery-browser/sound.png");
                } else if (fileType === mediaTypes.video) {
                    img.setAttribute("src", "/img/panel/gallery-browser/video.png");
                } else {
                    img.setAttribute("src", "/img/panel/gallery-browser/other.png");
                }

                img.setAttribute("data-id", data.data[i].id);

                let divCol = document.createElement("Div");
                divCol.setAttribute("class", "col-lg-3 col-md-4 col-sm-6 col-12");

                let galleryItem = document.createElement("Div");
                galleryItem.setAttribute("class", "gallery-grid-item");

                let captionSpan = document.createElement("Span");
                captionSpan.setAttribute("class", "gallery-caption");
                captionSpan.innerHTML = name;

                galleryItem.append(img);
                galleryItem.append(captionSpan);

                divCol.append(galleryItem);
                galleryGrid.append(divCol);
            }
        }




        //Enable And Disable Next/Prev Buttons
        if (data.hasNext) {
            $("#btnNext").removeClass("disabled");
        } else {
            $("#btnNext").addClass("disabled");
        }

        if (data.hasPrev) {
            $("#btnPrev").removeClass("disabled");
        } else {
            $("#btnPrev").addClass("disabled");
        }

        //Set Next/Prev Page Index
        document.getElementById("btnNext").dataset.nextpage = data.nextPageIndex;
        document.getElementById("btnPrev").dataset.prevpage = data.prevPageIndex;




        //Current Filters Values
        currentSearch = data.currentSearchString;
        currentFileType = data.mediaType;


        //Change Current Button Color
        changeButtonColor();



        //Select Items That Are Selected In The Array !
        SelectingPreviousItems(selectedItemsArr);
    }

    else {
        var messageP = document.createElement("P");
        messageP.setAttribute("id", "gallery-emptyMessage");
        messageP.innerHTML = "هیچ رسانه ای یافت نشد";
        galleryGrid.append(messageP);
    }

    console.log("Hello" + selectedItemsArr);
    GallerySelecting(selectedItemsArr);
}




function changeButtonColor() {
    //Change Current Filter Button
    let buttons = document.getElementById("filter-btns").children;
    for (let i = 0; i < buttons.length; i++) {
        if (buttons[i].dataset.filetype === currentFileType) {
            buttons[i].setAttribute("class", "btn btn-danger");
        } else {
            buttons[i].setAttribute("class", "btn btn-dark");
        }
    }
}




if (document.getElementById("filter-btns")) {
    document.getElementById("filter-btns").addEventListener("click", function (e) {

        if (e.target.dataset.filetype === mediaTypes.image) {
            generateGallery(mediaTypes.image, 1, null, currentSearch);
        } else if (e.target.dataset.filetype === mediaTypes.sound) {
            generateGallery(mediaTypes.sound, 1, null, currentSearch);
        } else if (e.target.dataset.filetype === mediaTypes.video) {
            generateGallery(mediaTypes.video, 1, null, currentSearch);
        } else if (e.target.dataset.filetype === mediaTypes.other) {
            generateGallery(mediaTypes.other, 1, null, currentSearch);
        }
    });
}

if (document.getElementById("pagination-btns")) {
    document.getElementById("pagination-btns").addEventListener("click", function (e) {

        if (e.target.hasAttribute("data-nextpage")) {
            if (!($("#btnNext").hasClass("disabled"))) {
                let pageNumber = e.target.dataset.nextpage;
                generateGallery(currentFileType, pageNumber, null, currentSearch);
            }
        } else if (e.target.hasAttribute("data-prevpage")) {
            if (!($("#btnPrev").hasClass("disabled"))) {
                let pageNumber = e.target.dataset.prevpage;
                generateGallery(currentFileType, pageNumber, null, currentSearch);
            }
        }

    });
}


if (document.getElementById("txtSearch")) {
    document.getElementById("txtSearch").addEventListener("keyup", function (e) {
        let query = document.getElementById("txtSearch").value;
        if (currentSearch != null) {
            generateGallery(currentFileType, 1, query, currentSearch);
        } else {
            generateGallery(currentFileType, 1, query, null);
        }
    });
}






function SelectingPreviousItems(itemsArr) {
    let galleryGrid = document.getElementById("gallery-grid");
    for (var i = 0; i < galleryGrid.children.length; i++) {
        let img = galleryGrid.children[i].children[0].children[0];
        if (itemsArr.indexOf(img.dataset.id) >= 0) {
            if (!img.classList.contains("gallery-selected")) {
                img.setAttribute("class", "gallery-selected");
            }
        }
    }
}


function bindCkEditor(id) {
    CKEDITOR.replace(id,
        {
            filebrowserImageBrowseUrl: 'null'
        });

    CKEDITOR.on('dialogDefinition',
        function (ev) {
            var editor = ev.editor;
            var dialogDefinition = ev.data.definition;
            var tabCount = dialogDefinition.contents.length;
            for (var i = 0; i < tabCount; i++) {
                var browseButton = dialogDefinition.contents[i].get('browse');
                if (browseButton !== null) {
                    browseButton.onClick = function (dialog, i) {
                        editor._.filebrowserSe = this;
                        InsertToWhere = InsertToArr.Editor;
                        generateGallery(mediaTypes.image, 1, null, currentSearch);
                        $("#galleryModal").appendTo('body').modal('show');
                    }
                }
            }
        });

}

function InsertToCkEditor(value) {
    var dialog = CKEDITOR.dialog.getCurrent();
    dialog.setValueOf('info', 'txtUrl', value);
}


//Insert media to CkEditor
if (document.getElementById("gallery-grid")) {
    document.getElementById("gallery-grid").addEventListener("click",
        function (e) {
            if (e.target.tagName === "IMG") {

                var imgSrc = e.target.getAttribute("src");

                if (InsertToWhere === InsertToArr.Editor) {
                    InsertToCkEditor(imgSrc);
                    $("#galleryModal").modal('hide');
                } else if (InsertToWhere === InsertToArr.Single) {
                    let id = e.target.dataset.id;
                    InsertToMediaId(imgSrc, id);
                    $("#galleryModal").modal('hide');
                } else {

                    let id = e.target.dataset.id;

                    //Selecting
                    if (!e.target.classList.contains("gallery-selected")) {

                        if (!(selectedItemsArr.indexOf(id) >= 0)) {
                            selectedItemsArr.push(id);

                            GallerySelecting(selectedItemsArr);

                            GetSelectItemJSONData(id);
                        }
                    }
                    //DeSelecting
                    else {
                        GalleryDeSelecting(selectedItemsArr, id);
                        e.target.removeAttribute("class");
                    }


                }

            }
        });
}



function GetSelectItemJSONData(id) {
    $.ajax({
        url: urlGetMediaInfo,
        data: {
            id: id
        },
        dataType: "json",
        success: function (data) {

            PrintSingleItemFromAjaxData(data, id);
        }
    });
}


function PrintSingleItemFromAjaxData(data, id) {
    if (data.filetype === mediaTypes.image) {
        let selectedImgs = document.getElementById("gallery-selected-imgs");
        let img = document.createElement("img");
        let div = document.createElement("Div");
        div.setAttribute("class", "col-lg-3 col-md-4 col-sm-6 col-12 mt-2");
        div.setAttribute("data-id", id);
        img.setAttribute("src", `${data.link}`);
        div.append(img);
        selectedImgs.append(div);
    } else {
        let selectedSounds = document.getElementById("gallery-selected-allOther");
        let anchor = document.createElement("a");
        let div = document.createElement("Div");
        div.setAttribute("data-id", id);
        div.setAttribute("class", "col-12");
        anchor.setAttribute("href", `${data.link}`);
        anchor.setAttribute("class", "text-danger");
        anchor.innerHTML = data.name;
        div.append(anchor);
        selectedSounds.append(div);
    }
}



function InsertToMediaId(src, id) {
    document.getElementById("media-preview").setAttribute("src", src);
    document.getElementById("input-mediaId").value = id;
}


function GallerySelecting(itemsArr) {
    let galleryGrid = document.getElementById("gallery-grid");


    for (let i = 0; i < galleryGrid.children.length; i++) {

        let img = galleryGrid.children[i].children[0].children[0];

        if (itemsArr.indexOf(img.dataset.id) >= 0) {
            if (!img.classList.contains("gallery-selected")) {
                img.setAttribute("class", "gallery-selected");
            }
        }

    }

}



function GalleryDeSelecting(itemsArr, itemValue) {
    if (itemsArr.indexOf(itemValue) >= 0) {
        itemsArr.splice(itemsArr.indexOf(itemValue), 1);
    }

    let imgsGrid = document.getElementById("gallery-selected-imgs");
    let otherGrid = document.getElementById("gallery-selected-allOther");


    for (let i = 0; i < imgsGrid.children.length; i++) {
        if (imgsGrid.children[i].dataset.id === itemValue) {
            imgsGrid.children[i].remove();
        }
    }

    for (let i = 0; i < otherGrid.children.length; i++) {
        if (otherGrid.children[i].dataset.id === itemValue) {
            otherGrid.children[i].remove();
        }
    }

}


if ($("#btn-selectMedia-media-mutliple")) {
    $("#btn-selectMedia-media-mutliple").click(function (e) {
        InsertToWhere = InsertToArr.Multiple;
        generateGallery(mediaTypes.image, 1, null, currentSearch);
        $("#galleryModal").appendTo('body').modal('show');
    });
}


if ($("#btn-selectMedia")) {
    $("#btn-selectMedia").click(function () {
        InsertToWhere = InsertToArr.Single;
        generateGallery(mediaTypes.image, 1, null, currentSearch);
        $("#galleryModal").appendTo('body').modal('show');
    });
}



function GetSelectedItemsArray() {
    return selectedItemsArr;
}

function AddToSelectedItemsArray(id) {
    selectedItemsArr.push(id);
}