$("document").ready(function (){
	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent) ) 
	{
		$('.selectpicker').selectpicker('mobile');
	} 
	else 
	{
		$('.selectpicker').selectpicker(); 
	}
});

function showEditProduct(productID)
{
	if($("#edit" + productID).is(":visible"))
	{
		$("#edit" + productID).hide();
		$("#" + productID).fadeIn();
	}
	else
	{
		$("#" + productID).hide();
		$("#edit" + productID).fadeIn();
	}
}

function updateProduct (productID)
{
	var valid = true;
	var name = $("#name" + productID).val();
	if(name.length == 0)
	{
		$("#name" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#name" + productID).removeClass("input-error");
	}
	var price = $("#price" + productID).val();
	if(price.length == 0)
	{
		$("#price" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#price" + productID).removeClass("input-error");
	}
	var unit = $("#unit" + productID).val();
	if(unit.length == 0)
	{
		$("#unit" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#unit" + productID).removeClass("input-error");
	}
	var platformEntityId = $("#platformEntityId" + productID).val();
	if(platformEntityId.length == 0)
	{
		$("#platformEntityId" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#platformEntityId" + productID).removeClass("input-error");
	}
	var externalId = $("#externalId" + productID).val();
	if(externalId.length == 0)
	{
		$("#externalId" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#externalId" + productID).removeClass("input-error");
	}
	var colour = $("#colour" + productID).val();
	if(colour.length == 0)
	{
		$("#colour" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#colour" + productID).removeClass("input-error");
	}
	var published = $("#published" + productID).val();
	if(published.length == 0)
	{
		$("#published" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#published" + productID).removeClass("input-error");
	}
	var costPrice = $("#costPrice" + productID).val();
	if(costPrice.length == 0)
	{
		$("#costPrice" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#costPrice" + productID).removeClass("input-error");
	}
	var supplier = $("#supplier" + productID).val();
	if(supplier.length == 0)
	{
		$("#supplier" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#supplier" + productID).removeClass("input-error");
	}
	var brand = $("#brand" + productID).val();
	if(brand.length == 0)
	{
		$("#brand" + productID).addClass("input-error");
		valid = false;
	}
	else
	{
		$("#brand" + productID).removeClass("input-error");
	}

	if(valid)
	{
		var url = cloudCodeURL + "updateProduct";
		$.ajax({
			type: "POST",
			headers: headers,
			url: url,
			data: '{"productId":"' + productID + '",'
				 + '"name":"' + name + '",'
				 + '"unit":"' + unit + '",'
				 + '"brand":"' + brand + '",'
				 + '"supplier":"' + supplier + '",'
				 + '"costPrice":"' + costPrice + '",'
				 + '"platformEntityId":"' + platformEntityId + '",'
				 + '"externalId":"' + externalId + '",'
				 + '"colour":"' + colour + '",'
				 + '"published":"' + published + '",'
				 + '"subCategory":"' + $("#subCategory" + productID).val() + '",'
				 + '"price":"' + price + '"}',
			contentType: "application/jsonp",
			success: function (httpResponse)
			{
				if (httpResponse.result.status == "success")
				{
					window.location.href = "/cms/products";
				}
			},
			error: function (httpResponse)
			{
				console.log(httpResponse);
			}
		});
	}
}