$(window).click(function (e) {
    var clickedElement = e.target;
    if ($(clickedElement).hasClass("top-nav-dropdown-right") == false && $(clickedElement).hasClass("is-selected") == false) {
        if ($(".top-nav-dropdown-right.is-selected").length) {
            $(".top-nav-dropdown-right.is-selected").siblings(".top-sub-nav-relative").toggleClass("is-hidden");
            $(".top-nav-dropdown-right.is-selected").toggleClass("is-selected");
        }
    }
});

$(document).ready(function () {
    $(".top-nav-dropdown-right").click(function () {
        $(this).toggleClass("is-selected");
        $(this).siblings(".top-sub-nav-relative").toggleClass("is-hidden");
    });
});

function ToggleKeyUsed(id, caller, callerCompanionId)
{
    if ($(caller).hasClass('ajax-awaiting') == false && $(caller).hasClass('awaiting-reveal') == false) {
        $(caller).toggleClass('ajax-awaiting');
        $.ajax({
            type: "POST",
            url: '/Account/AjaxMarkKeyUsed',
            data: { productKeyId: id },
            datatype: "html",
            success: function (data) {
                $(callerCompanionId).toggleClass("unused-btn");
                $(caller).toggleClass('ajax-awaiting');
                $(callerCompanionId).toggleClass("used-btn");
                if ($(caller).val() === '🔒')
                    $(caller).val('🔓');
                else
                    $(caller).val('🔒');
            }
        });
    }
}

function MarkKeyRevealed(id, caller)
{
    if ($(caller).hasClass('ajax-awaiting') == false) {
        $(caller).toggleClass('ajax-awaiting');
        $.ajax({
            type: "POST",
            url: '/Account/AjaxRevealKey',
            data: { productKeyId: id },
            datatype: "html",
            success: function (data) {
                $(caller).val(data);
                $(caller).toggleClass('ajax-awaiting');
                $(caller).toggleClass("unused-btn");
                $(caller).toggleClass("reveal-btn");
                var id = "#" + $(caller).attr('id') + "toggle";
                document.getElementById($(caller).attr('id')).onclick = null;
                $(id).toggleClass('toggle-used-btn');
                $(id).toggleClass('toggle-used-inactive-btn');
                $(id).toggleClass('awaiting-reveal');
            }
        });
    }
}

function AddToCart(id, caller)
{
    var element = caller;
    //element.setAttribute('disabled', 'disabled');
    if ($(element).hasClass('ajax-awaiting') == false) {
        $(element).toggleClass('ajax-awaiting');
        $.ajax({
            type: "POST",
            url: '/Cart/AjaxAddToCart',
            data: { listingId: id },
            datatype: "html",
            success: function (data) {
                var cartTotal = $('#cartTotal').html();
                $('#cartTotal').html(+cartTotal + +data);
                //element.removeAttribute('disabled');
                $(element).toggleClass('ajax-awaiting');
                alert('Added');
            }
        });
    }
}

function AddToBlacklist(id, rowId)
{
    $.ajax({
        type: "POST",
        url: "/User/AjaxAddToBlacklist",
        data: { listingId: id },
        datatype: "html",
        success: function(data) {
            alert('Added');
            var blacklistVal = $('#PreviousFilterBlacklist').val();
            if (blacklistVal == "True")
            {
                var $div = $('#listingRow' + rowId);
                $div.find('div').fadeOut(1000, function () {
                    $div.remove();
                });
            }
        }
    });
}

function TransferPoints(id, row, myrow)
{
    //TODO: fix it for navbar points (update those correctly) and get and check the reserved balance in the else if clause

    var points = prompt("How many points would you like to transfer to " + $('#userName' + row).html() + "?", "0");
    points = parseInt(points, 10);

    //var reservedBalance = parseInt($('#reservedBalance').text(), 10);
    var currentUserBalance = parseInt($('#userBalance' + myrow).text(), 10);
    var senderBalance = parseInt($('#userBalance' + row).text(), 10);

    if (isNaN(points))
    {
        alert("Please enter a valid number!");
    }
    //else if (points > reservedBalance || points <= 0) {
    else if (points <= 0) {
        alert("Please enter a positive balance less than or equal to your own balance!");
    }
    else {
        $.ajax({
            type: "POST",
            url: "/User/AjaxTransferPoints",
            data: { points: points, userId: id },
            datatype: "html",
            success: function (data) {
                alert(data);
                //$('#reservedBalance').html(reservedBalance - points);
                $('#userBalance' + row).html(senderBalance + points);
                $('#userBalance' + myrow).html(currentUserBalance - points);
                // add javascript code to modify navbar balance, when added
            }
        });
    }
}