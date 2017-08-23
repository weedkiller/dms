﻿// date time

$(document).on("createFilter", function (event, filters) {
    $.each(filters, function (x, filter) {
        $.each(filter, function (y, value) {
    var datePickerTemplate = '<ul class="list-unstyled" role="presentation"> \
                                    <li class="entitylist-filter-option-group"> \
                                        <label class="entitylist-filter-option-group-label h4" for="dropdownfilter_0"> \
                                            <span class="sr-only">Filter: </span> \
                                            ' + filter[1] + ' From \
                                        </label> \
                                            <ul class="list-unstyled" role="presentation"> \
                                                <li class="entitylist-filter-option"> \
                                                    <div class="input-group entitylist-filter-option-text"> \
                                                        <div class="input-group date" id="dateFrom' + x + '"> \
                                                            <input type="text" id="dateFromValue' + x + '" class="form-control" placeholder="From"/> \
                                                            <span class="input-group-addon"> \
                                                                <span class="glyphicon glyphicon-calendar"></span> \
                                                            </span> \
                                                        </div> \
                                                     </div>\
                                                </li>\
                                            </ul>\
                                     </li>\
                                     <li class="entitylist-filter-option-group"> \
                                        <label class="entitylist-filter-option-group-label h4" for="dropdownfilter_0"> \
                                            <span class="sr-only">Filter: </span> \
                                            ' + filter[1] + ' To \
                                        </label> \
                                            <ul class="list-unstyled" role="presentation"> \
                                                <li class="entitylist-filter-option"> \
                                                    <div class="input-group entitylist-filter-option-text"> \
                                                        <div class="input-group date" id="dateTo' + x + '"> \
                                                                 <input type="text" id="dateToValue' + x + '" class="form-control" placeholder="To"/> \
                                                                <span class="input-group-addon"> \
                                                                    <span class="glyphicon glyphicon-calendar"></span> \
                                                                </span> \
                                                        </div> \
                                                     </div>\
                                                </li>\
                                            </ul>\
                                     </li>\
                                </ul>';

    $(datePickerTemplate).insertBefore($('#EntityListFilterControl .panel-body .pull-right'));
    
            return false;
        });
    });

    $(function () {
        $.each(filters, function (x, filter) {
            $.each(filter, function (y, value) {
                $('#dateFrom' + x).datetimepicker({
                    pickTime: false,
                    autoclose: true
                });

                $('#dateTo' + x).datetimepicker({
                    autoclose: true,
                    pickTime: false,
                    useCurrent: false //Important! See issue #1075
                });

                $("#dateFrom" + x).on("dp.change", function (e) {
                    $('#dateTo' + x).data("DateTimePicker").setMinDate(e.date);
                });
                $("#dateTo" + x).on("dp.change", function (e) {
                    $('#dateFrom' + x).data("DateTimePicker").setMaxDate(e.date);
                });

                $("#dateToValue" + x).on("change", function (e) {
                    $('#dateFrom' + x).data("DateTimePicker").setMaxDate(new Date(9999,12,31));
                });

                $("#dateFromValue" + x).on("change", function (e) {
                    $('#dateTo' + x).data("DateTimePicker").setMinDate(new Date(1753,01,01));
                });

                $('#dateFromValue' + x).mask('00/00/0000');
                $('#dateToValue' + x).mask('00/00/0000');

                if (value !== 'undefined') {
                    $('#dateFromValue'+ x).attr('data-entityfield', value);
                 }
                return false;
            });
        });
    });
});

$(document).on("createStateCodeFilter", function (event, entity) {
    var statecode = [];

    switch(entity)
    {
        case 'lead':
            statecode.push({"value": 0, "label": 'Open'});
            statecode.push({"value": 1, "label": 'Qualified' });
            statecode.push({"value": 2, "label": 'Disqualified' });
            break;
        case 'opportunity':
            statecode.push({"value": 0, "label": 'Open' });
            statecode.push({"value": 1, "label": 'Won'});
            statecode.push({"value": 2, "label": 'Lost'});
            break;
        case 'quote':
            statecode.push({"value": 0, "label": 'Draft'});
            statecode.push({"value": 1, "label": 'Active'});
            statecode.push({"value": 2, "label": 'Won'});
            statecode.push({"value": 3, "label": 'Closed'});
            break;
         default:
             break;
    }

    var stateCodeOptionSet = '<ul class="list-unstyled" role="presentation"> \
                                    <li class="entitylist-filter-option-group"> \
                                        <label class="entitylist-filter-option-group-label h4" for="satecode"> \
                                            <span class="sr-only">Filter: </span> Status\
                                        </label> \
                                            <ul class="list-unstyled" role="presentation"> \
                                                <li class="entitylist-filter-option"> \
                                                    <div class="input-group entitylist-filter-option-text"> \
                                                        <span class="input-group-addon"><span class="fa fa-filter" aria-hidden="true"></span></span> \
                                                       <select class="form-control" id="satecode"> \
                                                            <option value="" label="All"> </option> \
                                                        </select>\
                                                     </div>\
                                                </li>\
                                            </ul>\
                                     </li>\
                                </ul>';


    $(stateCodeOptionSet).insertBefore($('#EntityListFilterControl .panel-body .pull-right'));
    var stateCodeSelect = $("#satecode").find('option').end();
    $.each(statecode, function (x, status) {
        console.log(status.value);
        console.log(status.label);
        stateCodeSelect.append("<option value='{" + status.value + "}' label='" + status.label + "'>" + status.label + "</option>");
    });


});
