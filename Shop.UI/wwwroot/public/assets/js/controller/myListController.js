var returnedListItems = new Array();
var renderLists = new Array();
$("document").ready(function(){
	refreshMyList();
});

function refreshMyList()
{
	var url = cloudCodeURL + "getMyLists";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"val":"' + "val1" + '",'
			 + '"currentUser":"' + currentUserID + '",'
			 + '"val1":"' + "val1" + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			if (httpResponse.result.status == "success")
			{
				var lists = httpResponse.result.lists;
				returnedListItems = lists;
				renderLists = httpResponse.result.renderLists;
				var html = "";
				for (var i = 0; i < lists.length; i++)
				{
				    html += "    <div onclick='viewList(\"" + i + "\",\"" + lists[i].objectId + "\")' id='myListBlock" + lists[i].objectId + "' class='col-xs-6' style='background-color:#0bdd69;height:80px;padding:15px;'>";
			        html += '      <div class="row" style="color:white;padding-left:15px;padding-top:3px;padding-right:10px;">';
			        html += "      <div onclick='deleteList(\"" + lists[i].objectId + "\")' style='float:right;color:#0bdd69;background-color:white;width: 15px;text-align: center;font-size: 12px; cursor: pointer;' >x</div>";
			        html += '          List ' + (i + 1) + '-<span style="color:black">' + lists[i].name + '</span>-R' + lists[i].total + '';
			        html += '      </div> ';
			        html += '      <div class="row" style="color:white;font-size:20px;padding-left:15px;">';
			        html +=   			lists[i].name;
			        html += '      </div>';
			        html += '     </div>';
				}				

				$("#lists").html(html);

			}
		},
		error: function (httpResponse)
		{
			console.log(httpResponse);
		}
	});
}

//bad naming conventions sorry but dont have time to refactor TODO
function viewList(listID, listObjectId)
{
	$("#searchTable").slideUp();
	$("#currentListObjectId").val(listObjectId);
	$("#addAllToCart").attr("onclick", "addAllToCart('" + listObjectId + "')");

	var listItem = returnedListItems[parseInt(listID)];

	$("#viewListName").html(listItem.name);

	$("#viewListAmount").html('List ' + (listID + 1) + '-<span style="color:black">' + listItem.products.length + ' items</span>-R' + listItem.total + '0');
	$("#viewAList").slideDown();

	var products = listItem.products;
	var html = "";
	$("#viewAListTableBody").html('');
	for (var i = 0; i < products.length; i++)
	{
		if(products[i].published != false)
		{
			html += '    <tr class="specialRow myListRow" id="myListRow' + products[i].objectId + '" style="outline: 4px solid white">';
	        html += '        <td style="background-color:white;vertical-align:middle;" data-title="iamge">';
	        html += '            <img class="smallImage" src="' + products[i].imageUrl + '">';
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="vertical-align:middle;padding-left:10px;" data-title="PRODUCT #">';
	        html += '            <label style="margin-bottom: 0px;">' + products[i].name + '</label>';
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="vertical-align:middle;padding-left:10px;" data-title="CODE">';
	        html += '            <label style="margin-bottom: 0px;">' + products[i].externalId + '</label>';
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="UOM">';
	        html += '            <label style="margin-bottom: 0px;>' + products[i].unit + '</label>';
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="vertical-align:middle;padding-left:10px;" data-title="PRICE">';
	        html += '            <label style="margin-bottom: 0px;" id="productPriceView' + listObjectId + products[i].objectId +'">R' + products[i].price.toFixed(2) + '</label>';
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="vertical-align:middle;padding-left:10px;" data-title="QUANTITY">';
	        html += "           <input id='totalAmountInput" + listObjectId + products[i].objectId +"' onclick='updateMyListQuantity(\"" + products[i].objectId + "\",\"" + listObjectId + "\")' value='1' min='1' class='totalAmountInputmyListRow" + products[i].objectId + " AmountInputmyListRow" + listObjectId + products[i].objectId + " mod productAmountBlockInput' type='number' name='Qty' style='width: 50px; margin-top: 0px;margin-bottom: 0px;height: 30px;'>";
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="vertical-align:middle;padding-left:10px;" data-title="TOTAL PRICE">';
	        html += '            <label class="productPriceBlock" id="totalValue' + listObjectId + products[i].objectId +'">R' + products[i].price.toFixed(2) + '</label>';
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="vertical-align:middle;padding-left:10px;">';
	        html += "            <label style='margin-bottom: 0px;' onclick='addProductToCartFromList(\"" + products[i].objectId + "\",\"" + listObjectId + "\")' class='addToCartLabel'>Add to cart</label>";
	        html += '        </td>';
	        html += '        <td class="tableDiv" style="vertical-align:middle;padding-left:10px;" >';
	        html += "            <button onclick='removeFromMyList(\"" + (i) + products[i].objectId + "\",\"" + products[i].objectId + "\",\"" + listObjectId + "\")' style='background-color:grey;color:white;border: none;border-radius: 3px;'>x</button>";
	        html += '        </td>';
	        html += '    </tr>';
	        $("#viewAListTableBody").append(html);
		}		
        html = '';


        $('#totalAmountInput' + (i) + products[i].objectId).change(function(){
        	updateOrderSummary();

        	var id = this.id.replace("totalAmountInput", "");
        	$('#totalValue' + id).html("R" + (parseInt($("#productPriceView" + id).html().replace("R", "")) * parseInt($(this).val())));
        })

        updateOrderSummary();
	}
}

function updateMyListQuantity(productId, listId)
{
	var totalItems = $("#totalAmountInput" + listId + productId).val();
	var currentItemTotal = parseFloat($("#productPriceView" + listId + productId).html().replace("R", ""));

	$("#totalValue" + listId + productId).html("R" + (currentItemTotal * totalItems).toFixed(2));
	updateOrderSummary();
}

function addAllToCart(listObjectId)
{
	var products = new Array();

	$( ".myListRow" ).each(function( index ) {
		var classUsed = "totalAmountInput" + this.id;
		var productID = classUsed.replace("totalAmountInputmyListRow", "");
		var amount = $(".totalAmountInput" + this.id).val();

		for (var i = 0; i < amount; i++)
		{
			products.push(productID);
		};
	});

	var url = cloudCodeURL + "addAllToCart";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"products":"' + products + '",'
		 	+ '"currentUser":"' + currentUserID + '",'
			+ '"val":"' + "val" + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			if (httpResponse.result.status == "success")
			{
				getCartCount();
			}
		},
		error: function (httpResponse)
		{
			console.log(httpResponse);
		}
	});
}

function removeFromMyList(rowId, productId, listId)
{
	$("#myListRow" + rowId).hide();

	var url = cloudCodeURL + "removeProductMyList";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"productId":"' + productId + '",'
		 	+ '"currentUser":"' + currentUserID + '",'
			+ '"listId":"' + listId + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			if (httpResponse.result.status == "success")
			{
				getCartCount();
			}
		},
		error: function (httpResponse)
		{
			console.log(httpResponse);
		}
	});
}

function addProductToCartFromList(productId, index)
{
	var amount = parseInt($("#totalAmountInput" + index + productId).val());

	var url = cloudCodeURL + "createOrUpdateCartMultipleProducts";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"productId":"' + productId + '",'
		 	+ '"currentUser":"' + currentUserID + '",'
			+ '"amount":"' + amount + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			if (httpResponse.result.status == "success")
			{
				getCartCount();
			}
		},
		error: function (httpResponse)
		{
			console.log(httpResponse);
		}
	});
}

function deleteList(listID)
{
	$("#deleteModal").modal("show");
	$("#confirmDelete").attr("onclick", "confirmDeleteList(\'" + listID + "\')");
}

function confirmDeleteList(listID)
{
	$("#deleteModal").modal("hide");
	$("#viewAList").hide();
	$("#myListBlock" + listID).hide();
	var url = cloudCodeURL + "deleteMyList";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"val":"' + "val1" + '",'
			 + '"listId":"' + listID + '",'
			 + '"val1":"' + "val1" + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			if (httpResponse.result.status == "success")
			{
				getCartCount();
			}
		},
		error: function (httpResponse)
		{
			console.log(httpResponse);
		}
	});
}

function toggleCreateList()
{
	$("#addMyListContainer").toggleClass("hidden");

}

function createMyList()
{
	var listName = $("#listNameInput").val();
	if(listName.length > 0)
	{
		var url = cloudCodeURL + "createMyList";
		$.ajax({
			type: "POST",
			headers: headers,
			url: url,
			data: '{"val":"' + "val1" + '",'
				 + '"listName":"' + listName + '",'
				 + '"currentUser":"' + currentUserID + '",'
				 + '"val1":"' + "val1" + '"}',
			contentType: "application/jsonp",
			success: function (httpResponse)
			{
				if (httpResponse.result.status == "success")
				{
					$("#addMyListContainer").toggleClass("hidden");
					$("#listNameInput").val("");
					refreshMyList();
				}
			},
			error: function (httpResponse)
			{
				console.log(httpResponse);
			}
		});
	}
}

function updateOrderSummary()
{
	//logic needed for vat
	var vat = 0;
	var totalProducts = 0
	var subTotal = 0;
	var grandTotal = 0;
	var delivery = 0;
	var deliveryText = "FREE";

	$( ".productPriceBlock" ).each(function( index ) {
	  	subTotal += parseInt($( this ).html().replace("R", ""))
	  	grandTotal = (subTotal + vat + delivery);
	});

	$( ".productAmountBlockInput" ).each(function( index ) {
		totalProducts += parseInt($(this).val())
	});

    $("#summaryTotalProducts").html(totalProducts);
    $("#summaryDeliveryText").html(deliveryText);
    $("#summarySubTotal").html("R" + subTotal);
    $("#summaryVat").html("R" + vat);
    $("#summaryOrderFrandTotal").html("R" + grandTotal);

}

function searchProducts()
{
	var searchText = $("#searchInput").val();

	var url = cloudCodeURL + "searchProducts";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"val":"' + "val1" + '",'
			 + '"searchText":"' + searchText + '",'
			 + '"currentUser":"' + currentUserID + '",'
			 + '"val1":"' + "val1" + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			if (httpResponse.result.status == "success")
			{
				$("#searchTable").slideDown();
				var products = httpResponse.result.products;
				$("#searchTableBody").html('');
				var html = "";
				for (var i = 0; i < products.length; i++)
				{
					html += '    <tr class="searchedListRow" id="myListRow' + (i) + products[i].objectId + '" style="outline: 4px solid white">';
			        html += '        <td style="background-color:white;vertical-align:middle;padding-left:10px;" data-title="iamge">';
			        html += '            <img class="smallImage" src="http://thoughtfaqtory.com/officebox/images/product_images/'+products[i].externalId+'.jpg">';
			        html += '        </td>';
			        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="PRODUCT #">';
			        html += '            <label>' + products[i].name + '</label>';
			        html += '        </td>';
			        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="CODE">';
			        html += '            <label>' + products[i].externalId + '</label>';
			        html += '        </td>';
			        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="UOM">';
			        html += '            <label>' + products[i].unit + '</label>';
			        html += '        </td>';
			        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="PRICE">';
			        html += '            <label id="productPriceView' + (i) + products[i].objectId +'">R' + products[i].price + '</label>';
			        html += '        </td>';
			        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;">';
			        html += "            <label onclick='addProductToList(\"" + products[i].objectId + "\")' class='addToCartLabel'>Add to list</label>";
			        html += '        </td>';
			        html += '    </tr>';

			        $("#searchTableBody").append(html);
			        html = '';
				}
			}
			$("#searchTable").slideDown();
		},
		error: function (httpResponse)
		{
			console.log(httpResponse);
		}
	});
}

function addProductToList(productId)
{
	var searchText = $("#searchInput").val();

	var existingMyListRows = $(".myListRow").length;

	var url = cloudCodeURL + "addSearchedProductToList";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"val":"' + "val1" + '",'
			 + '"productId":"' + productId + '",'
			 + '"listId":"' + $("#currentListObjectId").val() + '",'
			 + '"val1":"' + "val1" + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			if (httpResponse.result.status == "success")
			{
				var html = "";
				var product = httpResponse.result.product;

				html += '    <tr class="myListRow" id="myListRow' + (existingMyListRows) + product.objectId + '" style="outline: 4px solid white">';
		        html += '        <td style="background-color:white;vertical-align:middle;padding-left:10px;" data-title="iamge">';
		        html += '            <img class="smallImage" src="http://thoughtfaqtory.com/officebox/images/product_images/'+product.externalId+'.jpg">';
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="PRODUCT #">';
		        html += '            <label>' + product.name + '</label>';
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="CODE">';
		        html += '            <label>' + product.externalId + '</label>';
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="UOM">';
		        html += '            <label>' + product.unit + '</label>';
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="PRICE">';
		        html += '            <label id="productPriceView' + (existingMyListRows) + product.objectId +'">R' + product.price + '</label>';
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="QUANTITY">';
		        html += '            <div class="number-wrapper">';
		        html += '                <input id="totalAmountInput' + (existingMyListRows) + product.objectId +'" value="1" class="AmountInputmyListRow' + (existingMyListRows) + product.objectId + ' mod productAmountBlockInput" type="number" name="Qty" style=" width: 50px; margin-top:10px;">';
		        html += '            </div>';
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" data-title="TOTAL PRICE">';
		        html += '            <label class="productPriceBlock" id="totalValue' + (existingMyListRows) + product.objectId +'">R' + product.price + '</label>';
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;">';
		        html += "            <label onclick='addProductToCard(\"" + product.objectId + "\",\"" + existingMyListRows + "\")' class='addToCartLabel'>Add to cart</label>";
		        html += '        </td>';
		        html += '        <td style="background-color:#EFEEEC;vertical-align:middle;padding-left:10px;" >';
		        html += "            <label onclick='removeFromMyList(\"" + (existingMyListRows) + product.objectId + "\",\"" + product.objectId + "\",\"" + $("#currentListObjectId").val() + "\")' style='background-color:grey;color:white;vertical-align:middle;'>x</label>";
		        html += '        </td>';
		        html += '    </tr>';

		        $("#viewAListTableBody").append(html);
		        html = '';


		        $('#totalAmountInput' + (existingMyListRows) + product.objectId).change(function(){
		        	updateOrderSummary();

		        	var id = this.id.replace("totalAmountInput", "");

		        	$('#totalValue' + id).html("R" + (parseInt($("#productPriceView" + id).html().replace("R", "")) * parseInt($(this).val())));
		        })

		        updateOrderSummary();
			}
		},
		error: function (httpResponse)
		{
			console.log(httpResponse);
		}
	});
}
