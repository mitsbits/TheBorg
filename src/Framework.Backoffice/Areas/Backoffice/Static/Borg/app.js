$(function () {

    $(".server-table").dataTable({
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": false,
        "bInfo": false,
        "bAutoWidth": true
    });

    $('input.datetimepicker').daterangepicker({
        "timePicker": true, "timePickerIncrement": 15, "format": 'DD/MM/YYYY HH:mm', "timePicker24Hour": true,
        "singleDatePicker": true//, startDate: moment().subtract(29, 'days'),
    });
    //$("input[type='checkbox']:not(.simple), input[type='radio']:not(.simple)").iCheck(
    //    {
    //        checkboxClass: 'icheckbox_minimal-aero',
    //        radioClass: 'iradio_minimal-aero'
    //    }
    //);
    //$("select.select2").select2();

    //$('input.datetimepicker').daterangepicker({
    //    "timePicker": true, "timePickerIncrement": 5, "format": 'DD/MM/YYYY HH:mm', "timePicker24Hour": true,
    //    "singleDatePicker": true,//, startDate: moment().subtract(29, 'days'),
    //});
    //$('input.datepickernullable').daterangepicker({

    //    "singleDatePicker": true, "format": 'DD/MM/YYYY'//, startDate: moment().subtract(29, 'days'),
    //});

    //$('.pull-down').each(function () {
    //    var $this = $(this);
    //    $this.css('margin-top', $this.parent().height() - $this.height());
    //});

    //var serverdatetime = new Date($('input#loadclock').val());
    //startTime();

    //function startTime() {
    //    var today = serverdatetime;
    //    var h = today.getHours();
    //    var m = today.getMinutes();
    //    var s = today.getSeconds();

    //    m = checkTime(m);
    //    s = checkTime(s);
    //    $('span#clock').html(h + ":" + m + ":" + s);
    //    serverdatetime = new Date(serverdatetime.getTime() + 1000);
    //    var t = setTimeout(startTime, 1000);
    //}
    //function checkTime(i) {
    //    if (i < 10) { i = "0" + i };  // add zero in front of numbers < 10
    //    return i;
    //}
});