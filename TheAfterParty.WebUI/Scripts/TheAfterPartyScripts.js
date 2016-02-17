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
        tpe: "POST",
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