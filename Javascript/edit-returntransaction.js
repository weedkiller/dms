//Created By : Raphael Herrera, Created On : 7/18/2016
$(document).ready(function () {

    setTimeout(function () {

        hideDevField();

        createPostButton();
        // if open show cancel
        if ($('#gsc_vrstatus').val() == 100000000) {
            createCancelButton();
        }
        CreatePrintButton();

        var status = $(".record-status").html();
        if (status == 'Cancelled' || status == 'Returned') {
            $(".datetimepicker input").attr("disabled", "disabled");
            $(".datetimepicker span").hide();
            setReadOnly();
        }
    }, 300);

    // if cancelled/return hide delete. disable form
    if ($('#gsc_vrstatus').val() == 100000002 || $('#gsc_vrstatus').val() == 100000001) {
        $('.toolbar-right button.delete-link').remove();
        DMS.Helpers.DisableEntityForm();
    }

    function createPostButton() {
        var postBtn = DMS.Helpers.CreateButton('button', 'btn btn-primary', '', ' POST', DMS.Helpers.CreateFontAwesomeIcon('fa-thumb-tack'));
        postBtn.attr("id", "postButton");
        var postModal = DMS.Helpers.CreateModalConfirmation({
            id: 'postModal',
            headerTitle: ' Post - Return Transaction',
            Body: '<p>Post this Return Transaction?</p>',
            headerIcon: 'fa fa-thumb-tack'
        });
        $('.crmEntityFormView').append(postModal);
        postBtn.on('click', function (evt) {
            postModal.find('.confirmModal').on('click', function () {
                postTransaction();
            });
            postModal.modal('show');
        });
        DMS.Helpers.AppendButtonToToolbar(postBtn);
    }

    function createCancelButton() {
        var cancelIcon = DMS.Helpers.CreateFontAwesomeIcon('fa-ban');
        var cancelBtn = DMS.Helpers.CreateButton('button', 'btn btn-primary cancel', '', ' CANCEL', cancelIcon);
        var cancelConfirmation = DMS.Helpers.CreateModalConfirmation(
          {
              id: 'cancelModal',
              headerIcon: 'fa fa-ban', headerTitle: ' Cancel ', Body: 'Are you sure you want to cancel this Purchase Order Return?'
          });
        $(".crmEntityFormView").append(cancelConfirmation);
        cancelBtn.on('click', function (evt) {
            cancelConfirmation.find('.confirmModal').on('click', function () {
                $('#gsc_vrstatus').val('100000002');
                $('#UpdateButton').click();
            });
            cancelConfirmation.modal('show');
        });

        DMS.Helpers.AppendButtonToToolbar(cancelBtn);
    }

    function CreatePrintButton() {
        //Modified By : Jerome Anthony Gerero, Modified On : 7/28/2016
        $printBtn = DMS.Helpers.CreateAnchorButton("btn-primary btn", '', ' PRINT', DMS.Helpers.CreateFontAwesomeIcon('fa-print'));
        $printBtn.click(function (evt) {
            if (Page_ClientValidate("")) {
                var recordId = getQueryVariable('id');
                var protocol = window.location.protocol;
                var host = window.location.host;
                var url = protocol + '//' + host + '/report/?reportname={c13624c1-c7ed-e611-80ee-00155d010e2c}&reportid=' + recordId;
                window.open(url, 'blank', 'width=850,height=1000');
                event.preventDefault();
            }
        });
        DMS.Helpers.AppendButtonToToolbar($printBtn);
        //End modify
    }

    function setReadOnly() {
        //disable buttons
        $(".deactivate-link").addClass("permanent-disabled disabled");
        $("#UpdateButton").addClass("permanent-disabled disabled");
        $("#postButton").hide();

        //disbale fieldset
        $("#EntityFormView fieldset").attr("disabled", "disabled");
        $("#EntityFormView fieldset").addClass("permanent-disabled");
    }

    function postTransaction() {
        $('#gsc_vrstatus').val('100000001');
        $("#UpdateButton").click();
    }

    function getQueryVariable(variable) {
        var query = window.location.search.substring(1);
        var vars = query.split('&');
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split('=');
            if (pair[0] == variable) {
                return pair[1];
            }
        }
    }

    function hideDevField() {
        $('select[id="gsc_vrstatus"], label[for="gsc_vrstatus"]').hide();
        $('#gsc_vrstatus').css({ "pointer-events": "none", "cursor": "default" });
        $('#gsc_returnstatus').css({ "pointer-events": "none", "cursor": "default" });
        $('#gsc_vrstatus').attr('readOnly', true);
        $('#gsc_returnstatus').attr('readOnly', true);
    }

    setTimeout(disableTab, 300);

    function disableTab() {
        $('.permanent-disabled').attr("tabindex", "-1");
    }
});