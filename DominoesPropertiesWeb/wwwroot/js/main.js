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
                    message(res.Message + '. Kindly check your mail to activate your account', 'success');
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

$('.btn-adminlogin').click(() => {

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
    let url = "/authadmin";
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

const GetUserProfile = (mode) => {
    
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
                profile(data,mode);
               
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const profile = (data, mode) => {
    if (mode == "edit") {
        $('#firstname').val(data.firstName);
        $('#lastname').val(data.lastName);
        $('#email').val(data.email);
        $('#phone').val(data.phone);
        $('#address').val(data.address);
    } else {
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
    
}


const GetProperties = (type) => {
    if ($('#isAdmin').val() == "1") {

        $('.add-property').html(`<a href="/property/create" class="default-btn">Add New Property</a>`);
    }
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

            if (JSON.parse(res).success) {
                console.log(data);
                $('#property-count').html(data.length + ' Results Found')
                if (type == "admin") {
                    propertTmp(data);
                } else {
                    propertiesTmp(data);
                }
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const propertTmp = (data) => {
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-xl-4 col-md-6">
				    <div class="single-featured-item">
					    <div class="featured-img mb-0">
						    <img src="/images/featured/featured-1.jpg" alt="Image">
					    </div>
					    <div class="featured-content style-three">
						    <div class="d-flex justify-content-between">
							    <h3>
								    <a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">${x.name}</a>
							    </h3>
							    <h3 class="price">&#8358; ${formatToCurrency(x.unitPrice)}</h3>
						    </div>
						    <p>
							    <i class="ri-map-pin-fill"></i>
							    ${x.location}
						    </p>
						    <ul>
							   <li>
								    <i class="ri-hotel-bed-fill"></i>
								    ${x.description["bedroom"]} Bed
							    </li>
							    <li>
								    <i class="ri-wheelchair-fill"></i>
								    ${x.description["bathroom"]} Bath
							    </li>
							    <li>
								    <i class="ri-ruler-2-line"></i>
								    ${x.description["landSize"]} Sqft
							    </li>
                                <li>
								    <i class="ri-wheelchair-fill"></i>
								    ${x.description["toilet"]} Toilet
							    </li>
                                <li>
								    <i class="ri-building-2-fill"></i>
								    ${x.description["floorLevel"]} Floor
							    </li>
						    </ul>

						    <button type="button" onclick="propertyDetails('${x.uniqueId}')" class="btn btn-primary btn-sm">
								<span>View Property</span>
							</button>
					    </div>
				    </div>
			    </div>`;

        $('#properties').append(res);
    });
}

const propertiesTmp = (data) => {
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-lg-6 col-md-6">
									<div class="single-featured-item">
										<div class="featured-img mb-0">
											<img src="/images/featured/featured-2.jpg" alt="Image">
										</div>
										<div class="featured-content style-three">
											<div class="d-flex justify-content-between">
												<h3>
													<a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">${x.name} this property with long property name</a>
												</h3>
												 <h3 class="price">&#8358;${formatToCurrency(x.unitPrice)}</h3>
											</div>
											<p>
												<i class="ri-map-pin-fill"></i>
												${x.location}
											</p>
											<ul>
												<li>
													<i class="ri-hotel-bed-fill"></i>
													${x.description["bedroom"]} Bed
												</li>
												<li>
													<i class="ri-wheelchair-fill"></i>
													${x.description["bathroom"]} Bath
												</li>
												<li>
													<i class="ri-ruler-2-line"></i>
													${x.description["landSize"]} Sqft
												</li>
                                                <li>
													<i class="ri-wheelchair-fill"></i>
													${x.description["toilet"]} Toilet
												</li>
                                                <li>
													<i class="ri-building-2-fill"></i>
													${x.description["floorLevel"]} Floor
												</li>
											</ul>

											<button type="button" onclick="propertyDetails('${x.uniqueId}')" class="btn btn-primary btn-sm">
												<span>View Property</span>
											</button>
										</div>
									</div>
								</div>`;

        $('#properties').append(res);
    });
}

const propertyDetails = (id) => {
    if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
        $('#subscribeModal').modal('show');
        return;
    }

    location = '/Home/PropertyDetails/' + id;
}

const getSingleProperty = () => {
    if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
        location = '/Dashboard/Profile';
        return;
    }

    
    let urls = window.location.href.split("/");
    let id = urls[5];
    let xhr = new XMLHttpRequest();
    let url = `/single-property/${id}`;
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

                $('#name').html(data.name);
                $('#price').html("&#8358; " + formatToCurrency(data.unitPrice));
                $('#location').html(data.location);
                console.log(data);
            } else {
                
                window.scrollTo(0, 0);
                //console.log(data);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-activate").html("Activate").attr("disabled", !1);
    }
}


$('.btn-property').click(() => {
    $(".btn-property").html("Processing...").attr("disabled", !0);
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

    if ((!Number($("#price").val())) && $("#price").val() != '') {
        $(".btn-property").html("Submit").attr("disabled", !1);
        message("Invalid price supply, kindly check and try again!", 'error');
        $('#price').focus();
        return;
    }

    //if ((!Number(Number($("#logitutde").val()))) && $("#logitude").val() != '') {
    //    $(".btn-property").html("Submit").attr("disabled", !1);
    //    message("Invalid logitude supply, kindly check and try again!", 'error');
    //    $('#logitude').focus();
    //    return;
    //}

    //if ((!Number($("#latitude").val())) && $("#latitude").val() != '') {
    //    $(".btn-Submit").html("Submit").attr("disabled", !1);
    //    message("Invalid latitude supply, kindly check and try again!", 'error');
    //    $('#latitude').focus();
    //    return;
    //}
    

    var params = {
        Name: $("#name").val().trim(),
        Location: $("#location").val(),
        Type: $("#types").val(),
        UnitPrice: Number($("#price").val()),
        Status: $("#status").val(),
        UnitAvailable: Number($("#unit").val()),
        Description: {
            Bathroom: Number($("#bathroom").val()),
            Toilet: Number($("#toilet").val()),
            FloorLevel: Number($("#floorLevel").val()),
            Bedroom: Number($("#bedRoom").val()),
            LandSize: $("#landSize").val(),
            AirConditioned: $("#airConditioned").is(":checked") ? 1 : 0,
            Refrigerator: $("#refrigerator").is(":checked") ? 1 : 0,
            Parking: $("#parking").is(":checked") ? 1 : 0,
            SwimmingPool: $("#swimmingPool").is(":checked") ? 1 : 0,
            Laundry: $("#laundry").is(":checked") ? 1 : 0,
            Gym: $("#gym").is(":checked") ? 1 : 0,
            SecurityGuard: $("#securityGuard").is(":checked") ? 1 : 0,
            Fireplace: $("#fireplace").is(":checked") ? 1 : 0,
            Basement: $("#basement").is(":checked") ? 1 : 0,
        },
        InterestRate: Number($("#interest").val()),
        Longitude: Number($("#logitude").val()),
        Latitude: Number($("#latitude").val())

    };

    console.log(params);
    let xhr = new XMLHttpRequest();
    let url = "/create-property";
    xhr.open('POST', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {
        xhr.send(JSON.stringify(params));
        if (xhr.status != 200) {
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).Data;
            console.log(res);
            if (JSON.parse(res).Success) {
                window.scrollTo(0, 0);
                message(JSON.parse(res).Message, "success");
                $('.form-control').val('');
                $(".btn-property").html("Submit").attr("disabled", !1);

                setTimeout(() => {
                    location.reload();
                }, 2000);
            } else {
                $(".btn-property").html("Submit").attr("disabled", !1);
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-property").html("Submit").attr("disabled", !1);
    }
});


const GetInvestments = () => {
    let xhr = new XMLHttpRequest();
    let url = "/get-investments";
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
                investmentTmp(data);
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-login").html("Login").attr("disabled", !1);
    }
}

const GetPropertyTypes = () => {
    let xhr = new XMLHttpRequest();
    let url = "/get-property-types";
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
                $('#types').html(`<option lable="&nbsp">Select Type </option>`);
                data.forEach(x => {
                    $('#types').append(`<option value="${x.id}">${x.name} </option>`);
                });
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-login").html("Login").attr("disabled", !1);
    }
}

$('.btn-activate').click(() => {

    let urls = window.location.href.split("/");
    let token = urls[5].split("?")[0];

    if (!token.includes("AT")) {
        return;
    }
    $(".btn-activate").html("Processing...").attr("disabled", !0);

    let params = {
        "token": token
    }
    let xhr = new XMLHttpRequest();
    let url = "/activate";
    xhr.open('PUT', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send(JSON.stringify(params));
        if (xhr.status != 200) {
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).Data;
            var messages = JSON.parse(res).Message;

            if (JSON.parse(res).Success) {
                $('.btn-activate').css('display', 'none').removeClass('.btn-activate');
                $('.active-title').html(`${messages}. Kindly proceed to login into your account`);
                setTimeout(() => {
                    location = '/Home/signin';
                }, 3000);
                console.log(data);
            } else {
                $(".btn-activate").html("Activate").attr("disabled", !1);
                window.scrollTo(0, 0);
                message(messages, 'error');
                console.log(data);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-activate").html("Activate").attr("disabled", !1);
    }
});

$('#property-link').click(() => {
    if ($('#refId').val() == null || $('#refId').val() == "") {
        location = "/Home/signin";
        return;
    }
    location = "/Home/properties";
});

$('.btn-subscribe').click(() => {
    $(".btn-subscribe").html("Processing...").attr("disabled", !0);
    let xhr = new XMLHttpRequest();
    let url = "/subscribe";
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send();
        if (xhr.status != 200) {
            $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).Data;

            if (JSON.parse(res).Success) {
                location = data;
                //window.open(data, "Dominoes Society", "status=1,toolbar=1");
                $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
                console.log(data);
            } else {
                $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
                window.scrollTo(0, 0);
                message(data, 'error');
                console.log(data);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
    }
});

$('.btn-verify').click(() => {

    let urls = window.location.href.split("/");
    let token = urls[5].split("?")[0];
    console.log(token);

    $(".btn-verify").html("Processing...").attr("disabled", !0);

    
    let xhr = new XMLHttpRequest();
    let url = "/verifypayment/" + token;
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send();
        if (xhr.status != 200) {
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            console.log(res);
            var data = JSON.parse(res).Data;

            if (JSON.parse(res).Success) {
                $('.btn-verify').css('display', 'none').removeClass('.btn-verify');
                $('.active-title').html(`${messages}. Kindly proceed to login into your account`);
               
                console.log(data);
            } else {
                $(".btn-verify").html("Verify").attr("disabled", !1);
                window.scrollTo(0, 0);
                $('.active-title').html(`<span class="text-danger">Payment verification failed, kindly contact administrator</span>`);
                console.log(data);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-verify").html("Verify").attr("disabled", !1);
    }
});

$('#signout').click(() => {
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
   
});


const message = (msg, _class) => $('#msg').html(`<div class="alert alert-${_class == "error" ? 'danger' : 'success'} alert-dismissible fade show" role="alert">
							${msg}
						</div>`);

function formatToCurrency(amount) {
    return (amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}