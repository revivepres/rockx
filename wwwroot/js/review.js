// review.js
$(document).ready(function () {

});

// EVENT HANDLERS
// -----------------------------------------------
$("#dateSelection").on("change", function (event) {
    $("#loader").show();
    var total = 0;
    var date = $(this).val().trim();
    $.ajax({
        url: "api/GetRecords/" + date,
        type: "get",
        dataType: "json",
        success: function (data, textStatus, jqXHR) {
            $("#loader").hide();
            var html = `<tbody>
            <tr>
                <td>
                    <i class="fas fa-user-slash"></i>Guests
                </td>
                <td class="text-right">
                    <div class="custom-control">
                        ${data.guestCount}
                        &nbsp;
                    </div>
                </td>
            </tr>`;
            total += data.guestCount;
            $.each(data.people, function (index, person) {
                var checked = "";
                if (person.isAttend) {
                    total++;
                    checked = "checked='checked'";
                }
                html += `
                <tr>
                    <td>
                        <i class="fas fa-user-check"></i> ${person.firstName} ${person.lastName}
                    </td>
                    <td class="text-right">
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" disabled
                                   id="${person.id}" ${checked}">
                            <label class="custom-control-label" for="${person.id}">&nbsp;</label>
                        </div>
                    </td>
                </tr>`;
            });
            html += `
            <tr>
                <td>
                    <strong>Total Attendees</strong>
                </td>
                <td class="text-right">
                    ${total}
                </td>
            </tr></tbody>`;
            $("#peopleTable").find('tbody').replaceWith(html);
        },
        error: function (msg) {
            $("#loader").hide();
            alert("ERROR " + msg.statusText);
        }
    });
});