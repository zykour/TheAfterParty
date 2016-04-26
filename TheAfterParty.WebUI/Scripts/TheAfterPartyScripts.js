$(document).ready(function () {
    
    $("ul.subnav").parent().append("<span></span>"); //Only shows drop down trigger when js is enabled (Adds empty span tag after ul.subnav*)

    $("ul.topnav li span").click(function () { //When trigger is clicked...

        //Following events are applied to the subnav itself (moving subnav up and down)
        $(this).parent().find("ul.subnav").slideDown('fast').show(); //Drop down the subnav on click

        $(this).parent().hover(function () {
        }, function () {
            $(this).parent().find("ul.subnav").slideUp('slow'); //When the mouse hovers out of the subnav, move it back up
        });

        //Following events are applied to the trigger (Hover events for the trigger)
    }).hover(function () {
        $(this).addClass("subhover"); //On hover over, add class "subhover"
    }, function () {	//On Hover Out
        $(this).removeClass("subhover"); //On hover out, remove class "subhover"
    });

});

function AddToCart(id)
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