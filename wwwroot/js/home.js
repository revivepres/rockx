// home.js
$(document).ready(function () {
    GuestCount.init(0);
    SelectedDate.init();
});

// EVENT HANDLERS
// -----------------------------------------------
$("#btnPageup").click(function (event) {
    event.preventDefault();
    $('html,body').animate({ scrollTop: $("#top").offset().top }, 'slow');
});

$("#btnPagedown").click(function (event) {
    event.preventDefault();
    $('html,body').animate({ scrollTop: $("#bottom").offset().top }, 'slow');
});

$("#btnGuestinc").click(function (event) {
    GuestCount.add();
});

$("#btnGuestdec").click(function (event) {
    GuestCount.minus();
});

$("#btnSave").click(function (event) {
    var date = SelectedDate.get();
    if (date === "") {
        alert("Date is required!");
        event.stopPropagation();
        return;
    }
    var total = $("input[type='checkbox']:checked").length / 2;
    total += GuestCount.get();
    $("#totalAttendees").text(total);
    $("#alreadyRecorded").hide();
    $.ajax({
        url: "api/HasRecords/" + date,
        type: "get",
        dataType: "json",
        success: function (recorded, textStatus, jqXHR) {
            if (recorded == true) {
                $("#alreadyRecorded").show();
            }
        },
        error: function (msg) {
            alert("ERROR " + msg.statusText);
        }
    });
});

$("input[type='checkbox']").change(function (event) {
    var id = event.target.id;
    $("#person_" + id).prop("checked", !$("#person_" + id).prop("checked"));
});

var GuestCount = (function () {
    var count = 0;
    function draw() {
        $("#guestCount").html(count);
        $("#guestCountInput").val(count);
    }
    return {
        init: function (value) {
            if (value) {
                count = value;
            }
            draw();
        },
        add: function () {
            count++;
            draw();
        },
        minus: function () {
            if (count > 0) {
                count--;
            }
            draw();
        },
        get: function () {
            return count;
        }
    }
})();

var SelectedDate = (function () {
    var now = new Date();
    var day = ("0" + now.getDate()).slice(-2);
    var month = ("0" + (now.getMonth() + 1)).slice(-2);
    var todaydate = now.getFullYear() + "-" + (month) + "-" + (day);
    function draw(date) {
        $("#selectedDate").val(date);
    }
    return {
        init: function () {
            draw(todaydate);
        },
        set: function (value) {
            draw(value);
        },
        get: function () {
            return $("#selectedDate").val();
        },
        today: function () {
            return todaydate;
        }
    }
})();