//REGISTRATION VARIABLES
var firstname;
var lastname;
var username;
var pwd;
var confirm;
var companyname;
var contactnumber;
var addressline1;
var addressline2;
var addressline3 = "";
var city;
var postcode;
var country;
var creditlimit;
var companyregistration;
var companyvatnumber;
var cnameOne;
var cnameTwo;
var cnameThree;
var ccnameOne;
var ccnameTwo;
var ccnameThree;
var ccsurnameOne;
var ccsurnameTwo;
var ccsurnameThree;
var cemailOne;
var isTermsChecked;
var cemailTwo;
var cemailThree;
var ctelnumberOne;
var ctelnumberTwo;
var ctelnumberThree;
var Form1Done;
var Form2Done;
var Form3Done = true;
$(document).ready(function () {
	$("#spinner").hide();
	$("#firstname-error").hide();
	$("#lastname-error").hide();
	$("#email-error").hide();
	$("#password-error").hide();
	$("confirm-error").hide();
	$("register-error").hide()

	$("#companyname-error").hide();
	$("#contactnumber-error").hide();
	$("#addressline1-error").hide();
	$("#addressline2-error").hide();
	$("#city-error").hide();
	$("#postcode-error").hide();

	$("#creditlimit-error").hide();
	$("#companyregistration-error").hide();
	$("#ccsurnameOne-error").hide();
	$("#companyvatnumber-error").hide();

	$("#cnameOne-error").hide();
	$("#ccnameOne-error").hide();
	$("#ccsurnameOne-error").hide();
	$("#cemailOne-error").hide();
	$("ctelnumberOne-error").hide();

	$("#cnameTwo-error").hide();
	$("#ccnameTwo-error").hide();
	$("#ccsurnameTwo-error").hide();
	$("#cemailTwo-error").hide();
	$("ctelnumberTwo-error").hide();

	$("#cnameThree-error").hide();
	$("#ccnameThree-error").hide();
	$("#ccsurnameThree-error").hide();
	$("#cemailThree-error").hide();
	$("#ctelnumberThree-error").hide();
	$("#register-error").hide();
});
function changeActive(index) {
	$(".officebox-step").removeClass("active-step");
	$(".step-label-span").removeClass("step-label-active");
	$("#s" + index).addClass("active-step");
	$("#ss" + index).addClass("step-label-active");
	$(".business-form-container").hide();
	$("#step-container-" + index).show();
}

function showThirtyDayContainer() {
	$("#thirty-day-option-row").hide();
	$(".thirty-day-option-form").show();
}

function confirmPersonalDetails() {
	var valid = true;

	// $("#spinner").show();
	firstname = $("#firstname").val();
	if (firstname.length == 0) {
		$("#firstname").addClass("input-error");
		valid = false;
		$("#firstname-error").show();
		$("#firstname-error").text("FIRST NAME FIELD CANNOT BE EMPTY");
	}
	else {
		$("#firstname").removeClass("input-error");
		$("#firstname-error").hide();
	}

	lastname = $("#lastname").val();
	if (lastname.length == 0) {
		$("#lastname-error").show();
		$("#lastname").addClass("input-error");
		$("#lastname-error").text("LAST NAME FIELD CANNOT BE EMPTY");
		valid = false;
	}
	else {
		$("#lastname").removeClass("input-error");
		$("#lastname-error").hide();
	}


	username = $("#username").val();
	if (username.length == 0) {
		$("#email-error").show();
		$("#username").addClass("input-error");
		$("#email-error").text("EMAIL ADDRESS FIELD CANNOT BE EMPTY");
		valid = false;
	}
	else {
		$("#email-error").hide();
		$("#username").removeClass("input-error");
	}
	pwd = $("#pwd").val();
	if (pwd.length == 0) {
		$("#password-error").show();
		$("#pwd").addClass("input-error");
		$("#password-error").text("PASSWORD FIELD CANNOT BE EMPTY");
		valid = false;
	}
	else {
		$("#password-error").hide();
		$("#pwd").removeClass("input-error");

	}

	confirm = $("#confirm").val();
	if (confirm.length == 0) {

		$("#confirm").addClass("input-error");
		$("#confirm-error").show();
		$("#confirm-error").text("CONFIRM PASSWORD FIELD CANNOT BE EMPTY");
		valid = false;

	}
	else {
		$("#confirm-error").hide();
		$("#confirm").removeClass("input-error");
	}

	if (pwd !== confirm) {

		console.log("password match");
		$("#confirm-error").show();
		$("#password-error").show();
		$("#password-error").text("PASSWORD FIELD DOES NOT MATCH CONFIRM PASSWORD FIELD");
		$("#confirm-error").text("CONFIRM PASSWORD FIELD DOES NOT MATCH PASSWORD FIELD");
		$("#confirm").addClass("input-error");
		$("#pwd").addClass("input-error");
		valid = false;

	} else {
		// $("#confirm-error").hide();
		// $("#password-error").hide();

	}

	if (valid) {
		changeActive('2');
		// $("#spinner").hide();
		// $("#register").show();
	}
	Form1Done = valid;
}

function confirmBusinessDetails() {

	var valid = true;
	companyname = $("#companyname").val();
	if (companyname.length == 0) {
		$("#companyname").addClass("input-error");
		$("#companyname-error").show();
		$("#companyname-error").text("COMPANY NAME CANNOT BE EMPTY");
		valid = false;
		console.log("valid");
	}
	else {

		$("#companyname-error").hide();
		$("#companyname").removeClass("input-error");
	}

	contactnumber = $("#contactnumber").val();
	if (contactnumber.length == 0) {
		$("#contactnumber").addClass("input-error");

		valid = false;
		$("#contactnumber-error").show();
		$("#contactnumber-error").text("CONTACT NUMBER CANNOT BE EMPTY");
	}
	else {
		$("#contactnumber-error").hide();
		$("#contactnumber").removeClass("input-error");
	}

	addressline1 = $("#addressline1").val();
	if (addressline1.length == 0) {
		$("#addressline1").addClass("input-error");
		valid = false;

		$("#addressline1-error").show();
		$("#addressline1-error").text("ADDRESS LINE 1 CANNOT BE EMPTY");
	}
	else {
		$("#addressline1-error").hide();
		$("#addressline1").removeClass("input-error");
	}

	addressline2 = $("#addressline2").val();
	if (addressline2.length == 0) {
		$("#addressline2").addClass("input-error");
		valid = false;

		$("#addressline2-error").show();
		$("#addressline2-error").text("ADDRESS LINE 2 CANNOT BE EMPTY");
	}
	else {
		$("#addressline2-error").hide();
		$("#addressline2").removeClass("input-error");
	}

	city = $("#city").val();
	if (city.length == 0) {
		$("#city").addClass("input-error");
		valid = false;

		$("#city-error").show();
		$("#city-error").text("CITY CANNOT BE EMPTY");
	}
	else {
		$("#city-error").hide();
		$("#city").removeClass("input-error");
	}

	postcode = $("#postcode").val();
	if (postcode.length == 0) {
		$("#postcode").addClass("input-error");
		valid = false;
		$("#postcode-error").show();
		$("#postcode-error").text("POSTAL CODE CANNOT BE EMPTY");
	}
	else {
		$("#postcode-error").hide();
		$("#postcode").removeClass("input-error");
	}



	if (valid) {
		changeActive('3');
	}

	Form2Done = valid;
}

function confirmCreditDetails() {
	var valid = true;
	creditlimit = $("#creditlimit").val();
	if (creditlimit.length == 0) {
		$("#creditlimit").addClass("input-error");
		valid = false;
		$("#creditlimit-error").show();
		$("#creditlimit-error").text("CREDIT LIMIT CANNOT BE EMPTY");
	}
	else {
		$("#creditlimit-error").hide();
		$("#creditlimit").removeClass("input-error");
	}

	companyregistration = $("#companyregistration").val();
	if (companyregistration.length == 0) {
		$("#companyregistration").addClass("input-error");
		valid = false;
		$("#companyregistration-error").show();
		$("#companyregistration-error").text("COMPANY REGISTRATION NUMBER CANNOT BE EMPTY");
	}
	else {
		$("#companyregistration").removeClass("input-error");
		$("#companyregistration-error").hide();
	}

	companyvatnumber = $("#companyvatnumber").val();
	if (companyvatnumber.length == 0) {
		$("#companyvatnumber-error").show();
		$("#companyvatnumber").addClass("input-error");
		$("#companyvatnumber-error").text("COMPANY VAT NUMBER CANNOT BE EMPTY");
		valid = false;
	}
	else {
		$("#companyvatnumber-error").hide();
		$("#companyvatnumber").removeClass("input-error");
	}
	cnameOne = $("#cnameOne").val();
	if (cnameOne.length == 0) {
		$("#cnameOne").addClass("input-error");
		valid = false;
		$("#cnameOne-error").show();
		$("#cnameOne-error").text("COMPANY NAME CANNOT BE EMPTY");
	}
	else {
		$("#cnameOne").removeClass("input-error");
		$("#cnameOne-error").hide();
	}

	cnameTwo = $("#cnameTwo").val();
	if (cnameTwo.length == 0) {
		$("#cnameTwo").addClass("input-error");
		valid = false;

		$("#cnameTwo-error").show();
		$("#cnameTwo-error").text("COMPANY NAME CANNOT BE EMPTY");
	}
	else {
		$("#cnameTwo-error").hide();
		$("#cnameTwo").removeClass("input-error");
	}

	cnameThree = $("#cnameThree").val();
	if (cnameThree.length == 0) {
		$("#cnameThree").addClass("input-error");
		valid = false;
		$("#cnameThree-error").show();
		$("#cnameThree-error").text("COMPANY NAME CANNOT BE EMPTY");
	}
	else {
		$("#cnameThree-error").hide();
		$("#cnameThree").removeClass("input-error");
	}

	ccnameOne = $("#ccnameOne").val();
	if (ccnameOne.length == 0) {
		$("#ccnameOne-error").show();
		$("#ccnameOne-error").text("CONTACT NAME CANNOT BE EMPTY");
		$("#ccnameOne").addClass("input-error");
		valid = false;
	}
	else {
		$("#ccnameOne-error").hide();
		$("#ccnameOne").removeClass("input-error");
	}

	ccnameTwo = $("#ccnameTwo").val();
	if (ccnameTwo.length == 0) {
		$("#ccnameTwo").addClass("input-error");
		valid = false;
		$("#ccnameTwo-error").show();
		$("#ccnameTwo-error").text("CONTACT NAME CANNOT BE EMPTY");
	}
	else {
		$("#ccnameTwo-error").hide();
		$("#ccnameTwo").removeClass("input-error");
	}

	ccnameThree = $("#ccnameThree").val();
	if (ccnameThree.length == 0) {
		$("#ccnameThree-error").show();
		$("#ccnameThree-error").text("CONTACT NAME CANNOT BE EMPTY");
		$("#ccnameThree").addClass("input-error");
		valid = false;
	}
	else {
		$("#ccnameThree-error").hide();
		$("#ccnameThree").removeClass("input-error");
	}

	ccsurnameOne = $("#ccsurnameOne").val();
	if (ccsurnameOne.length == 0) {
		$("#ccsurnameOne-error").show();
		$("#ccsurnameOne-error").text("CONTACT SURNAME CANNOT BE EMPTY");
		$("#ccsurnameOne").addClass("input-error");
		valid = false;
	}
	else {
		$("#ccsurnameOne-error").hide();
		$("#ccsurnameOne").removeClass("input-error");
	}

	ccsurnameTwo = $("#ccsurnameTwo").val();
	if (ccsurnameTwo.length == 0) {
		$("#ccsurnameTwo-error").show();
		$("#ccsurnameTwo-error").text("CONTACT SURNAME CANNOT BE EMPTY");
		$("#ccsurnameTwo").addClass("input-error");
		valid = false;
	}
	else {
		$("#ccsurnameTwo-error").hide();
		$("#ccsurnameTwo").removeClass("input-error");
	}

	ccsurnameThree = $("#ccsurnameThree").val();
	if (ccsurnameThree.length == 0) {
		$("#ccsurnameThree-error").show();
		$("#ccsurnameThree-error").text("CONTACT SURNAME CANNOT BE EMPTY");
		$("#ccsurnameThree").addClass("input-error");
		valid = false;
	}
	else {
		$("#ccsurnameThree-error").hide();
		$("#ccsurnameThree").removeClass("input-error");
	}


	cemailOne = $("#cemailOne").val();
	if (cemailOne.length == 0) {
		$("#cemailOne-error").show();
		$("#cemailOne-error").text("EMAIL CANNOT BE EMPTY");
		$("#cemailOne").addClass("input-error");
		valid = false;
	}
	else {
		$("#cemailOne-error").hide();
		$("#cemailOne").removeClass("input-error");
	}

	cemailTwo = $("#cemailTwo").val();
	if (cemailTwo.length == 0) {
		$("#cemailTwo").addClass("input-error");
		$("#cemailTwo-error").show();
		$("#cemailTwo-error").text("EMAIL CANNOT BE EMPTY");
		valid = false;
	}
	else {
		$("#cemailTwo-error").hide();
		$("#cemailTwo").removeClass("input-error");
	}

	cemailThree = $("#cemailThree").val();
	if (cemailThree.length == 0) {
		$("#cemailThree").addClass("input-error");
		valid = false;
		$("#cemailThree-error").show();
		$("#cemailThree-error").text("EMAIL CANNOT BE EMPTY");
	}
	else {
		$("#cemailThree-error").hide();
		$("#cemailThree").removeClass("input-error");
	}


	ctelnumberOne = $("#ctelnumberOne").val();
	if (ctelnumberOne.length == 0) {
		$("#ctelnumberOne-error").show();
		$("#ctelnumberOne-error").text("TEL NUMBER CANNOT BE EMPTY");
		$("#ctelnumberOne").addClass("input-error");
		valid = false;
	}
	else {
		$("#ctelnumberOne-error").hide();
		$("#ctelnumberOne").removeClass("input-error");
	}

	ctelnumberTwo = $("#ctelnumberTwo").val();
	if (ctelnumberTwo.length == 0) {
		$("#ctelnumberTwo-error").show();
		$("#ctelnumberTwo-error").text("TEL NUMBER CANNOT BE EMPTY");
		$("#ctelnumberTwo").addClass("input-error");
		valid = false;
	}
	else {
		$("#ctelnumberTwo-error").hide();
		$("#ctelnumberTwo").removeClass("input-error");
	}

	ctelnumberThree = $("#ctelnumberThree").val();
	if (ctelnumberThree.length == 0) {
		$("#ctelnumberThree-error").show();
		$("#ctelnumberThree-error").text("TEL NUMBER CANNOT BE EMPTY");
		$("#ctelnumberThree").addClass("input-error");
		valid = false;
	}
	else {
		$("#ctelnumberThree-error").hide();
		$("#ctelnumberThree").removeClass("input-error");
	}

	if (valid) {
		changeActive('4');
	} else {
		// $("#spinner").hide();
		// $("#register").show();
	}
	Form3Done = valid;

}

function confirmRegistration() {

	if (!Form1Done) {
		changeActive('1');
		confirmPersonalDetails();
	} else if (!Form2Done) {
		confirmBusinessDetails();
		changeActive('2');
	} else if (!Form3Done) {
		changeActive('3');
		// confirmCreditDetails();
	}
	else {
		$("#spinner").show();
		$("#confirmRegistration").hide();
		var url = cloudCodeURL + "businessRegistration";
		$.ajax({
			type: "POST",
			headers: headers,
			url: url,
			data: '{"firstname":"' + firstname + '",'
				+ '"lastname":"' + lastname + '",'
				+ '"username":"' + username + '",'
				+ '"confirm":"' + confirm + '",'
				+ '"companyname":"' + companyname + '",'
				+ '"contactnumber":"' + contactnumber + '",'
				+ '"addressline1":"' + addressline1 + '",'
				+ '"addressline2":"' + addressline2 + '",'
				+ '"addressline3":"' + addressline3 + '",'
				+ '"city":"' + city + '",'
				+ '"postcode":"' + postcode + '",'
				+ '"country":"' + country + '",'
				+ '"creditlimit":"' + creditlimit + '",'
				+ '"companyregistration":"' + companyregistration + '",'
				+ '"companyvatnumber":"' + companyvatnumber + '",'
				+ '"cnameOne":"' + cnameOne + '",'
				+ '"cnameTwo":"' + cnameTwo + '",'
				+ '"cnameThree":"' + cnameThree + '",'
				+ '"ccnameOne":"' + ccnameOne + '",'
				+ '"ccnameTwo":"' + ccnameTwo + '",'
				+ '"ccnameThree":"' + ccnameThree + '",'
				+ '"ccsurnameOne":"' + ccsurnameOne + '",'
				+ '"ccsurnameTwo":"' + ccsurnameTwo + '",'
				+ '"ccsurnameThree":"' + ccsurnameThree + '",'
				+ '"cemailOne":"' + cemailOne + '",'
				+ '"cemailTwo":"' + cemailTwo + '",'
				+ '"cemailThree":"' + cemailThree + '",'
				+ '"ctelnumberOne":"' + ctelnumberOne + '",'
				+ '"ctelnumberTwo":"' + ctelnumberTwo + '",'
				+ '"ctelnumberThree":"' + ctelnumberThree + '",'
				+ '"pwd":"' + pwd + '"}',
			contentType: "application/jsonp",
			success: function (httpResponse) {
				if (httpResponse.result.status == "success") {

					$("#spinner").hide();
					$("#confirmRegistration").show();
					$("#email").val(username);
					$("#password").val(confirm);
					$("#loginForm").submit();
					$("#spinner").hide();
					$("#confirmRegistration").show();
				} else if (parseInt(httpResponse.result.code) == 202) {
					$("#username").addClass("input-error");
					changeActive('1');
					$("#spinner").hide();
					$("#confirmRegistration").show();
					$("#email-error").show();
					$("#email-error").text("EMAIL ADDRESS IN USE");
				}
				else if (parseInt(httpResponse.result.code) == 400) {
					$("#register-error").show();
					$("#register-error").text("Oh no! Something went wrong we were unable to create your account please try again later");
					$("#spinner").hide();
					changeActive('4');
					$("#confirmRegistration").show();
				}
				else if (parseInt(httpResponse.result.code) == 502) {
					$("#register-error").show();
					$("#register-error").text("Oh no! Something went wrong we were unable to create your account please try again later");
					$("#spinner").hide();
					changeActive('4');
					$("#confirmRegistration").show();
				}
				$("#spinner").hide();
				$("#confirmRegistration").show();
			},
			error: function (httpResponse) {
				console.log(httpResponse);
				$("#invite_button").fadeIn();
				$("#inv_wait").fadeOut();
				$("#spinner").hide();
				$("#confirmRegistration").show();
			}
		});
	}

}

$(function () {
	changeActive('1');
});