//COST CENTRES
function createNewCostCenter() {
	var valid = true;

	var businessCostCenterName = $("#businessCostCenterName").val();
	if (businessCostCenterName.length == 0) {
		$("#businessCostCenterName").addClass("input-error");
		valid = false;
	}
	else {
		$("#businessCostCenterName").removeClass("input-error");
	}

	var businessCostCenterLineOne = $("#businessCostCenterLineOne").val();
	if (businessCostCenterLineOne.length == 0) {
		$("#businessCostCenterLineOne").addClass("input-error");
		valid = false;
	}
	else {
		$("#businessCostCenterLineOne").removeClass("input-error");
	}

	var businessCostCenterLineTwo = $("#businessCostCenterLineTwo").val();
	if (businessCostCenterLineTwo.length == 0) {
		$("#businessCostCenterLineTwo").addClass("input-error");
		valid = false;
	}
	else {
		$("#businessCostCenterLineTwo").removeClass("input-error");
	}

	var businessCostCenterCity = $("#businessCostCenterCity").val();
	if (businessCostCenterCity.length == 0) {
		$("#businessCostCenterCity").addClass("input-error");
		valid = false;
	}
	else {
		$("#businessCostCenterCity").removeClass("input-error");
	}

	var businessCostCenterPostalCode = $("#businessCostCenterPostalCode").val();
	if (businessCostCenterPostalCode.length == 0) {
		$("#businessCostCenterPostalCode").addClass("input-error");
		valid = false;
	}
	else {
		$("#businessCostCenterPostalCode").removeClass("input-error");
	}

	if (valid) {
		var url = cloudCodeURL + "createCostCenter";
		$.ajax({
			type: "POST",
			headers: headers,
			url: url,
			data: '{"currentAccountId":"' + currentAccountId + '",'
				+ '"businessCostCenterName":"' + businessCostCenterName + '",'
				+ '"businessCostCenterLineOne":"' + businessCostCenterLineOne + '",'
				+ '"businessCostCenterLineTwo":"' + businessCostCenterLineTwo + '",'
				+ '"businessCostCenterPostalCode":"' + businessCostCenterPostalCode + '",'
				+ '"businessCostCenterCity":"' + businessCostCenterCity + '"}',
			contentType: "application/jsonp",
			success: function (httpResponse) {
				if (httpResponse.result.status == "success") {
					$("#addCostCenterForm").slideUp();

					$("#businessCostCenterName").val("");
					$("#businessCostCenterLineOne").val("");
					$("#businessCostCenterLineTwo").val("");
					$("#businessCostCenterCity").val("");
					$("#businessCostCenterPostalCode").val("");
					$("#businessCostCenterCountry").val("");

					addNewCompanyAddressState = 0;

					var costCentre = httpResponse.result.costCentre;
					var country = httpResponse.result.country;

					var costCenterUsers = httpResponse.result.costCenterUsers;
					var costCenterAuthorisers = httpResponse.result.costCenterAuthorisers;
					console.log(costCenterUsers)
					console.log(costCenterAuthorisers)

					var html = '';
					html += '<div id="costCenterRowContainer-' + costCentre.objectId + '" class="addressRowContainer">';
					html += '<div id="address-row-' + costCentre.objectId + '" class="row order-table-row-inactive">';
					html += '<div class="col-xs-3 ">';
					html += '<div id="address-label-' + costCentre.objectId + '" class="order-table-item-text" >' + costCentre.name + '</div>';
					html += '</div>';
					html += '<div class="col-xs-3">';
					html += '<div class="order-table-item-text" >' + costCentre.addressLine1 + ' ' + costCentre.addressLine2 + '</div>';
					html += '</div>';
					html += '<div class="col-xs-2 ">';
					html += '<div class="order-table-item-text" >' + costCentre.city + '</div>';
					html += '</div>';
					html += '<div  class="col-xs-4" style=" padding-right: 0px; text-align: right;">';
					html += '<div  class="col-xs-12 noPadding" >';
					if (renderUserCanApprove) {
						html += '<button type="button" id="edit-row-button-' + costCentre.objectId + '" onclick="showUpdateEditForm(\'' + costCentre.objectId + '\')" class="order-table-item-button_decline">EDIT</button>';
						html += '<p id="deleteConfirmation-' + costCentre.objectId + '" class="officebox-form-submit-text" style="width:65%; position: absolute; display:none;"><a onclick="deleteAddress(\'' + costCentre.objectId + '\')" class="office-box-linked-label" style="color:#ee8534;">DELETE ADDRESS?</a><br> <a onclick="hideDeleteConfirmation(\'' + costCentre.objectId + '\')" class="office-box-linked-label">CANCEL</a> </p>';
						html += '<button type="button" id="delete-row-button-' + costCentre.objectId + '" onclick="showDeleteConfirmation(\'' + costCentre.objectId + '\')" class="order-table-item-button_download_invoice">DELETE</button>';
					}
					html += '</div>';
					html += '</div>';
					html += '</div>';
					html += '<div class="row address-edit-form" id="address-edit-form-' + costCentre.objectId + '" style="display:none;">';
					html += '<div class="col-xs-6 col-md-4 noPadding">';
					html += '<form method="post">';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">ADDRESS NAME</label>';
					html += '<input type="text" name="businessCostCenterName" id="businessCostCenterName-' + costCentre.objectId + '" value="' + costCentre.name + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">ADDRESS LINE 1</label>';
					html += '<input type="text" name="businessCostCenterLineOne" id="businessCostCenterLineOne-' + costCentre.objectId + '" value="' + costCentre.addressLine1 + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">ADDRESS LINE 2</label>';
					html += '<input type="text" name="businessCostCenterLineTwo" id="businessCostCenterLineTwo-' + costCentre.objectId + '" value="' + costCentre.addressLine2 + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">CITY</label>';
					html += '<input type="text" name="businessCostCenterCity" id="businessCostCenterCity-' + costCentre.objectId + '" value="' + costCentre.city + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<div style="display: flex">';
					html += '<div class="col-xs-6 no-padding-col padding-right-split-input-col">';
					html += '<label class="form-input-label">POSTAL CODE</label>';
					html += '<input type="text" name="businessCostCenterPostalCode" id="businessCostCenterPostalCode-' + costCentre.objectId + '" value="' + costCentre.postalCode + '" class="form-control form-input" aria-label="...">';
					html += '</div>';
					html += '<div class="col-xs-6 no-padding-col padding-left-split-input-col">';
					html += '<label class="form-input-label">COUNTRY</label>';
					html += '<input type="text" name="businessCostCenterCountry" id="businessCostCenterCountry-' + costCentre.objectId + '" value="' + country.name + '" class="form-control form-input" aria-label="...">';
					html += '</div>';
					html += '</div>';
					html += '</div>';
					//******* COST CENTER USERS ******
					html += '<div class="row form-input-row">';
					html += '<div class="row">';
					html += '<label class="form-input-label">USERS</label>';
					html += '</div>';
					html += '<div class="row">';
					html += '<select id="costCenterUsers-' + costCentre.objectId + '" class="selectpicker" multiple title="Users">';
					for (var x = 0; x < costCenterUsers.length; x++) {
						if (costCenterUsers[x].selected == true) {
							html += '<option value="' + costCenterUsers[x].objectId + '" selected>' + costCenterUsers[x].firstName + ' ' + costCenterUsers[x].lastName + '</option>';
						}
						else {
							html += '<option value="' + costCenterUsers[x].objectId + '">' + costCenterUsers[x].firstName + ' ' + costCenterUsers[x].lastName + '</option>';
						}
					}
					html += '</select>';
					html += '</div>';
					html += '</div>';
					//******* COST CENTER USERS ******
					//******* COST CENTER AUTHORISERS ******
					html += '<div class="row form-input-row">';
					html += '<div class="row">';
					html += '<label class="form-input-label">AUTHORISERS</label>';
					html += '</div>';
					html += '<div class="row">';
					html += '<select id="costCenterAuthorisers-' + costCentre.objectId + '" class="selectpicker" multiple title="Authorisers">';
					for (var x = 0; x < costCenterAuthorisers.length; x++) {
						if (costCenterAuthorisers[x].selected == true) {
							html += '<option value="' + costCenterAuthorisers[x].objectId + '" selected>' + costCenterAuthorisers[x].firstName + ' ' + costCenterAuthorisers[x].lastName + '</option>';
						}
						else {
							html += '<option value="' + costCenterAuthorisers[x].objectId + '">' + costCenterAuthorisers[x].firstName + ' ' + costCenterAuthorisers[x].lastName + '</option>';
						}
					}
					html += '</select>';
					html += '</div>';
					html += '</div>';
					//******* COST CENTER AUTHORISERS ******

					html += '<div class="row  form-input-row">';
					html += '<div class="col-xs-6 no-padding-col">';
					html += '<button onclick="addUpdateCostCenter(false,\'' + costCentre.objectId + '\')" type="button" class="officebox-form-submit-button button-orange">UPDATE DETAILS</button>';
					html += '</div>';
					html += '<div class="col-xs-6 padding-left-extra-col">';
					html += '<p class="officebox-form-submit-text">CHANGED MY MIND.<br> <a onclick="hideAddressUpdateEditForm(\'' + costCentre.objectId + '\')" class="office-box-linked-label">CANCEL</a> </p>';
					html += '</div>';
					html += '</div>';
					html += '</form>';
					html += '</div>';
					html += '</div>';
					html += '</div>';

					$("#costCenterRows").append(html);
					$('.selectpicker').selectpicker({
						style: 'ob-btn-info',
						size: 4
					});
				}
			},
			error: function (httpResponse) {

			}
		});
	}
}

function updateCostCenterRow(costCentre, costCenterAuthorisers, costCenterUsers) {
	hideAddressUpdateEditForm(costCentre.objectId);

	var html = '';
	html += '<div id="address-row-' + costCentre.objectId + '" class="row order-table-row-inactive">';
	html += '<div class="col-xs-3 ">';
	html += '<div id="address-label-' + costCentre.objectId + '" class="order-table-item-text" >' + $("#businessCostCenterName-" + costCentre.objectId).val() + '</div>';
	html += '</div>';
	html += '<div class="col-xs-3">';
	html += '<div class="order-table-item-text" >' + $("#businessCostCenterLineOne-" + costCentre.objectId).val() + ' ' + $("#businessCostCenterLineTwo-" + costCentre.objectId).val() + '</div>';
	html += '</div>';
	html += '<div class="col-xs-2 ">';
	html += '<div class="order-table-item-text" >' + $("#businessCostCenterCity-" + costCentre.objectId).val() + '</div>';
	html += '</div>';
	html += '<div  class="col-xs-4" style=" padding-right: 0px; text-align: right;">';
	html += '<div  class="col-xs-12 noPadding" >';
	if (renderUserCanApprove) {
		html += '<button type="button" id="edit-row-button-' + costCentre.objectId + '" onclick="showUpdateEditForm(\'' + costCentre.objectId + '\')" class="order-table-item-button_decline">EDIT</button>';
		html += '<p id="deleteConfirmation-' + costCentre.objectId + '" class="officebox-form-submit-text" style="width:65%; position: absolute; display:none;"><a onclick="deleteAddress(\'' + costCentre.objectId + '\')" class="office-box-linked-label" style="color:#ee8534;">DELETE ADDRESS?</a><br> <a onclick="hideDeleteConfirmation(\'' + costCentre.objectId + '\')" class="office-box-linked-label">CANCEL</a> </p>';
		html += '<button type="button" id="delete-row-button-' + costCentre.objectId + '" onclick="showDeleteConfirmation(\'' + costCentre.objectId + '\')" class="order-table-item-button_download_invoice">DELETE</button>';
	}
	html += '</div>';
	html += '</div>';
	html += '</div>';
	html += '<div class="row address-edit-form" id="address-edit-form-' + costCentre.objectId + '" style="display:none;">';
	html += '<div class="col-xs-6 col-md-4 noPadding">';
	html += '<form method="post">';
	html += '<div class="row form-input-row">';
	html += '<label class="form-input-label">ADDRESS NAME</label>';
	html += '<input type="text" name="businessCostCenterName" id="businessCostCenterName-' + costCentre.objectId + '" value="' + $("#businessCostCenterName-" + costCentre.objectId).val() + '" class="form-control form-input" aria-label="..." placeholder="">';
	html += '</div>';
	html += '<div class="row form-input-row">';
	html += '<label class="form-input-label">ADDRESS LINE 1</label>';
	html += '<input type="text" name="businessCostCenterLineOne" id="businessCostCenterLineOne-' + costCentre.objectId + '" value="' + $("#businessCostCenterLineOne-" + costCentre.objectId).val() + '" class="form-control form-input" aria-label="..." placeholder="">';
	html += '</div>';
	html += '<div class="row form-input-row">';
	html += '<label class="form-input-label">ADDRESS LINE 2</label>';
	html += '<input type="text" name="businessCostCenterLineTwo" id="businessCostCenterLineTwo-' + costCentre.objectId + '" value="' + $("#businessCostCenterLineTwo-" + costCentre.objectId).val() + '" class="form-control form-input" aria-label="..." placeholder="">';
	html += '</div>';
	html += '<div class="row form-input-row">';
	html += '<label class="form-input-label">CITY</label>';
	html += '<input type="text" name="businessCostCenterCity" id="businessCostCenterCity-' + costCentre.objectId + '" value="' + $("#businessCostCenterCity-" + costCentre.objectId).val() + '" class="form-control form-input" aria-label="..." placeholder="">';
	html += '</div>';
	html += '<div class="row form-input-row">';
	html += '<div style="display: flex">';
	html += '<div class="col-xs-6 no-padding-col padding-right-split-input-col">';
	html += '<label class="form-input-label">POSTAL CODE</label>';
	html += '<input type="text" name="businessCostCenterPostalCode" id="businessCostCenterPostalCode-' + costCentre.objectId + '" value="' + $("#businessCostCenterPostalCode-" + costCentre.objectId).val() + '" class="form-control form-input" aria-label="...">';
	html += '</div>';
	html += '<div class="col-xs-6 no-padding-col padding-left-split-input-col">';
	html += '<label class="form-input-label">COUNTRY</label>';
	html += '<input type="text" name="businessCostCenterCountry" id="businessCostCenterCountry-' + costCentre.objectId + '" value="' + $("#businessCostCenterCountry-" + costCentre.objectId).val() + '" class="form-control form-input" aria-label="...">';
	html += '</div>';
	html += '</div>';
	//******* COST CENTER USERS ******
	html += '<div class="row form-input-row">';
	html += '<div class="row">';
	html += '<label class="form-input-label">USERS</label>';
	html += '</div>';
	html += '<div class="row">';
	html += '<select id="costCenterUsers-' + costCentre.objectId + '" class="selectpicker" multiple title="Users">';
	console.log(costCenterUsers)
	for (var x = 0; x < costCenterUsers.length; x++) {
		if (costCenterUsers[x].selected == true) {
			html += '<option value="' + costCenterUsers[x].objectId + '" selected>' + costCenterUsers[x].firstName + ' ' + costCenterUsers[x].lastName + '</option>';
		}
		else {
			html += '<option value="' + costCenterUsers[x].objectId + '">' + costCenterUsers[x].firstName + ' ' + costCenterUsers[x].lastName + '</option>';
		}
	}
	html += '</select>';
	html += '</div>';
	html += '</div>';
	//******* COST CENTER USERS ******
	//******* COST CENTER AUTHORISERS ******
	html += '<div class="row form-input-row">';
	html += '<div class="row">';
	html += '<label class="form-input-label">AUTHORISERS</label>';
	html += '</div>';
	html += '<div class="row">';
	html += '<select id="costCenterAuthorisers-' + costCentre.objectId + '" class="selectpicker" multiple title="Authorisers">';
	for (var x = 0; x < costCenterAuthorisers.length; x++) {
		if (costCenterAuthorisers[x].selected == true) {
			html += '<option value="' + costCenterAuthorisers[x].objectId + '" selected>' + costCenterAuthorisers[x].firstName + ' ' + costCenterAuthorisers[x].lastName + '</option>';
		}
		else {
			html += '<option value="' + costCenterAuthorisers[x].objectId + '">' + costCenterAuthorisers[x].firstName + ' ' + costCenterAuthorisers[x].lastName + '</option>';
		}
	}
	html += '</select>';
	html += '</div>';
	html += '</div>';
	//******* COST CENTER AUTHORISERS ******
	html += '</div>';
	html += '<div class="row  form-input-row">';
	html += '<div class="col-xs-6 no-padding-col">';
	html += '<button onclick="addUpdateCostCenter(false,\'' + costCentre.objectId + '\')" type="button" class="officebox-form-submit-button button-orange">UPDATE DETAILS</button>';
	html += '</div>';
	html += '<div class="col-xs-6 padding-left-extra-col">';
	html += '<p class="officebox-form-submit-text">CHANGED MY MIND.<br> <a onclick="hideAddressUpdateEditForm(\'' + costCentre.objectId + '\')" class="office-box-linked-label">CANCEL</a> </p>';
	html += '</div>';
	html += '</div>';
	html += '</form>';
	html += '</div>';
	html += '</div>';
	html += '</div>';

	$("#costCenterRowContainer-" + costCentre.objectId).html(html);
	$('.selectpicker').selectpicker({
		style: 'ob-btn-info',
		size: 4
	});
}

function getCostCentres() {
	console.log("Querying cost centres");

	var url = cloudCodeURL + "getBusinessCostCentres";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"currentAccountId":"' + currentAccountId + '",' + '"val":"' + "val1" + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse) {
			var costCentres = httpResponse.result.costCentres;

			console.log("LOGGING COST CENTERS");
			console.log(httpResponse);

			if (httpResponse.result.status == "success") {

				$("#costCenterRows").empty();
				for (var i = 0; i < costCentres.length; i++) {
					var costCentre = costCentres[i];

					var html = '';
					html += '<div id="costCenterRowContainer-' + costCentre.objectId + '" class="addressRowContainer">';
					html += '<div id="address-row-' + costCentre.objectId + '" class="row order-table-row-inactive">';
					html += '<div class="col-xs-3 ">';
					html += '<div id="address-label-' + costCentre.objectId + '" class="order-table-item-text" >' + costCentre.name + '</div>';
					html += '</div>';
					html += '<div class="col-xs-3">';
					html += '<div class="order-table-item-text" >' + costCentre.addressLine1 + ' ' + costCentre.addressLine2 + '</div>';
					html += '</div>';
					html += '<div class="col-xs-2 ">';
					html += '<div class="order-table-item-text" >' + costCentre.city + '</div>';
					html += '</div>';
					html += '<div  class="col-xs-4" style=" padding-right: 0px; text-align: right;">';
					html += '<div  class="col-xs-12 noPadding" >';
					if (renderUserCanApprove) {
						html += '<button type="button" id="edit-row-button-' + costCentre.objectId + '" onclick="showUpdateEditForm(\'' + costCentre.objectId + '\')" class="order-table-item-button_decline">EDIT</button>';
						html += '<p id="deleteConfirmation-' + costCentre.objectId + '" class="officebox-form-submit-text" style="width:65%; position: absolute; display:none;"><a onclick="deleteCostCentre(\'' + costCentre.objectId + '\')" class="office-box-linked-label" style="color:#ee8534;">DELETE CENTRE?</a><br> <a onclick="hideDeleteConfirmation(\'' + costCentre.objectId + '\')" class="office-box-linked-label">CANCEL</a> </p>';
						html += '<button type="button" id="delete-row-button-' + costCentre.objectId + '" onclick="showDeleteConfirmation(\'' + costCentre.objectId + '\')" class="order-table-item-button_download_invoice">DELETE</button>';
					}
					html += '</div>';
					html += '</div>';
					html += '</div>';
					html += '<div class="row address-edit-form" id="address-edit-form-' + costCentre.objectId + '" style="display:none;">';
					html += '<div class="col-xs-6 col-md-4 noPadding">';
					html += '<form method="post">';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">ADDRESS NAME</label>';
					html += '<input type="text" name="businessCostCenterName" id="businessCostCenterName-' + costCentre.objectId + '" value="' + costCentre.name + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">ADDRESS LINE 1</label>';
					html += '<input type="text" name="businessCostCenterLineOne" id="businessCostCenterLineOne-' + costCentre.objectId + '" value="' + costCentre.addressLine1 + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">ADDRESS LINE 2</label>';
					html += '<input type="text" name="businessCostCenterLineTwo" id="businessCostCenterLineTwo-' + costCentre.objectId + '" value="' + costCentre.addressLine2 + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<label class="form-input-label">CITY</label>';
					html += '<input type="text" name="businessCostCenterCity" id="businessCostCenterCity-' + costCentre.objectId + '" value="' + costCentre.city + '" class="form-control form-input" aria-label="..." placeholder="">';
					html += '</div>';
					html += '<div class="row form-input-row">';
					html += '<div style="display: flex">';
					html += '<div class="col-xs-6 no-padding-col padding-right-split-input-col">';
					html += '<label class="form-input-label">POSTAL CODE</label>';
					html += '<input type="text" name="businessCostCenterPostalCode" id="businessCostCenterPostalCode-' + costCentre.objectId + '" value="' + costCentre.postalCode + '" class="form-control form-input" aria-label="...">';
					html += '</div>';
					html += '<div class="col-xs-6 no-padding-col padding-left-split-input-col">';
					html += '<label class="form-input-label">COUNTRY</label>';
					html += '<input type="text" name="businessCostCenterCountry" id="businessCostCenterCountry-' + costCentre.objectId + '" value="' + costCentre.country + '" class="form-control form-input" aria-label="...">';
					html += '</div>';
					html += '</div>';
					html += '</div>';
					//******* COST CENTER USERS ******
					html += '<div class="row form-input-row">';
					html += '<div class="row">';
					html += '<label class="form-input-label">USERS</label>';
					html += '</div>';
					html += '<div class="row">';
					html += '<select id="costCenterUsers-' + costCentre.objectId + '" class="selectpicker" multiple title="Users">';
					for (var x = 0; x < costCentre.users.length; x++) {
						if (costCentre.users[x].selected == true) {
							html += '<option value="' + costCentre.users[x].objectId + '" selected>' + costCentre.users[x].firstName + ' ' + costCentre.users[x].lastName + '</option>';
						}
						else {
							html += '<option value="' + costCentre.users[x].objectId + '">' + costCentre.users[x].firstName + ' ' + costCentre.users[x].lastName + '</option>';
						}
					}
					html += '</select>';
					html += '</div>';
					html += '</div>';
					//******* COST CENTER USERS ******
					//******* COST CENTER AUTHORISERS ******
					html += '<div class="row form-input-row">';
					html += '<div class="row">';
					html += '<label class="form-input-label">AUTHORISERS</label>';
					html += '</div>';
					html += '<div class="row">';
					html += '<select id="costCenterAuthorisers-' + costCentre.objectId + '" class="selectpicker" multiple title="Authorisers">';
					for (var x = 0; x < costCentre.authorisers.length; x++) {
						if (costCentre.authorisers[x].selected == true) {
							html += '<option value="' + costCentre.authorisers[x].objectId + '" selected>' + costCentre.authorisers[x].firstName + ' ' + costCentre.authorisers[x].lastName + '</option>';
						}
						else {
							html += '<option value="' + costCentre.authorisers[x].objectId + '">' + costCentre.authorisers[x].firstName + ' ' + costCentre.authorisers[x].lastName + '</option>';
						}
					}
					html += '</select>';
					html += '</div>';
					html += '</div>';
					//******* COST CENTER AUTHORISERS ******
					html += '<div class="row  form-input-row">';
					html += '<div class="col-xs-6 no-padding-col">';
					html += '<button onclick="addUpdateCostCenter(false,\'' + costCentre.objectId + '\')" type="button" class="officebox-form-submit-button button-orange">UPDATE DETAILS</button>';
					html += '</div>';
					html += '<div class="col-xs-6 padding-left-extra-col">';
					html += '<p class="officebox-form-submit-text">CHANGED MY MIND.<br> <a onclick="hideAddressUpdateEditForm(\'' + costCentre.objectId + '\')" class="office-box-linked-label">CANCEL</a> </p>';
					html += '</div>';
					html += '</div>';
					html += '</form>';
					html += '</div>';
					html += '</div>';
					html += '</div>';

					$("#costCenterRows").append(html);
					$('.selectpicker').selectpicker({
						style: 'ob-btn-info',
						size: 4
					});
				};
			}
		},
		error: function (httpResponse) {
			console.log("FAILED TO QUERY COST CENTRES");
			console.log(httpResponse);
		}
	});
}

//BUSINESS PROFILE USERS
function getBusinessProfileUsers() {
	var url = cloudCodeURL + "getBusinessProfileUsers";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"currentAccountId":"' + currentAccountId + '",'
			+ '"val":"' + "val1" + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse) {
			console.log(httpResponse);
			if (httpResponse.result.status == "success") {
				console.log("ABOUT TO LOG CUSTOMER TO ACCOUNT");
				console.log(httpResponse.result);

				var customerToAccounts = httpResponse.result.customerToAccounts;
				var allCostCenters = httpResponse.result.allCostCenters;

				for (var i = 0; i < customerToAccounts.length; i++) {
					var customerToAccount = customerToAccounts[i];
					console.log("Customer to account: ");
					console.log(customerToAccount);

					if (customerToAccount.customer) {
						var html = '';
						html += '<div id="customerToAccountsRowContainer-' + customerToAccount.customer.objectId + '" class="addressRowContainer">';
						html += '<div id="address-row-' + customerToAccount.customer.objectId + '" class="row order-table-row-inactive">';
						html += '<div class="col-xs-3 ">';
						html += '<div id="CTAName-' + customerToAccount.objectId + '" class="order-table-item-text" style="white-space: pre">' + customerToAccount.customer.firstName + ' ' + customerToAccount.customer.lastName + '</div>';
						html += '</div> ';
						html += '<div class="col-xs-2">';
						html += '<div id="CTAAddress-' + customerToAccount.objectId + '" class="order-table-item-text" >' + customerToAccount.customer.type.name + '</div>';
						html += '</div>';
						html += '<div class="col-xs-3">';
						html += '<div id="CTAEmail-' + customerToAccount.objectId + '" class="order-table-item-text" style="white-space: pre">' + customerToAccount.customer.email + '</div>';
						html += '</div>';
						html += '<div  class="col-xs-4" style=" padding-right: 0px; text-align: right;">';
						html += '<div  class="col-xs-12 noPadding" >';
						if (renderUserCanApprove) {
							html += '<button type="button" id="edit-row-button-' + customerToAccount.objectId + '" onclick="showUpdateEditCustomerToAccountForm(\'' + customerToAccount.objectId + '\',\'' + customerToAccount.customer.objectId + '\')" class="order-table-item-button_decline">EDIT</button>';
							html += '<p id="deleteConfirmation-' + customerToAccount.objectId + '" class="officebox-form-submit-text" style="width:65%; position: absolute; display:none;"><a onclick="deleteCustomerToAccount(\'' + customerToAccount.objectId + '\',\'' + customerToAccount.customer.objectId + '\')" class="office-box-linked-label" style="color:#ee8534;">DELETE ADDRESS?</a><br> <a onclick="hideDeleteConfirmation(\'' + customerToAccount.objectId + '\')" class="office-box-linked-label">CANCEL</a> </p>';
							html += '<button type="button" id="delete-row-button-' + customerToAccount.objectId + '" onclick="showDeleteConfirmation(\'' + customerToAccount.objectId + '\')" class="order-table-item-button_download_invoice">DELETE</button>';
						}
						html += '</div>';
						html += '</div>';
						html += '</div>';
						html += '<div class="row address-edit-form" id="customerToAccounts-edit-form-' + customerToAccount.objectId + '" style="display:none;">';
						html += '<div class="col-xs-6 col-md-4 noPadding">';
						html += '<form method="post">';
						html += '<div class="row form-input-row">';
						html += '<label class="form-input-label">NAME</label>';
						html += '<input type="text" name="businessCostCenterName" id="customerToAccountFirstName-' + customerToAccount.objectId + '" value="' + customerToAccount.customer.firstName + '" class="form-control form-input" aria-label="..." placeholder="">';
						html += '</div>';
						html += '<div class="row form-input-row">';
						html += '<label class="form-input-label">LAST NAME</label>';
						html += '<input type="text" name="businessCostCenterLineOne" id="customerToAccountLastName-' + customerToAccount.objectId + '" value="' + customerToAccount.customer.lastName + '" class="form-control form-input" aria-label="..." placeholder="">';
						html += '</div>';
						html += '<div class="row form-input-row">';
						html += '<label class="form-input-label">EMAIL ADDRESS</label>';
						html += '<input type="text" name="businessCostCenterLineTwo" id="customerToAccountEmail-' + customerToAccount.objectId + '" value="' + customerToAccount.customer.email + '" class="form-control form-input" aria-label="..." placeholder="">';
						html += '</div>';
						html += '<div class="row form-input-row">';
						html += '<label class="form-input-label">MOBILE NUMBER</label>';
						html += '<input type="text" name="businessCostCenterCity" id="customerToAccountContact-' + customerToAccount.objectId + '" value="' + customerToAccount.customer.contactNumber + '" class="form-control form-input" aria-label="..." placeholder="">';
						html += '</div>';
						html += '<div class="row form-input-row">';
						html += '<select id="userType-' + customerToAccount.objectId + '" class="selectpicker">';
						if (customerToAccount.customer.type.objectId == "ebqnlutqxk") {
							html += '<option value="ebqnlutqxk" selected>ADMIN</option>';
							html += '<option value="so1SAZPyVz">NORMAL</option>';
						} else {
							html += '<option value="ebqnlutqxk">ADMIN</option>';
							html += '<option value="so1SAZPyVz" selected>NORMAL</option>';
						}
						html += '</select>';
						html += '</div>';
						html += '<div class="row form-input-row">';
						html += '<div class="col-xs-6 no-padding-col">';
						html += '<button onclick="updateCustomerToAccount(\'' + customerToAccount.objectId + '\',\'' + customerToAccount.customer.objectId + '\')" type="button" class="officebox-form-submit-button button-orange">UPDATE DETAILS</button>';
						html += '</div>';
						html += '<div class="col-xs-6 padding-left-extra-col">';
						html += '<p class="officebox-form-submit-text">CHANGED MY MIND.<br> <a onclick="hideCustomerToAccountEditForm(\'' + customerToAccount.objectId + '\)" class="office-box-linked-label">CANCEL</a> </p>';
						html += '</div>';
						html += '</div>';
						html += '</form>';
						html += '</div>';
						html += '<div class="col-xs-6 col-md-4 col-md-offset-2" style="display:none;">';
						html += '<h2 class="officebox-step-header-title" style="padding-bottom:20px;">Cost Centers</h2>';
						for (var z = 0; z < allCostCenters.length; z++) {
							html += '<div class="col-xs-6">';
							html += '<input type="checkbox" id="costCenterCheckbox' + customerToAccount.customer.objectId + allCostCenters[z].objectId + '" class="checkbox costCenterCheckbox' + customerToAccount.customer.objectId + '"><label class="costCenterCheckboxLabel" for="costCenterCheckbox' + customerToAccount.customer.objectId + allCostCenters[z].objectId + '">' + allCostCenters[z].name + '</label>';
							html += '</div>';
						}
						html += '</div>';
						html += '</div>';

						$("#customerRows").append(html);
					}
				};
			}
		},
		error: function (httpResponse) {
			console.log(httpResponse);
		}
	});
}

function deleteCustomerToAccount(customerObjectId, containerID) {
	$("#customerToAccountsRowContainer-" + containerID).remove();

	var url = cloudCodeURL + "deleteCustomerToAccount";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"customerObjectId":"' + customerObjectId + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse) {
			if (httpResponse.result.code == 200) {

			}
		},
		error: function (httpResponse) {

		}
	});
}

function deleteCostCentre(costCentreId) {
	$("#costCenterRowContainer-" + costCentreId).remove();

	var url = cloudCodeURL + "deleteCostCentre";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"costCentreId":"' + costCentreId + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse) {
			if (httpResponse.result.code == 200) {

			}
		},
		error: function (httpResponse) {

		}
	});
}