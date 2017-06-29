//Created By : Raphael Herrera, Created On : 8/1/2016
$(document).ready(function () {
    //transfer status
    var transferStatus = $('#gsc_transferstatus').val();
    var status = $(".record-status").html();

    if (status == "Cancelled" || status == "Posted") {
        checkSubgrid();
    }

    function checkSubgrid() {
        if ($('table[data-name="tab_5_section_4"]').is(":visible")) {
            $('table[data-name="tab_5_section_4"]').parent().addClass("permanent-disabled");
            $('table[data-name="tab_5_section_4"]').parent().attr("disabled", "disabled");
        }
        else {
            setTimeout(function () { checkSubgrid(); }, 50);
        }
    }

    checkPermission();
    function checkPermission() {
        if (DMS.Settings.Permission.Update == null) {
            setTimeout(function () {
                checkPermission();
            }, 100);
        }
        else {
            if (DMS.Settings.Permission.Update == true)
                drawAllocateButton();
        }
    }

    if ($('a:contains("DISPLAY")').is(':visible'))
        $('a:contains("DISPLAY")').click();

    // comment here please
    //if(transferStatus != 100000000 || $('.record-status').html() == "")  
    drawPostButton();

    $('#AllocatedVehicles .entity-grid.subgrid').on('loaded', function () {
        if ($('#AllocatedVehicles tr').length > 1 && status == "Open") {
            if ($('.post').length == 1)
                $('.post').removeClass("permanent-disabled disabled");
        }
    });

    if (transferStatus != 100000001) {
        setTimeout(function () {
            // DMS.Helpers.DisableEntityForm();
        }, 500);
        setTimeout(function () {
            //   DMS.Helpers.DisableEntityForm();
        }, 1000);
    }
    if (status == "Open") createCancelButton();

    $('#gsc_inventoryidtoallocate').hide();
    $('#gsc_inventoryidtoallocate_label').hide();
    $('#gsc_inventoryidtoallocate').val('');

    $('#gsc_transferstatus').hide();
    $('#gsc_transferstatus_label').hide();

    /*transferStatus == Posted*/
    if (status != "Open")
        disableFields();

    setTimeout(function () {
        RefreshAvailableItems($(".btn-primary").closest("div #Inventory"), 1, 4);

        $('.btn-primary').on('click', function (e) {

            var $subgrid = $(this).closest(".subgrid");
            var $subgridId = $subgrid.parent().attr("id");

            if ($subgridId == "Inventory") {
                e.preventDefault();
                e.stopPropagation();

                RefreshAvailableItems($subgrid.parent(), 1, 4);
            }
        });

    }, 3000);


    function RefreshAvailableItems($parent, page, PageSize) {
        var $subgrid = $parent.children(".subgrid");
        var $table = $subgrid.children(".view-grid").find("table");
        var $tbody = $("<tbody></tbody>");
        var $errorMessage = $subgrid.children(".view-error");
        var $emptyMessage = $subgrid.children(".view-empty");
        var $accessDeniedMessage = $subgrid.children(".view-access-denied");
        var $loadingMessage = $subgrid.children(".view-loading");
        var $pagination = $subgrid.children(".view-pagination");
        var url = $subgrid.data("get-url");
        var layout = $subgrid.data("view-layouts");
        var configuration = layout[0].Configuration;
        var base64SecureConfiguration = layout[0].Base64SecureConfiguration;
        var sortExpression = $table.data("sort-expression");

        $subgrid.children(".view-grid").find("tbody").remove();

        $errorMessage.hide().prop("aria-hidden", true);
        $emptyMessage.hide().prop("aria-hidden", true);
        $accessDeniedMessage.hide().prop("aria-hidden", true);

        $loadingMessage.show().prop("aria-hidden", false);

        $pagination.hide();

        var odataUrl = "/_odata/inventory?$filter=gsc_status/Value eq 100000000";
        $.ajax({
            type: "get",
            async: true,
            url: odataUrl,
            success: function (inventory) {

                var filteredInventory = inventory.value.filter(FilterInventory);

                ReCreateInventoryTable($parent, filteredInventory, page, PageSize);
            }
        });

    }

    //filtered inventory of the filter criteria
    function FilterInventory(data) {

        var colorIdFilter = $("#gsc_colorid_name").val();
        var siteIdFilter = $("#gsc_sitecriteriaid").val();
        var modelDescriptionFilter = $("#gsc_productid").val();
        var modelCodeFilter = $("#gsc_modelcode").val();
        var optionCodeFilter = $("#gsc_optioncode").val();
        var baseModelFilter = $('#gsc_vehiclebasemodelid').val();

        var colorId = data["gsc_color"];
        var siteId = data["gsc_iv_productquantity-gsc_siteid"];
        var productId = data["gsc_iv_productquantity-gsc_productid"];
        var modelCode = data["gsc_modelcode"];
        var optionCode = data["gsc_optioncode"];
        var baseModel = data["gsc_iv_productquantity-gsc_vehiclemodelid"];

        var removeData = false;

        if (colorIdFilter != null && colorIdFilter != "")
            if (colorId != colorIdFilter)
                removeData = true;

        if (siteIdFilter != null && siteIdFilter != "")
            if (siteId != null) {
                if (siteId.Id != siteIdFilter)
                    removeData = true;
            }
            else
                removeData = true;

        if (modelDescriptionFilter != null && modelDescriptionFilter != "")
            if (productId != null) {
                if (productId.Id != modelDescriptionFilter)
                    removeData = true;
            }
            else
                removeData = true;

        if (baseModelFilter != null && baseModelFilter != "")
            if (baseModel != null) {
                if (baseModel.Id != baseModelFilter)
                    removeData = true;
            }
            else
                removeData = true;

        if (modelCodeFilter != null && modelCodeFilter != "")
            if (modelCodeFilter != modelCode)
                removeData = true;

        if (optionCodeFilter != null && optionCodeFilter != "")
            if (optionCodeFilter != optionCode)
                removeData = true;

        if (removeData == false)
            return data;

    }

    function ReCreateInventoryTable($parent, data, page, PageSize) {
        var $subgrid = $parent.children(".subgrid");
        var $table = $subgrid.children(".view-grid").find("table");
        var $tbody = $("<tbody></tbody>");
        var $errorMessage = $subgrid.children(".view-error");
        var $emptyMessage = $subgrid.children(".view-empty");
        var $accessDeniedMessage = $subgrid.children(".view-access-denied");
        var $loadingMessage = $subgrid.children(".view-loading");
        var $pagination = $subgrid.children(".view-pagination");
        var url = $subgrid.data("get-url");
        var layout = $subgrid.data("view-layouts");
        var configuration = layout[0].Configuration;
        var base64SecureConfiguration = layout[0].Base64SecureConfiguration;
        var sortExpression = $table.data("sort-expression");

        $subgrid.children(".view-grid").find("tbody").remove();

        $errorMessage.hide().prop("aria-hidden", true);
        $emptyMessage.hide().prop("aria-hidden", true);
        $accessDeniedMessage.hide().prop("aria-hidden", true);

        $loadingMessage.show().prop("aria-hidden", false);


        if (typeof data !== typeof undefined && data !== false && (data == null || data.length == 0)) {
            $emptyMessage.fadeIn().prop("aria-hidden", false);
            $loadingMessage.hide().prop("aria-hidden", true);
            return;
        }

        var columns = $.map($table.find("th"), function (e) {
            return $(e).data('field');
        });

        var nameColumn = columns.length == 0 ? "" : columns[0] == "col-select" ? columns[1] : columns[0];

        $subgrid.data("total-record-count", data.length);

        var pageStart = (parseInt(page) - 1) * parseInt(PageSize);
        var pageEnd = parseInt(pageStart) + (parseInt(PageSize - 1));

        data.forEach(function (item, index) {

            if ((index < pageStart)) {
                return true;
            }
            else if ((index > pageEnd)) {
                return false;
            }

            var record = item;
            var name = record.gsc_inventorypn;

            var $tr = $("<tr></tr>")
                .attr("data-id", record.gsc_iv_inventoryid)
                .attr("data-entity", configuration.EntityName)
                .attr("data-name", name)
                .on("focus", function () {
                    $(this).addClass("active");
                })
                .on("blur", function () {
                    $(this).removeClass("active");
                });

            for (var j = 0; j < columns.length; j++) {
                var found = false;

                $.each(item, function (key, value) {
                    if (key == columns[j]) {
                        var html = value;
                        if (typeof value === 'object') {
                            var $td = $("<td></td>").attr('data-value', JSON.stringify(value));
                            if (value != null && typeof value.Name !== 'undefined') {
                                $td.html(value.Name);
                            }
                            $tr.append($td);
                            found = true;
                            return false;
                        }

                        var $td = $("<td></td>")
                            .attr("data-attribute", value)
                            .attr("data-value", value)
                            .html(html);
                        $tr.append($td);
                        found = true;
                        return false;
                    }
                });
                if (!found) {
                    var typeColumn = columns[j];

                    var $td = $("<td></td>")
                        .attr("data-attribute", columns[j]);

                    $tr.append($td);
                };
            }

            $tbody.append($tr);
        });

        $subgrid.children(".view-grid").children("table").append($tbody.show());
        $subgrid.fadeIn();
        initializePagination(data, $parent, page);
        $loadingMessage.hide().prop("aria-hidden", true);

    }

    function initializePagination(data, $parent, PageNumber) {
        // requires ~/js/jquery.bootstrap-pagination.js

        var $subgrid = $parent.children(".subgrid");
        var $pagination = $subgrid.children(".view-pagination");
        var ItemCount = data.length;
        var PageSize = 4;
        var PageCount = Math.round(ItemCount / PageSize); //Add by ARM

        if (typeof data === typeof undefined || data === false || data == null) {
            $pagination.hide();
            return;
        }

        if (PageCount <= 1) {
            $pagination.hide();
            return;
        }

        $pagination
            .data("pagesize", PageSize)
            .data("pages", PageCount)
            .data("current-page", PageNumber)
            .data("count", ItemCount)
            .off("click")
            .pagination({
                total_pages: $pagination.data("pages"),
                current_page: $pagination.data("current-page"),
                callback: function (event, pg) {
                    var $li = $(event.target).closest("li");
                    if ($li.not(".disabled").length > 0 && $li.not(".active").length > 0) {
                        $pagination.show();
                        RefreshAvailableItems($parent, pg, PageSize);
                    }
                    event.preventDefault();
                }
            })
            .show();
    }


    //Create Print Button By: Artum Ramos
    $printBtn = DMS.Helpers.CreateAnchorButton("btn-primary btn", '', ' PRINT', DMS.Helpers.CreateFontAwesomeIcon('fa-print'));
    $printBtn.click(function (evt) {
        if (Page_ClientValidate("")) {
            var recordId = getQueryVariable('id');
            var protocol = window.location.protocol;
            var host = window.location.host;
            var url = protocol + '//' + host + '/report/?reportname={3E749607-955F-E611-80DB-00155D010E2C}&reportid=' + recordId;
            window.open(url, 'blank', 'width=1200,height=850');
            event.preventDefault();
        }
    });
    DMS.Helpers.AppendButtonToToolbar($printBtn);

    //Custom Buttons
    function drawAllocateButton() {

        var allocateButton = document.createElement("BUTTON");
        var allocate = document.createElement("SPAN");
        allocate.className = "fa fa-plus";
        allocateButton.appendChild(allocate);
        allocateButton.style = "margin-left:5px";
        var allocateButtonLabel = document.createTextNode(" TRANSFER VEHICLE");
        allocateButton.appendChild(allocateButtonLabel);

        /*transferStatus == Unposted*/
        //if(transferStatus == 100000001 || $('.record-status').html() == "")
        //allocateButton.className = "allocate-link btn btn-primary action";
        //else
        allocateButton.className = "allocate-link btn btn-primary action disabled";
        allocateButton.addEventListener("click", AllocateVehicle);
        $("#Inventory").find(".view-toolbar.grid-actions.clearfix").append(allocateButton);
    }

    function drawPostButton() {
        var postButton = document.createElement("BUTTON");
        var post = document.createElement("SPAN");
        post.className = "fa fa-thumb-tack";
        postButton.appendChild(post);
        var postButtonLabel = document.createTextNode(" POST");
        postButton.className = "post btn btn-primary permanent-disabled disabled";
        postButton.appendChild(postButtonLabel);
        postButton.addEventListener("click", postTransaction);

        DMS.Helpers.AppendButtonToToolbar(postButton);
    }

    //Functions
    function postTransaction() {
        showLoading();
        $('#gsc_transferstatus').val('100000000')
        $("#UpdateButton").click();
    }

    function AllocateVehicle(event) {
        var count = 0;
        var id = "";

        $('#Inventory tbody tr td.multi-select-cbx').each(function () {
            if ($(this).data('checked') == "true") {
                count += 1;
                id = $(this).closest('tr').data('id');
            }
        });

        if (count == 1) {
            showLoading();

            $("#gsc_inventoryidtoallocate").val(id);
            $("#UpdateButton").click();
        }
        else {
            DMS.Notification.Error(" You can only allocate one vehicle per transaction.", true, 5000);
        }
        //event.preventDefault();
    }

    //set fields to readonly
    function disableFields() {
        $('#UpdateButton').addClass('permanent-disabled disabled');
        $('.delete-link').addClass('permanent-disabled disabled');
        $('.control > input').attr('readOnly', true);
        $('.control > textarea').attr('readOnly', true);
        $('.datetimepicker > .form-control').attr('readOnly', true);
        $('.add-margin-right').addClass('permanent-disabled disabled');

        $('.clearlookupfield').remove();
        $('.launchentitylookup').remove();
        $('.input-group-addon').remove();

        $('#gsc_transferstatus').hide();
        $('#gsc_transferstatus_label').hide();
    }

    function getQueryVariable(variable) {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (pair[0] == variable) {
                return pair[1];
            }
        }
    }

    //Cancel
    function createCancelButton() {
        var cancelIcon = DMS.Helpers.CreateFontAwesomeIcon('fa-ban');
        var cancelBtn = DMS.Helpers.CreateButton('button', 'btn btn-primary cancel', '', ' CANCEL', cancelIcon);
        var cancelConfirmation = DMS.Helpers.CreateModalConfirmation({ id: 'cancelModal', headerIcon: 'fa fa-ban', headerTitle: ' Cancel ', Body: 'Are you sure you want to canncel vehicle sransfer?' });
        $(".crmEntityFormView").append(cancelConfirmation);
        cancelBtn.on('click', function (evt) {
            cancelConfirmation.find('.confirmModal').on('click', function () {
                $('#gsc_transferstatus').val('100000002');
                $("#UpdateButton").click();
                cancelConfirmation.modal('hide');
                //  showLoading();
            });
            cancelConfirmation.modal('show');
        });

        DMS.Helpers.AppendButtonToToolbar(cancelBtn);
    }

    $(document).on('click', '#Inventory .view-grid table tbody tr', AddEventInventory);

    setTimeout(function () {
        //$('#Inventory .allocate-link').addClass('disabled');
        $('#Inventory .view-grid table tbody td.multi-select-cbx').click(function () {
            AddEventInventory();
        });
    }, 3000);

    function AddEventInventory() {
        var id;
        var counter = 0;

        $('#Inventory tbody tr td.multi-select-cbx').each(function () {
            if ($(this).data('checked') == "true") {
                counter++;
            }
        });

        if (counter > 0) {
            if (transferStatus == 100000001 || $('.record-status').html() == "")
                $('#Inventory .allocate-link').removeClass('disabled');
        } else {
            $('#Inventory .allocate-link').addClass('disabled');
        }
    }

    function preventDefault(event) {
        event.preventDefault();
    }

    function showLoading() {
        $.blockUI({ message: null, overlayCSS: { opacity: .3 } });

        var div = document.createElement("DIV");
        div.className = "view-loading message text-center";
        div.style.cssText = 'position: absolute; top: 50%; left: 50%;margin-right: -50%;display: block;';
        var span = document.createElement("SPAN");
        span.className = "fa fa-2x fa-spinner fa-spin";
        div.appendChild(span);
        $(".content-wrapper").append(div);
    }
    setTimeout(disableTab, 3000);

    function disableTab() {
        $('.disabled').attr("tabindex", "-1");
    }
});