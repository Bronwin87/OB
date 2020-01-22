//REGISTRATION VARIABLES

var firstname;
var lastname;
var username;
var pwd;
var confirm;
$(document).ready(function(){
    $("#spinner").hide();
     $("#firstname-error").hide();
     $("#lastname-error").hide();
     $("#email-error").hide();
     $("#password-error").hide();
     $("confirm-error").hide();
     $("register-error").hide()
});
function confirmPersonalDetails()
{
	var valid = true;
	$("#register").hide();

	$("#spinner").show();
	firstname = $("#firstname").val();
	if(firstname.length == 0)
	{
		$("#firstname").addClass("input-error");
		valid = false;
		$("#firstname-error").show();
		$("#firstname-error").text("FIRST NAME FIELD CANNOT BE EMPTY");
	}
	else
	{
		$("#firstname").removeClass("input-error");
		$("#firstname-error").hide();
	}

	lastname = $("#lastname").val();
	if(lastname.length == 0)
	{
		$("#lastname-error").show();
		$("#lastname").addClass("input-error");
		$("#lastname-error").text("LAST NAME FIELD CANNOT BE EMPTY");
		valid = false;
	}
	else
	{
		$("#lastname").removeClass("input-error");
		$("#lastname-error").hide();
	}

	username = $("#username").val();
	if(username.length == 0)
	{
		$("#email-error").show();
		$("#username").addClass("input-error");
		$("#email-error").text("EMAIL ADDRESS FIELD CANNOT BE EMPTY");
		valid = false;
	}
	else
	{
		$("#email-error").hide();
		$("#username").removeClass("input-error");
	}
	pwd = $("#pwd").val();
	if(pwd.length == 0)
	{
		$("#password-error").show();
		$("#pwd").addClass("input-error");
		$("#password-error").text("PASSWORD FIELD CANNOT BE EMPTY");
		valid = false;
	}
	else
	{
		$("#password-error").hide();
		$("#pwd").removeClass("input-error");

	}

	confirm = $("#confirm").val();
	if(confirm.length == 0)
	{
	
		$("#confirm").addClass("input-error");
		$("#confirm-error").show();
		$("#confirm-error").text("CONFIRM PASSWORD FIELD CANNOT BE EMPTY");
		valid = false;
		
	}
	else
	{
		$("#confirm-error").hide();
		$("#confirm").removeClass("input-error");
	}

	if(pwd !== confirm )
	{
		
		console.log("password match");
		$("#confirm-error").show();
		$("#password-error").show();
		$("#password-error").text("PASSWORD FIELD DOES NOT MATCH CONFIRM PASSWORD FIELD");
		$("#confirm-error").text("CONFIRM PASSWORD FIELD DOES NOT MATCH PASSWORD FIELD");
		$("#confirm").addClass("input-error");
		$("#pwd").addClass("input-error");
		valid = false;

	}else{
		// $("#confirm-error").hide();
		// $("#password-error").hide();
		
	}

	if(valid)
	{
		confirmRegistration();

	}else{
		$("#register").show();
		$("#spinner").hide();
	}
}

function confirmRegistration()
{
	$("#spinner").show();
	var url = cloudCodeURL + "individualRegistration";
	$.ajax({
		type: "POST",
		headers: headers,
		url: url,
		data: '{"firstname":"' + firstname + '",'
			 + '"lastname":"' + lastname + '",'
			 + '"username":"' + username + '",'
			 + '"confirm":"' + confirm + '",'
			 + '"pwd":"' + pwd + '"}',
		contentType: "application/jsonp",
		success: function (httpResponse)
		{
			$("#spinner").hide();
			$("#register").show();
			if (httpResponse.result.status == "success")
			{

				$("#registration-container").hide();
				//$("#isBusinessContainer").slideDown();
				$("#email").val(username);
				$("#password").val(pwd);
				$("#indvrgl").submit();
				$("#spinner").hide();
				$("#register").show();
			}
			else if(parseInt(httpResponse.result.code) == 202)
			{
				$("#username").addClass("input-error");
				$("#spinner").hide();
				$("#register").show();
				$("#email-error").show();
				$("#email-error").text("EMAIL ADDRESS IN USE");
			}
			else if(parseInt(httpResponse.result.code) == 400)
			{
				$("#register-error").show();
				$("#register-error").text("Oh no! Something went wrong we were unable to create your account please try again later");
				$("#spinner").hide();
				$("#register").show();
			}
		},
		error: function (httpResponse)
		{
			$("#spinner").hide();
			$("#register").show();
			console.log(httpResponse);
		}
	});
		// $("#spinner").hide();
		// $("#register").show();
}