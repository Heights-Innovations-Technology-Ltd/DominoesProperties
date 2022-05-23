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
            console.log(data);
          
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


$('btn-property').click(() => {
    $(".btn-login").html("Processing...").attr("disabled", !0);
    let t = false;
    var e = "";
    if (
        ($("#property-form")
            .find("input")
            .each(function () {
                $(this).prop("required") && ($(this).val() || ((t = !0), (name = $(this).attr("name")), (e += name + ", ")));
            })
        )
    )

        if (t) message("Validation error the following field are required " + e.substring(0, e.length - 2), 'error'), window.scrollTo(0, 0), $(".btn-property").attr("disabled", !1).html("Submit");

    if (!Number($("#price").val())) {
        message("Valid unit price is required!", 'error');
        $("#price").focus();
        return;
    }
    if ($("#logitude").val() != "" && !Number($("#logitude").val())) {
        message("Valid logitude is required!", 'error');
        $("#logitude").focus();
        return;
    }
    if ($("#latitude").val() != "" && !Number($("#latitude").val())) {
        message("Valid latitude is required!", 'error');
        $("#latitude").focus();
        return;
    }

    var params = {
        Name: $("#name").val().trim(),
        Location: $("#location").val(),
        Type: $("#type").val(),
        TotalUnits: $("#totalUnit").val(),
        UnitPrice: $("#price").val(),
        Status: $("#status").val(),
        Description: $("#description").val(),
        InterestRate: $("#inserestRate").val(),
        Longitude: $("#logitude").val(),
        Latitude: $("#latitude").val()
    };

    let xhr = new XMLHttpRequest();
    let url = "/get-customer";
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send(JSON.stringify(params);
        if (xhr.status != 200) {
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;

            if (JSON.parse(res).success) {
                console.log(data);
                message(data
                    , 'success');
                window.scrollTo(0, 0);
                $(".btn-property").html("Submit").attr("disabled", !1);
                $(".form-control").val("");
                
            } else {
                window.scrollTo(0, 0);
                message(data
                    , 'error');
                $(".btn-property").html("Submit").attr("disabled", !1);

            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-property").html("Submit").attr("disabled", !1);
    }
});

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

const AddProperty = () => {
    let xhr = new XMLHttpRequest();
    let url = "/create-property";
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
                propertiesTmp(data);
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        //$(".btn-login").html("Login").attr("disabled", !1);
    }
}

const GetProperties = () => {
    let xhr = new XMLHttpRequest();
    let url = "/get-properties";
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

            if (JSON.parse(res).success ) {
                console.log(data);
                propertiesTmp(data);
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        //$(".btn-login").html("Login").attr("disabled", !1);
    }
}

const PropertiesTmp = (data) => {
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-xl-4 col-md-6">
							<div class="single-featured-item">
								<div class="featured-img mb-0">
									<img src="~/images/featured/featured-1.jpg" alt="Image">
									<span>Rent</span>
								</div>
								<div class="featured-content style-three">
									<div class="d-flex justify-content-between">
										<h3>
											<a href="single-listing.html">House For Rent</a>
										</h3>
										<h3 class="price">$ 600,000</h3>
									</div>
									<p>
										<i class="ri-map-pin-fill"></i>
										77 Morris St. Ridgewood, NJ 67655
									</p>
									<ul>
										<li>
											<i class="ri-hotel-bed-fill"></i>
											5 Bed
										</li>
										<li>
											<i class="ri-wheelchair-fill"></i>
											5 Bath
										</li>
										<li>
											<i class="ri-ruler-2-line"></i>
											1000 Sqft
										</li>
									</ul>

									<a href="agents.html" class="agent-user">
										<img src="~/images/agents/agent-5.jpg" alt="Image">
										<span>By Darlene Small</span>
									</a>
								</div>
							</div>
						</div>`;

        $('#properties').append(res);
    })

    
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