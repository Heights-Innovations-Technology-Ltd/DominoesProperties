$(".btn-register").on("click", function () {

    if ($('.catch_me').val() != "") {
        return;
    }

    $(".btn-register").html("Processing...").attr("disabled", !0);
    let t = false;
    var e = "";
    if (
        ($("#user-form")
            .find("input")
            .each(function () {
                $(this).prop("required") && ($(this).val() || ((t = !0), (name = $(this).attr("name")), (e += name + ", ")));
            }),
            $("#password").val() != $("#confirm_password").val())
    )
        return message("Password do not match!", 'error'), $("#confirm_password").val(""), $("#confirm_password").focus(), window.scrollTo(0, 0), void $(".btn-register").html("Register").attr("disabled", !1);
    if (t) message("Validation error the following field are required " + e.substring(0, e.length - 2),  'error'), window.scrollTo(0, 0), $(".btn-register").attr("disabled", !1).html("Register");
    else {
        var a = {
            FirstName: $("#firstName").val(),
            LastName: $("#lastName").val(),
            Phone: $("#phone").val(),
            Email: $("#email").val(),
            Address: $("#address").val(),
            Password: $("#password").val(),
            ConfirmPassword: $("#confirm_password").val()
        };

        $.ajax({
            type: "post",
            url: "/createuser",
            headers: { "Content-Type": "application/json" },
            data: JSON.stringify(a),
            success: function (t) {
                var res = JSON.parse(t);
                if (res.Success) {
                    message(res.Message, 'success');
                    $(".form-control").val(""),
                    $("#firstName").focus(),
                        window.scrollTo(0, 0),
                        $(".btn-register").html("Register").attr("disabled", !1),

                        a = {};

                } else {
                   
                    message(res.Data
                        , 'error'),
                        window.scrollTo(0, 0),
                        $(".btn-register").html("Register").attr("disabled", !1);
                    a = {};
                }

            },
            error: function (t) {
                if (400 == t.status) return //alert("check your supply value and try again!"),
                void $(".btn-register").html("Register").attr("disabled", !1);
                $(".btn-register").html("Register").attr("disabled", !1);
            },
        });
    }
});

$('.btn-login').click(() => {

    if ($('.catch_me').val() != "") {
        return;
    }

    $(".btn-login").html("Processing...").attr("disabled", !0);
    let t = false;
    var e = "";
    if (
        ($("#login-form")
            .find("input")
            .each(function () {
                $(this).prop("required") && ($(this).val() || ((t = !0), (name = $(this).attr("name")), (e += name + ", ")));
            })
        )
    )
        
    if (t) message("Validation error the following field are required " + e.substring(0, e.length - 2), 'error'), window.scrollTo(0, 0), $(".btn-login").attr("disabled", !1).html("Login");

    var params = {
        Email: $("#logEmail").val().trim(),
        Password: $("#logPassword").val()
    };
    let xhr = new XMLHttpRequest();
    let url = "/authuser";
    xhr.open('POST', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send(JSON.stringify(params));
        if (xhr.status != 200) {
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;
            console.log(JSON.parse(res).Data);
          
            if (JSON.parse(res).success) {
                console.log(data);
                location = "/Dashboard";
                $(".btn-login").html("Login").attr("disabled", !1);
                $(".form-control").val("");
            } else {
                window.scrollTo(0, 0);
                message(data
                    , 'error');
                $(".btn-login").html("Login").attr("disabled", !1);

            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-login").html("Login").attr("disabled", !1);
    }
});


const customerDetails = () => {
    let xhr = new XMLHttpRequest();
    let url = "/get-customer";
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send();
        if (xhr.status != 200) {
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;

            if (JSON.parse(res).success) {
                console.log(data);
                profile(data);
                const pageProfile = this.href.substring(this.href.lastIndexOf('/') + 1);
                console.log( "Hello" + (this.href.substring(this.href.lastIndexOf('/') + 1)));
                console.log(pageProfile);
                if (pageProfile) {
                    profile(data)
                } else {
                    console.log('not profile');
                }
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-login").html("Login").attr("disabled", !1);
    }
}

const profile = (data) => {
    $('#profile').html(`
        <li>
		    <span id="firstname">First Name:</span>
		    ${data.firstName}
	    </li>
	    <li>
		    <span id="lastname">Last Name:</span>
		   ${data.lastName}
	    </li>
	   
	    <li>
		    <span id="email">Email:</span>
		   ${data.email}
	    </li>
	    <li>
		    <span>Phone:</span>
            ${data.phone}
	    </li>
	    <li>
		    <span id="address">Address:</span>
		    ${data.address};
	    </li>
        
    `);
}


$('#signout').click(() => {
    if (confirm("Are you sure you want to logout?")) {
        let xhr = new XMLHttpRequest();
        let url = "/logout";
        xhr.open('GET', url, false);
        xhr.setRequestHeader("content-type", "application/json");
        xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
        try {

            xhr.send();
            if (xhr.status != 200) {
                //alert('Something went wrong try again!');
            } else {
                var res = JSON.parse(xhr.responseText);
                if (res) {
                    location = '/';
                }
            }
        } catch (err) { // instead of onerror
            //alert("Request failed");
        }
    }
   
});


const message = (msg, _class) => $('#msg').html(`<div class="alert alert-${_class == "error" ? 'danger' : 'success'} alert-dismissible fade show" role="alert">
							${msg}
						</div>`);