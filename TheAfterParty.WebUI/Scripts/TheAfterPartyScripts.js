﻿function AddToCart(id)
{
    $.ajax({
        type: "POST",   
        url: '/Cart/AjaxAddToCart',
        data: { listingId: id },
        datatype: "html",
        success: function(data) {
            $('#cartInfo').html(data);
            alert('Added');
        }
    });
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
    var points = prompt("How many points would you like to transfer to " + $('#userName' + row).html() + "?", "0");
    points = parseInt(points, 10);

    var reservedBalance = parseInt($('#reservedBalance').text(), 10);
    var cartBalance = parseInt($('#cartBalance').text(), 10);
    var currentUserBalance = parseInt($('#currentUserBalance').text(), 10);
    var senderBalance = parseInt($('#userBalance' + row).text(), 10);

    if (isNaN(points))
    {
        alert("Please enter a valid number!");
    }
    else if (points > reservedBalance || points <= 0) {
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
                $('#reservedBalance').html(reservedBalance - points);
                $('#userBalance' + row).html(senderBalance + points);
                $('#userBalance' + myrow).html(currentUserBalance - points);
                $('#currentUserBalance').html(currentUserBalance - points);
                $('#cartBalance').html(cartBalance - points);
                // add javascript code to modify navbar balance, when added
            }
        });
    }
}