$(window).click(function (e) {
    if (e.which != 1)
    {
        return;
    }
    var clickedElement = e.target;
    if ($(clickedElement).hasClass("top-nav-dropdown-right") == false && $(clickedElement).hasClass("is-selected") == false) {
        if ($(".top-nav-dropdown-right.is-selected").length) {
            $(".top-nav-dropdown-right.is-selected").siblings(".top-sub-nav-relative").toggleClass("is-hidden");
            $(".top-nav-dropdown-right.is-selected").toggleClass("is-selected");
        }
    }
});

$(document).ready(function () {
    // add clickable dropdowns if present
    $(".top-nav-dropdown-right").click(function () {
        if ($(this).hasClass("is-selected") == false)
        {
            if ($(".top-nav-dropdown-right.is-selected").length) {
                $(".top-nav-dropdown-right.is-selected").siblings(".top-sub-nav-relative").toggleClass("is-hidden");
                $(".top-nav-dropdown-right.is-selected").toggleClass("is-selected");
            }
        }
        $(this).toggleClass("is-selected");
        $(this).siblings(".top-sub-nav-relative").toggleClass("is-hidden");
    });

    // add empty cart confirmation
    $('.empty-confirm').on('click', function () {
        return confirm('Are you sure you want to empty your cart?');
    });
    // add purchase confirmation
    $('.purchase-confirm').on('click', function () {
        return confirm('Are you sure you want to purchase these items?');
    });

    //fix footer height
    FixFooterHeight();
});

$(window).on("load", function () {
    $('.delayed-load').each(function () {
        $(this).css('background-image', 'url(' + $(this).data('src') + ')');
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

function SubmitBid(id, caller, auctionid, auctionName)
{
    if ($(caller).hasClass('ajax-awaiting') == false) {
        $(caller).toggleClass('ajax-awaiting');

        var bidAmount = $('#' + id + 'val').val();

        var reg = new RegExp('^[0-9]+$');

        if (reg.test(bidAmount) == false) {
            alert("\"" + bidAmount + "\" is not a valid bid. Please enter a valid bid.");
            $(caller).removeClass('ajax-awaiting');
            return false;
        }

        var points = parseInt(bidAmount, 10);

        var confirmed = confirm("Are you sure you want to submit a bid of " + points + " for " + auctionName);

        if (confirmed)
        {
            confirmed = confirm("Are you POSITIVE you want to submit a bid for " + points + " for " + auctionName);
        }

        if (confirmed) {
            $.ajax({
                type: "POST",
                url: '/Auctions/AjaxSubmitBid',
                data: { auctionId: auctionid, bid: points },
                datatype: "html",
                success: function (data) {
                    if ("True" == data) {
                        var element = $('#' + id + "cont");
                        var newSpan = document.createElement("span");
                        newSpan.className = "text-success";
                        newSpan.innerHTML = points + "";
                        alert("Your bid of " + points + " has been submitted. Refresh the page for effects to take place.")
                    }
                    else {
                        alert("Invalid bid. Make sure the bid is large enough or that you have enough points");
                    }
                    $(caller).removeClass('ajax-awaiting');
                }
            });
        }
        else
        {
            $(caller).removeClass('ajax-awaiting');
        }
    }
}

$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})

function ToggleFilters(id)
{
    $('#' + id + "Container").toggleClass("is-hidden");
    $('#' + id + "chev").toggleClass("fa-chevron-down");
    $('#' + id + "chev").toggleClass("fa-chevron-up");

    FixFooterHeight();
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
                var element = document.getElementById($(caller).attr('id'));
                    element.type = 'text';
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

function AddToBlacklist(caller, id, rowId)
{    
    if ($(caller).hasClass('ajax-awaiting') == false) {
        $(caller).toggleClass('ajax-awaiting');

        $.ajax({
            type: "POST",
            url: "/User/AjaxToggleBlacklist",
            data: { listingId: id },
            datatype: "html",
            success: function (data) {
                $(caller).toggleClass("text-inactive");
                $(caller).children(".store-blacklist").toggleClass("store-blacklist-active");
                if (data == "True") {
                    var blacklistVal = $('#PreviousFilterBlacklist').val();
                    if (blacklistVal == "True") {
                        var $div = $('#listingRow' + rowId);
                        $div.find('div').fadeOut(300, function () {
                            $div.remove();
                        });

                        $div = $('#listing' + rowId + 'id1');

                        if ($div != null) {
                            $div.find('div').fadeOut(300, function () {
                                $div.remove();
                            });
                        }
                    }
                }
                $(caller).toggleClass('ajax-awaiting');
            }
        });
    }

    FixFooterHeight();
}

function ToggleHidden(caller)
{
    $(caller).find('.expand-button').toggleClass("fa-chevron-down");
    $(caller).find('.expand-button').toggleClass("fa-chevron-up");
    $('#' + $(caller).attr('id') + '1').toggleClass("is-hidden");
    //.slideToggle(300, function () { $('#' + $(caller).attr('id') + '1')

    FixFooterHeight();
}

function ToggleHiddenAlt(caller) {
    $(caller).find('.expand-button').toggleClass("fa-chevron-down");
    $(caller).find('.expand-button').toggleClass("fa-chevron-right");
    $('#' + $(caller).attr('id') + '1').toggleClass("is-hidden");
    //.slideToggle(300, function () { $('#' + $(caller).attr('id') + '1')

    FixFooterHeight();
}

function TransferPoints(id, row, myrow)
{
    //TODO: fix it for navbar points (update those correctly) and get and check the reserved balance in the else if clause

    var pointsTxt = prompt("How many points would you like to transfer to " + $('#userName' + row).html() + "?", "0");

    if (pointsTxt == null)
    {
        return;
    }

    var reg = new RegExp('^[0-9]+$');

    var points = parseInt(pointsTxt, 10);

    //var reservedBalance = parseInt($('#reservedBalance').text(), 10);
    var currentUserBalance = parseInt($('#userBalance' + myrow).text(), 10);
    var senderBalance = parseInt($('#userBalance' + row).text(), 10);

    if (reg.test(pointsTxt) == false)
    {
        alert("Please enter a valid number!");
    }
    //else if (points > reservedBalance || points <= 0) {
    else if (points <= 0 || points > currentUserBalance) {
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
                $('#accountNavBtn').html("Account (" + (currentUserBalance - points) + " points)");
                // add javascript code to modify navbar balance, when added
            }
        });
    }
}

//courtesy of stackoverflow, floating footer
function FixFooterHeight()
{
    var docHeight = $(window).height();
    var footerHeight = $('#footer').height();
    var footerTop = $('#footer').position().top + footerHeight;

    if (footerTop < docHeight) {
        $('#footer').css('margin-top', (docHeight - footerTop) - 4 + 'px');
    }
    else
    {
        $('#footer').css('margin-top', 'auto');
    }
}