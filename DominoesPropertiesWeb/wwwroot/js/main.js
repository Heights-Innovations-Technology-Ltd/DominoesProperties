﻿$(".btn-register").on("click", function () {

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
                if (res.success) {
                   // message(res.Message + '. Kindly check your mail to activate your account', 'success');
                    Swal.fire(
                        'Good job!',
                        res.message + '. Kindly check your mail to activate your account',
                        'success'
                    )
                    $(".form-control").val(""),
                    $("#firstName").focus(),
                        window.scrollTo(0, 0),
                        $(".btn-register").html("Register").attr("disabled", !1),

                        a = {};

                } else {
                    Swal.fire(
                        'Oops!',
                        res.data,
                        'error'
                    );
                    //message(res.data
                    //    , 'error'),
                    window.scrollTo(0, 0);
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
    $(".btn-login").html("Processing...").attr("disabled", !0);
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
            if (JSON.parse(res).success) {
                var propertyId = sessionStorage.getItem("redirectToPropertyDetail");
                if (propertyId != null) {
                    propertyDetails(propertyId);
                    sessionStorage.removeItem("redirectToPropertyDetail");
                    return;
                }
                window.location.replace('/dashboard');
                $(".btn-login").html("Login").attr("disabled", !1);
                $(".form-control").val("");
            } else {
                window.scrollTo(0, 0);
                Swal.fire(
                    'Oops!',
                    data,
                    'error'
                );
                //message(data
                //    , 'error');
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

    $(".btn-adminlogin").html("Processing...").attr("disabled", !0);
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
            if (JSON.parse(res).success) {
                window.location.replace('/dashboard');
                $(".btn-adminlogin").html("Login").attr("disabled", !1);
                $(".form-control").val("");
            } else {
                window.scrollTo(0, 0);
                Swal.fire(
                    'Oops!',
                    data,
                    'error'
                );
                $(".btn-adminlogin").html("Login").attr("disabled", !1);

            }
        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-adminlogin").html("Login").attr("disabled", !1);
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
                profile(data,mode);
               
            } else {
                window.scrollTo(0, 0);
            }
        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const GetUserDashboard = () => {
    let xhr = new XMLHttpRequest();
    let url = "/customer-dashboard";
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
                $('.total-investment').text(data.TotalInvestment);
                $('.active-investment').text(data.ActiveInvestment);
                $('.closed-investment').text(data.ClosedInvestment);
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const GetUAdminDashboard = () => {
    let xhr = new XMLHttpRequest();
    let url = "/admin-dashboard";
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
                $('.total-property').text(data.Properties);
                $('.active-property').text(data.ActiveProperties);
                $('.total-investment').text(data.TotalInvestment);
                $('.active-investment').text(data.Investments);
                $('.total-customer').text(data.Customers);
                $('.inactive-customer').text(data.InactiveCustomer);
                $('.payment-due').text(data.DuePayment);
               
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
        $('#account').val(data.accountNumber);
        $('#bank').val(data.bankName);
        $('.profile-pic').attr("src", data.passportUrl)
    } else if (mode == "dashboard") {
        $('#walletBalance').html('&#8358; ' + currency(data.walletBalance));
    } else {
        $('.fullName').text(data.firstName + " " + data.lastName);
        $('.walletId').html("Wallet: ( <strong>" + data.walletId + "</strong> )");
        $('.walletBalance').html('&#8358; ' + currency(data.walletBalance));
        $("#passport").attr("src", data.passportUrl);
        $('#profile').html(`
            <div class="card-body">
				<div class="row">
					<div class="col-sm-3">
					<h6 class="mb-0">Full Name</h6>
					</div>
					<div class="col-sm-9">
					<p class="text-muted mb-0"> ${data.firstName} ${data.lastName}</p>
					</div>
				</div>
				<hr>
				<div class="row">
					<div class="col-sm-3">
					<h6 class="mb-0">Email</h6>
					</div>
					<div class="col-sm-9">
					<p class="text-muted mb-0"> ${data.email}</p>
					</div>
				</div>
				<hr>
				<div class="row">
					<div class="col-sm-3">
					<h6 class="mb-0">Phone</h6>
					</div>
					<div class="col-sm-9">
					<p class="text-muted mb-0"> ${data.phone}</p>
					</div>
				</div>
				<hr>
				<div class="row">
					<div class="col-sm-3">
					<h6 class="mb-0">Account Number</h6>
					</div>
					<div class="col-sm-9">
					<p class="text-muted mb-0">${data.accountNumber == null ? '--' : data.accountNumber}</p>
					</div>
				</div>
				<hr>
                <div class="row">
					<div class="col-sm-3">
					<h6 class="mb-0">Bank Name</h6>
					</div>
					<div class="col-sm-9">
					<p class="text-muted mb-0">${data.bankName == null ? '--' : data.bankName}</p>
					</div>
				</div>
				<hr>
				<div class="row">
					<div class="col-sm-3">
					<h6 class="mb-0">Address</h6>
					</div>
					<div class="col-sm-9">
					<p class="text-muted mb-0"> ${data.address}</p>
					</div>
				</div>
			</div>
    `);
    }
    
}


const GetProperties = (type) => {
    if ($('#isAdmin').val() == "1") {

        $('.add-property').html(`<a href="/property/create" class="default-btn "><i class="fa fa-plus"></i> New Property</a>`);
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
                $('#property-count').html(data.length + ' Results Found');
                
                if (type == "admin") {
                    adminPropertTmp(data);
                } else if (type == "landing") {
                    LandingPagePropertyTmp(data.slice(0, 7));
                    var i = getRandom(data, 1);
                        $('#property-img').html(`<img src="${i[0].data.length > 0 ? i[0].data[0].url  : '/images/why-choose-img.jpg'}" id="property-image" style="height:402px; width:636px"  alt="Image">`);
                    if (i[0].data.length > 0) {
                    }
                    $('.property').html(`
                            <div class="d-flex justify-content-between">
							    <a href="javascript:void(0)" onclick="propertyDetails('${i[0].uniqueId}')">
								    <h3>${i[0].name}</h3>
							    </a>
							   <h3 class="price">&#8358;${formatToCurrency(i[0].unitPrice)}</h3>
						    </div>
						    <p>
							    <i class="ri-map-pin-fill"></i>
							   ${i[0].location}
						    </p>
						    <ul class="feature-list">
							    <li>
									<i class="ri-hotel-bed-fill"></i>
									${i[0].description["bedroom"]} Bed
								</li>
								<li>
									<i class="ri-wheelchair-fill"></i>
									${i[0].description["bathroom"]} Bath
								</li>
								<li>
									<i class="ri-ruler-2-line"></i>
									${i[0].description["landSize"]} Sqft
								</li>
                                <li>
									<i class="ri-wheelchair-fill"></i>
									${i[0].description["toilet"]} Toilet
								</li>
                                <li>
									<i class="ri-building-2-fill"></i>
									${i[0].description["floorLevel"]} Floor
								</li>
						    </ul>
                       `);
                } else if (type == "investment") {
                    adminInvestmentTmp(data);
                }
                else {
                    propertiesTmp(data);
                }
            } else {
                if (type == "admin" && data.length == 0) {
                    $('#properties').html("<p class='text-center'>Empty property found, kindly proceed to create one</p>");
                    } 
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}


const filterProperty = () => {
    let urls = window.location.href.split("/");
    let currentUrl = urls[3];
    if (currentUrl == "") {
        sessionStorage.setItem("landingFilter", $(".location").val())
        window.location.replace("Home/properties");
        return;
    }
    var location = sessionStorage.getItem("landingFilter");
    var params = {
        Category: $("#types").val(),
        Bathroom: Number($("#bathroom").val()),
        Toilet: Number($("#toilet").val()),
        FloorLevel: Number($("#floor").val()),
        Bedroom: Number($("#bedroom").val()),
        AirConditioned: $("#airconditioned").is(":checked") ? 1 : 0,
        Refrigerator: $("#refrigerator").is(":checked") ? 1 : 0,
        Parking: $("#parking").is(":checked") ? 1 : 0,
        SwimmingPool: $("#swimmingpool").is(":checked") ? 1 : 0,
        Laundry: $("#laundry").is(":checked") ? 1 : 0,
        Gym: $("#gym").is(":checked") ? 1 : 0,
        SecurityGuard: $("#securityguard").is(":checked") ? 1 : 0,
        Fireplace: $("#fireplace").is(":checked") ? 1 : 0,
        Basement: $("#basement").is(":checked") ? 1 : 0,
        MinPrice: minPrice,
        MaxPrice: maxPrice,
        Location: location != null ? sessionStorage.getItem("landingFilter") : ""
    };

    let xhr = new XMLHttpRequest();
    let url = "/filter-property";
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
            if (JSON.parse(res).success) {
                $('#property-count').html(data.length + ' Results Found')
                propertiesTmp(data);
                sessionStorage.removeItem("landingFilter");

            } else {
                Swal.fire(
                    'Oops!',
                    'No data match your request',
                    'error'
                );
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

//$('.src-btn').on('click', (e) => {
//    e.preventDefault();
//    filterProperty();
//});

const adminPropertTmp = (data) => {
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-lg-4 col-md-4">
									<div class="single-featured-item">
 <a href="/property/viewproperty/${x.uniqueId}">
										<div class="featured-img mb-0">
											<img src="${x.data.length > 0 ? x.data[0].url : '/images/featured/featured-2.jpg'}" style="width: 100%;
    height: 350px;" alt="Image">
										</div>
										<div class="featured-content style-three">
											<div class="justify-content-between">
												<h3>
													<a href="/property/viewproperty/${x.uniqueId}">${x.name}</a>
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
										</div>
</a>
									</div>
								</div>`;

        $('#properties').append(res);
    });
}

const adminInvestmentTmp = (data) => {
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-lg-4 col-md-4">
									<div class="single-featured-item">
                                    <a href="/investment/viewinvestment/${x.uniqueId}">
										<div class="featured-img mb-0">
											<img src="${x.data.length > 0 ? x.data[0].url : '/images/featured/featured-2.jpg'}" alt="Image">
										</div>
										<div class="featured-content style-three">
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <h3 style="font-size:14px; font-weight:normal;">
													<a href="/investment/viewinvestment/${x.uniqueId}">${x.name}</a>
												</h3>
                                                </div>
                                                <div class="col-md-6">
                                                    <h3 class="price float-end">&#8358;${formatToCurrency(x.unitPrice)}</h3>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                   <h3 style="font-size:14px; font-weight:normal;">Total Units</h3>
                                                </div>
                                                <div class="col-md-6">
                                                    <h3 class="float-end" style="font-size:14px; font-weight:normal;">${x.totalUnits}</h3>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                   <h3 style="font-size:14px; font-weight:normal;">Unit Sold</h3>
                                                </div>
                                                <div class="col-md-6">
                                                    <h3 class="float-end" style="font-size:14px; font-weight:normal;">${x.unitSold}</h3>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                   <h3 style="font-size:14px; font-weight:normal;">Available Unit</h3>
                                                </div>
                                                <div class="col-md-6">
                                                    <h3 class="float-end" style="font-size:14px; font-weight:normal;">${x.unitAvailable}</h3>
                                                </div>
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
										</div>
</a>
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
 <a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">
										<div class="featured-img mb-0">
											<img src="${x.data.length > 0 ? x.data[0].url : '/images/featured/featured-2.jpg'}" style="width: 100%; height: 350px;" alt="Image">
										</div>
										<div class="featured-content style-three">
											<div class="justify-content-between">
												<h3>
													<a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">${x.name}</a>
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
										</div>
</a>
									</div>
								</div>`;

        $('#properties').append(res);
    });
}

const LandingPagePropertyTmp = (data) => {
   
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-lg-4 col-md-6 ">
									<div class="single-featured-item">
                                        <a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">
										    <div class="featured-img mb-0">
											   <img src="${x.data.length > 0 ? x.data[0].url : '/images/featured/featured-2.jpg'}" style="width: 100%; height: 350px;" alt="Image">
										    </div>
										    <div class="featured-content style-three">
											    <div class=" justify-content-between">
												    <h3>
													    <a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">${x.name}</a>
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
										    </div>
                                        </a>
									</div>
								</div>`;

        $('#properties').append(res);
    });
    if (data.length > 8) {
        $('#properties').append(`<div class="col-lg-12 text-center">
							<a href="/properties" class="default-btn">
								View All Properties
							</a>
						</div>`);
    }
}

const propertyDetails = (id) => {

    if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
        Swal.fire({
            icon: 'info',
            title: 'Oops...',
            text: 'Property details can only be view by subscribed users, kindly subscribe to get full access ',
            footer: `<a href="javascript:void(0)" class="default-btn btn-subscribe" onclick="onSubscribe()">Subcribe Now</a>`
        }).then(() => {
            
            //$('.btn-subscribe').trigger('click');
        })
        return;
    }
    window.location.replace('/Home/PropertyDetails/' + id);
}

$('.edit-property').click(() => {
    let urls = window.location.href.split("/");
    let id = urls[5];
    let url = `/property/edit/${id}`;
    window.location.replace(url);
});

let singleData;
const getSingleProperty = () => {

    if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
        location = '/Dashboard/Profile';
        return;
    }

    if ($('#refId').val() == null || $('#refId').val() == "") {
        Swal.fire({
            icon: 'info',
            title: 'Oops...',
            showCancelButton: false,
            showConfirmButton: false,
            allowOutsideClick: false,
            text: 'Please, kindly login to have full access',
            footer: `<a href="javascript:void(0)" class="default-btn" onclick="propertyDetailLogin()">Login</a>`
        });
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
                singleData = data;
                if (data.data.Cover.length > 0) {
                    $('.properties-bg-img').css('background-image', "url('" + data.data.Cover[0] + "')");
                }
              
                
                if (data.data.Images.length > 0) {
                    //$('.shorting').html(`
                       
                    //`);
                    $('.gallery-title').removeClass('d-none');
                    data.data.Images.forEach(x => {
                        $('.property-img').append(`
                        <div class="col-lg-4 col-sm-6">
							<div class="single-gallery">
								<img src="${x}" alt="Image" style="width: 100%; height: 200px;">
								<a href="${x}">
									<i class="ri-eye-line"></i>
								</a>
							</div>
						</div>
                    `);
                    })
                }

                $('#name').html(data.name);
                $('#price').html("&#8358; " + formatToCurrency(data.unitPrice));
                $('#location').html(data.location);
                $('.closeDate').html("Available Units: <strong>" + data.unitAvailable +"</strong>");
                $('.property-feature').html(`<li>
												<i class="ri-hotel-bed-fill"></i>
												${data.description["bedroom"]} Bed
											</li>
											<li>
												<i class="ri-wheelchair-fill"></i>
												${data.description["bathroom"]} Bath
											</li>
											<li>
												<i class="ri-ruler-2-line"></i>
												${data.description["landSize"]} Sqft
											</li>
                                            <li>
												<i class="ri-wheelchair-fill"></i>
												${data.description["toilet"]} Toilet
											</li>
                                            <li>
												<i class="ri-building-2-fill"></i>
												${data.description["floorLevel"]} Floor
											</li>`);
                data.summary != "" ? 
                $('.summary').html(`
                    <h3>Property Description</h3>
					<p class="summary">${data.summary}</p>`) : '';

                $('.description').html(`
                    <div class="col-lg-3 col-sm-6">
					    <ul>
						    <li>
							    <i class="${data.description['airConditioned'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Air Conditioned
						    </li>
						    <li>
							    <i class="${data.description['laundry'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Laundry
						    </li>
						    <li>
							    <i class="${data.description['refrigerator'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Refrigerator
						    </li>
					    </ul>
				    </div>
				   
				    <div class="col-lg-3 col-sm-6">
					    <ul>
						    <li>
							    <i class="${data.description['parking'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Parking
						    </li>
						    <li>
							    <i class="${data.description['gym'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Fitness Gym
						    </li>
						    <li>
							    <i class="${data.description['fireplace'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Fireplace
						    </li>
					    </ul>
				    </div>
				    <div class="col-lg-3 col-sm-6">
					    <ul>
						    <li>
							    <i class="${data.description['swimmingPool'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Swimming Pool
						    </li>
						    <li>
							    <i class="${data.description['securityGuard'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Security Garage
						    </li>
						    <li>
							    <i class="${data.description['basement'] ? 'ri-checkbox-line' : 'ri-checkbox-blank-line'}"></i>
							    Basement
						    </li>
					    </ul>
				    </div>
                `);
                $("#bathroom").val(data.description['bathroom']);
                $("#toilet").val(data.description['toilet']);
                $("#floorLevel").val(data.description['toilet']);
                $("#bedRoom").val(data.description['bedroom']);
                $("#landSize").val(data.description['landSize']);
                if (data.description['airConditioned']) $("#airConditioned").attr('checked', 'checked');
                if (data.description['refrigerator']) $("#refrigerator").attr('checked', 'checked');
                if (data.description['parking']) $("#parking").attr('checked', 'checked');
                if (data.description['swimmingPool']) $("#swimmingPool").attr('checked', 'checked');
                if (data.description['laundry']) $("#laundry").attr('checked', 'checked');
                if (data.description['gym']) $("#gym").attr('checked', 'checked');
                if (data.description['securityGuard']) $("#securityGuard").attr('checked', 'checked');
                if (data.description['fireplace']) $("#fireplace").attr('checked', 'checked');
                if (data.description['basement']) $("#basement").attr('checked', 'checked');

                if (data.videoLink != null) {
                    if (data.data.Images.length > 0) {
                        var i = getRandom(data.data.Images, 1);
                        $('.video-img').removeAttr('src');
                        $('.video-img').attr('src', i[0]);
                    }
                    
                    $('.video-url').attr('href', data.videoLink);
                }
                if (data.data["Document"].length > 0) {
                $('.floor-plan').html(`<h3>Floor Plans</h3>
						<img src="${data.data["Document"]}" alt="Image">`);
                }

                //checkout

               
            } else {
                
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-activate").html("Activate").attr("disabled", !1);
    }
}

const propertyDetailLogin = () => {
    let urls = window.location.href.split("/");
    let id = urls[5];
    sessionStorage.setItem("redirectToPropertyDetail", id);
    window.location.replace("/Home/Signin");
}

const calUnit =  () => {
    $('.unit').text($('#unit').val());
    let price = Number($('#price').text().replace(/[^0-9\.-]+/g, "").replace("₦", ""));

    $('.total').html("&#8358; " + formatToCurrency(price * $('#unit').val()));
    $('.groundTotal').html("&#8358; " + formatToCurrency(price * $('#unit').val()));
    var projectYield = (singleData.targetYield / 100) * price;
    $('.yield').html(projectYield * $('#unit').val());

};

function openModal() {
    Swal.fire({
        template: '#my-template'
    });

    $('.swal2-styled').css('display', 'none');
    //$('#propertyName').text(singleData.name);
    $('#checkoutPrice').html("&#8358; " + formatToCurrency(singleData.unitPrice) + " / Unit");
    $('.checkoutPrice').html(formatToCurrency(singleData.unitPrice));
    $('#availableUnit').html("Available Unit: " + singleData.unitAvailable);
    //$('#closeDate').html("Investment End On " + moment(data.closingDate).format('MMMM Do YYYY'));
    $('.total').html("&#8358; " + formatToCurrency(singleData.unitPrice));
    let price = Number($('#price').text().replace(/[^0-9\.-]+/g, "").replace("₦", ""));
    $('.groundTotal').html("&#8358; " + formatToCurrency(price * $('#unit').val()));
    $('.interest').html(singleData.targetYield + "<sup>%</sup>");
    $('.yield').html((singleData.targetYield / 100) * price);
}


const editSingleProperty = () => {

    if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
        window.location.replace('/Dashboard/Profile/');
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
               
                $("#name").val(data.name);
                $("#location").val(data.location);
                setSelectedOption("#types", data.type);
                setSelectedOption("#status", data.status);
                $("#price").val(data.unitPrice);
                $("#status").val();
                $("#unit").val(data.totalUnits);
                $("#maxCustomerUnit").val(data.maxUnitPerCustomer);
                $("#closingDate").val(data.closingDate);
                $("#account").val(data.accountNumber);
                $("#bank").val(data.bankName);
                setSelectedOption("#allowSharing", data.allowSharing);
                setSelectedOption("#minimumSharing", data.minimumSharingPercentage);

                $("#bathroom").val(data.description['bathroom']);
                $("#toilet").val(data.description['toilet']);
                $("#floorLevel").val(data.description['toilet']);
                $("#bedRoom").val(data.description['bedroom']);
                $("#landSize").val(data.description['landSize']);
                if (data.description['airConditioned']) $("#airConditioned").attr('checked', 'checked');
                if (data.description['refrigerator']) $("#refrigerator").attr('checked', 'checked');
                if (data.description['parking']) $("#parking").attr('checked', 'checked');
                if (data.description['swimmingPool']) $("#swimmingPool").attr('checked', 'checked');
                if (data.description['laundry']) $("#laundry").attr('checked', 'checked');
                if (data.description['gym']) $("#gym").attr('checked', 'checked');
                if (data.description['securityGuard']) $("#securityGuard").attr('checked', 'checked');
                if (data.description['fireplace']) $("#fireplace").attr('checked', 'checked');
                if (data.description['basement'])$("#basement").attr('checked', 'checked');
                $("#interest").val(data.interestRate);
                $("#logitude").val(data.longitude);
                $("#latitude").val(data.latitude);
                $("#description").val(data.summary);
            } else {

                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-activate").html("Activate").attr("disabled", !1);
    }
}

$(document).ready(function () {
    $('.btn-property').click((e) => {
        e.preventDefault();
        const confirmPropertyUpdate = Swal.mixin({
            customClass: {
                confirmButton: 'btn btn-success mx-2',
                cancelButton: 'btn btn-danger'
            },
            buttonsStyling: false
        })

        confirmPropertyUpdate.fire({
            title: 'Are you sure?',
            text: "To create this property!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, create it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
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
                    //InterestRate: Number($("#interest").val()),
                    TargetYield: Number($("#targetYield").val()),
                    Longitude: Number($("#logitude").val()),
                    Latitude: Number($("#latitude").val()),
                    Summary: $("#description").val(),
                    Account: $("#account").val(),
                    Bank: $("#bank").val(),
                    VideoLink: $("#videoLink").val(),
                    AllowSharing: Number($("#allowSharing").val()),
                    MinimumSharing: $("#minimumSharing").val()

                };

                let xhr = new XMLHttpRequest();
                let url = "/create-property";
                xhr.open('POST', url, false);
                xhr.setRequestHeader("content-type", "application/json");
                xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
                try {
                    xhr.send(JSON.stringify(params));
                    if (xhr.status != 200) {
                        // alert('Something went wrong try again!');
                        $(".btn-property").html("Submit").attr("disabled", !1);
                    } else {
                        var res = JSON.parse(xhr.responseText);
                        var data = JSON.parse(res).data;
                        var message = JSON.parse(res).message;
                        if (JSON.parse(res).success) {
                            window.scrollTo(0, 0);
                            $('.form-control').val('');
                            $(".btn-property").html("Submit").attr("disabled", !1);
                            Swal.fire(
                                'Good job!',
                                message,
                                'success'
                            ).then(() => {
                                location.reload();
                            });
                        } else {
                            var err = JSON.parse(res).message;
                            Swal.fire(
                                'Oops!',
                                err == "401" ? "You don't have permission to perform this action" : "Something went wrong, admin has been contacted",
                                'error'
                            );
                            $(".btn-property").html("Submit").attr("disabled", !1);
                            window.scrollTo(0, 0);
                        }

                    }
                } catch (err) { // instead of onerror
                    //alert("Request failed");
                }
            } else if (
                /* Read more about handling dismissals below */
                result.dismiss === Swal.DismissReason.cancel
            ) {
                confirmPropertyUpdate.fire(
                    'Cancelled',
                    'No changes was made :)',
                    'error'
                )
            }
        });
    });
})


$('.btn-update-property').click(() => {

    const confirmPropertyUpdate = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mx-2',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    })

    confirmPropertyUpdate.fire({
        title: 'Are you sure?',
        text: "To make changes to this property data!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, continue!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".btn-update-property").html("Processing...").attr("disabled", !0);
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

                if (t) message("Validation error the following field are required " + e.substring(0, e.length - 2), 'error'), window.scrollTo(0, 0), $(".btn-update-property").attr("disabled", !1).html("Submit");

            if ((!Number($("#price").val())) && $("#price").val() != '') {
                $(".btn-update-property").html("Submit").attr("disabled", !1);
                message("Invalid price supply, kindly check and try again!", 'error');
                $('#price').focus();
                return;
            }
            let urls = window.location.href.split("/");
            let id = urls[5];

            var params = {
                
                Location: $("#location").val(),
                Type: $("#types").val(),
                UnitPrice: Number($("#price").val()),
                ClosingDate: $("#date").val(),
                UnitAvailable: Number($("#unit").val()),
                InterestRate: Number($("#interest").val()),
                Longitude: Number($("#logitude").val()),
                Latitude: Number($("#latitude").val()),
                TargetYield: Number($("#targetYield").val()),
                ProjectedGrowth: Number($("#growth").val()),
                Account: $("#account").val(),
                Bank: $("#bank").val(),
                Summary: $("#description").val(),
                VideoLink: $("#videoLink").val(),
                AllowSharing: Number($("#allowSharing").val()),
                MinimumSharing: $("#minimumSharing").val()

            };

            let xhr = new XMLHttpRequest();
            let url = "/update-property/" + id;
            xhr.open('PUT', url, false);
            xhr.setRequestHeader("content-type", "application/json");
            xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
            try {
                xhr.send(JSON.stringify(params));
                if (xhr.status != 200) {
                    // alert('Something went wrong try again!');
                    Swal.fire(
                        'Good job!',
                        'Oops! something went wrong.',
                        'error'
                    );
                    $(".btn-update-property").html("Edit").attr("disabled", !1);
                } else {
                    var res = JSON.parse(xhr.responseText);
                    var data = JSON.parse(res).data;
                    if (JSON.parse(res).success) {
                        window.scrollTo(0, 0);
                        $(".btn-update-property").html("Edit").attr("disabled", !1);
                        Swal.fire(
                            'Good job!',
                            JSON.parse(res).message,
                            'success'
                        );
                    } else {
                        $(".btn-update-property").html("Edit").attr("disabled", !1);
                        window.scrollTo(0, 0);
                    }

                }
            } catch (err) { // instead of onerror
                //alert("Request failed");
                $(".btn-update-property").html("Edit").attr("disabled", !1);
            }
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            confirmPropertyUpdate.fire(
                'Cancelled',
                'No changes was made :)',
                'error'
            )
        }
    });
});

$('.btn-update-decription').click(() => {
     const confirmPropertyUpdate = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mx-2',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    })

    confirmPropertyUpdate.fire({
        title: 'Are you sure?',
        text: "To make changes to property description!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, continue!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".btn-update-decription").html("Processing...").attr("disabled", !0);

            let urls = window.location.href.split("/");
            let id = urls[5];

            var params = {
                Description: {
                    Bathroom: Number($("#bathroom").val()),
                    Toilet: Number($("#toilet").val()),
                    FloorLevel: Number($("#floorLevel").val()),
                    Bedroom: Number($("#bedRoom").val()),
                    LandSize: $("#landSize").val(),
                    AirConditioned: $("#airCondition").is(":checked") ? 1 : 0,
                    Refrigerator: $("#refrigerator").is(":checked") ? 1 : 0,
                    Parking: $("#parking").is(":checked") ? 1 : 0,
                    SwimmingPool: $("#swimmingPool").is(":checked") ? 1 : 0,
                    Laundry: $("#laundry").is(":checked") ? 1 : 0,
                    Gym: $("#gym").is(":checked") ? 1 : 0,
                    SecurityGuard: $("#securityGuard").is(":checked") ? 1 : 0,
                    Fireplace: $("#fireplace").is(":checked") ? 1 : 0,
                    Basement: $("#basement").is(":checked") ? 1 : 0,
                }
            };

            let xhr = new XMLHttpRequest();
            let url = "/property-decription/" + id;
            xhr.open('PUT', url, false);
            xhr.setRequestHeader("content-type", "application/json");
            xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
            try {
                xhr.send(JSON.stringify(params));
                if (xhr.status != 200) {
                    // alert('Something went wrong try again!');
                } else {
                    var res = JSON.parse(xhr.responseText);
                    var data = JSON.parse(res).data;
                    if (JSON.parse(res).success) {
                        window.scrollTo(0, 0);
                        $('#descriptionModal').modal('hide');
                        Swal.fire(
                            'Good job!',
                            JSON.parse(res).message,
                            'success'
                        );
                        $(".btn-update-decription").html("Submit").attr("disabled", !1);

                        setTimeout(() => {
                            location.reload();
                        }, 2000);
                    } else {
                        $(".btn-update-decription").html("Submit").attr("disabled", !1);
                        window.scrollTo(0, 0);
                    }

                }
            } catch (err) { // instead of onerror
                //alert("Request failed");
                $(".btn-update-decription").html("Submit").attr("disabled", !1);
            }
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            confirmPropertyUpdate.fire(
                'Cancelled',
                'No changes was made :)',
                'error'
            )
        }
    });
});

$('#btnUpload').on('click', function () {
    let t = false;
    var e = "";
    let urls = window.location.href.split("/");
    let id = urls[5];
    if ($('#uploadType').val() == "") {
        $('.msg').html(message("Upload type must be selected", "error"));
        return;
    }
    $('.msg').html('');
    $("#btnUpload").attr("disabled", !0).html(`Processing...`);
    var uploadType = $('#uploadType').val();
    var fileUpload = $("#fileUpload").get(0);

    let params = {
        "uploadType": uploadType
    }

    var files = fileUpload.files;

    if (files.length == 0) {
        $('.msg').html(message("Property image is required!", 'error'));
        return;
    }

    var formData = new FormData();

    for (var i = 0; i != files.length; i++) {
        formData.append("files", files[i]);
    }

    formData.append("uploadType", JSON.stringify(params));

    $.ajax(
        {
            url: "/upload-property/" + id,
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                var success = JSON.parse(data).success;
                var message = JSON.parse(data).message;
                if (success) {
                    Swal.fire(
                        'Good job!',
                        message,
                        'success'
                    ).then(() => {
                        location.reload();
                    });
                    $("#btnUpload").attr("disabled", !1).html(`Submit`);
                }
                else {
                    Swal.fire(
                        'Oops!',
                        message,
                        'error'
                    );
                    $("#btnUpload").attr("disabled", !1).html(`Submit`);
                }
            }
        }
    );
});

$('.btn-upload-picture').on('click', function () {
    let t = false;
    var e = "";
    $('.msg').html('');
    $(".btn-upload-picture").attr("disabled", !0).html(`Processing...`);
    var fileUpload = $("#file").get(0);
    var files = fileUpload.files;

    if (files.length == 0) {
        Swal.fire(
            'Oops!',
            'Profile picture is required',
            'error'
        );
        $(".btn-upload-picture").attr("disabled", !1).html(`Submit`);
        return;
    }

    var formData = new FormData();
    formData.append("files", files[0]);

    $.ajax(
        {
            url: "/upload-picture/" + $("#refId").val(),
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                var success = JSON.parse(data).success;
                var message = JSON.parse(data).message;
                if (success) {
                    Swal.fire(
                        'Good job!',
                        message,
                        'success'
                    ).then(() => {
                        location.reload();
                    });
                    $(".btn-upload-picture").attr("disabled", !1).html(`Submit`);
                }
                else {
                    Swal.fire(
                        'Oops!',
                        message,
                        'error'
                    );
                    $(".btn-upload-picture").attr("disabled", !1).html(`Submit`);
                }
            }
        }
    );
});

const GetInvestments = () => {
    let uniqueId = $('#refId').val();
    let xhr = new XMLHttpRequest();
    let url = "/get-investments/" + uniqueId;
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
                investmentTmp(data);
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const GetInvestmentById = () => {
    let urls = window.location.href.split("/");
    let token = urls[5];
    let xhr = new XMLHttpRequest();
    let url = `/get-investment/${token}`;
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
                //adminInvestmentsTmp(data);
                LoadCurrentData(data);
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const investmentTmp = (data) => {
    $('#investments').html('');
    let i = {
        yearlyInterest: 0,
        amount: 0
    };
   
    data.forEach(x => {
        i = {
            yearlyInterest: x.yearlyInterestAmount + x.amount,
            amount: x.amount
        };

        var investmentDate = moment(x.paymentDate).subtract(10, 'days').calendar();
        var maturedDate = new Date(investmentDate);
        maturedDate.setMonth(maturedDate.getMonth() + 11);

        const ctx = document.getElementsByClassName('myChart' + x.transactionRef);
        let res = `<div class="col-lg-3 col-md-3">
					    <div class="single-featured-item">
						    <div class="canvas-img" mb-0 p-4">
                                <img src="${x.data.length > 0 ? x.data[0].url : '/images/featured/featured-2.jpg'}" alt="Image">
							  
						    </div>
						    <div class="featured-content style-three">
							    <div>
                                     <div class="row">
                                        <div class="col-md-4">
                                            <h3 style="font-size:14px; font-weight:normal;">
									            Investment 
								            </h3>
                                        </div>
                                        <div class="col-md-8">
                                            <small class="float-end">${x.propertyId}</small>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-md-8">
                                            <h3 style="font-size:14px; font-weight:normal;">
									            Projected Interest
								            </h3>
                                        </div>
                                        <div class="col-md-4">
                                           <small class="float-end">${x.yield}<sup>%</sup></small>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-md-4">
                                            <h3 style="font-size:14px; font-weight:normal;">
									           Amount
								            </h3>
                                        </div>
                                        <div class="col-md-8">
                                           <small class="price float-end"><sup>&#8358;</sup>${formatToCurrency(x.amount)}</small>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-4">
                                            <h3 style="font-size:14px; font-weight:normal;">
									         Projected Yield
								            </h3>
                                        </div>
                                        <div class="col-md-8">
                                          <small class="price float-end"><sup>&#8358;</sup>${formatToCurrency(x.yearlyInterestAmount)}/yrs</small>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-4">
                                            <h3 style="font-size:14px; font-weight:normal;">
									         Unit
								            </h3>
                                        </div>
                                        <div class="col-md-8">
                                          <small class="float-end">${x.units}</small>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-4">
                                            <h3 style="font-size:14px; font-weight:normal;">
									            Date
								            </h3>
                                        </div>
                                        <div class="col-md-8">
                                            <small class="float-end">${moment(x.paymentDate).format('MMMM Do YYYY')}</small>
                                        </div>
                                    </div>
							    </div>
						    </div>
					    </div>
				    </div>`;
        $('#investments').append(res);
        //myChart(i, ctx);
    });
}

function LoadCurrentData(result) {
    $('#example').DataTable({
        "aLengthMenu": [[5, 10, 25, -1], [5, 10, 25, "All"]],
        "iDisplayLength": 5,
        retrieve: true,
        "data": result,
        "columns": [
            { "data": "customer", "class": "p-3"},
            { "data": "property", "class": "p-3"},
            {
                "data": "yield",
                "class": "p-3",
                render: function (data, type, row) {
                    return data + "<sup>%</sup>";
                }
            },
            {
                "data": "amount",
                "class": "p-3",
                render: function (data, type, row) {
                    return "<sup>&#8358;</sup>" + formatToCurrency(data);
                }
            },
            {
                "data": "yearlyInterestAmount",
                "class": "p-3",
                render: function (data, type, row) {
                    return "<sup>&#8358;</sup>" + formatToCurrency(data);
                }
            },
            { "data": "units", "class": "p-3"},
            { "data": "status", "class": "p-3"},
            {
                "data": "paymentDate",
                "class": "p-3",
                render: function (data, type, row) {
                    return moment(data).format('MMMM Do YYYY');
                }
            },
        ]
    });
}

const adminInvestmentsTmp = (data) => {
    $('#exampleData').html('');
    let i = {
        yearlyInterest: 0,
        amount: 0
    };

    
    data.forEach(x => {
        i = {
            yearlyInterest: x.yearlyInterestAmount + x.amount,
            amount: x.amount
        };

        var investmentDate = moment(x.paymentDate).subtract(10, 'days').calendar();
        var maturedDate = new Date(investmentDate);
        maturedDate.setMonth(maturedDate.getMonth() + 11);

        const ctx = document.getElementsByClassName('myChart' + x.transactionRef);
        let res = `<tr>
                        <td><canvas class="myChart"></canvas></td>
                        <td>${x.property}</td>
                        <td>${x.customer}</td>
                        <td>${x.yield}<sup>%</sup></td>
                        <td><sup>&#8358;</sup>${formatToCurrency(x.amount)}</td>
                        <td><sup>&#8358;</sup>${formatToCurrency(x.yearlyInterestAmount)}/yrs</td>
                        <td>${x.units}</td>
                        <td>${moment(x.paymentDate).format('MMMM Do YYYY')}</td>
                    </tr>`;
        $('#exampleData').append(res);
        myChart(i, ctx);
    });
}

const myChart = (i, ctx) => new Chart(ctx, {
    /*< canvas class= "myChart${x.transactionRef}" width = "400" height = "400" ></canvas >*/
    type: 'pie',
    data: {
        labels: [
            'Amount',
            ' Projected Yield'
        ],
        datasets: [{
            label: 'Investment',
            data: [i.amount, i.yearlyInterest],
            backgroundColor: [
                'rgb(234, 114, 61)',
                'rgb(0, 103, 102)',
            ],
            hoverOffset: 2
        }]
    },
    options: {
        responsive: true,
        plugins: {
            legend: {
                labels: {
                    generateLabels: function (chart) {
                        // Get the default label list
                        const original = Chart.overrides.pie.plugins.legend.labels.generateLabels;
                        const labelsOriginal = original.call(this, chart);

                        // Build an array of colors used in the datasets of the chart
                        let datasetColors = chart.data.datasets.map(function (e) {
                            return e.backgroundColor;
                        });
                        datasetColors = datasetColors.flat();

                        // Modify the color and hide state of each label
                        labelsOriginal.forEach(label => {
                            // There are twice as many labels as there are datasets. This converts the label index into the corresponding dataset index
                            label.datasetIndex = (label.index - label.index % 2) / 2;

                            // The hidden state must match the dataset's hidden state
                            label.hidden = !chart.isDatasetVisible(label.datasetIndex);

                            // Change the color to match the dataset
                            label.fillStyle = datasetColors[label.index];
                        });

                        return labelsOriginal;
                    }
                },
                onClick: function (mouseEvent, legendItem, legend) {
                    // toggle the visibility of the dataset from what it currently is
                    legend.chart.getDatasetMeta(
                        legendItem.datasetIndex
                    ).hidden = legend.chart.isDatasetVisible(legendItem.datasetIndex);
                    legend.chart.update();
                }
            },
            tooltip: {
                callbacks: {
                    label: function (context) {
                        const labelIndex = (context.datasetIndex * 2) + context.dataIndex;
                        return context.chart.data.labels[labelIndex] + ': ' + context.formattedValue;
                    }
                }
            }
        }
    },
});

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
                $('#types').html(`<option lable="&nbsp">Select Type </option>`);
                data.forEach(x => {
                    $('#types').append(`<option value="${x.id}">${x.name.replace("_", " ")} </option>`);
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
            var data = JSON.parse(res).data;
            var messages = JSON.parse(res).message;

            if (JSON.parse(res).success) {
                $('.btn-activate').css('display', 'none').removeClass('.btn-activate');
                //$('.active-title').html(`${messages}. Kindly proceed to login into your account`);
                const confirmPropertyUpdate = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-success mx-2'
                    },
                    buttonsStyling: false
                })

                confirmPropertyUpdate.fire({
                    title: 'Well done',
                    text: `${messages}. Kindly proceed to login into your account`,
                    icon: 'success',
                    showCancelButton: false,
                    confirmButtonText: 'Yes!',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.replace('/Home/signin');
                    }
                });
            } else {
                $(".btn-activate").html("Activate").attr("disabled", !1);
                window.scrollTo(0, 0);
                message(messages, 'error');
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-activate").html("Activate").attr("disabled", !1);
    }
});
const onSubscribe = () => {
    let xhr = new XMLHttpRequest();
    let url = "/subscribe";
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send();
        if (xhr.status != 200) {
            $(".btn-subscribe").html("Loading...").attr("disabled", !0);
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;

            if (JSON.parse(res).success) {
                location = data;
                //window.open(data, "Dominoes Society", "status=1,toolbar=1");
                $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
            } else {
                $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
                window.scrollTo(0, 0);
                message(data, 'error');
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
    }
}
$('.btn-subscribe').click(() => {
    $(".btn-subscribe").html("Loading...").attr("disabled", !0);
    let xhr = new XMLHttpRequest();
    let url = "/subscribe";
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {

        xhr.send();
        if (xhr.status != 200) {
            $(".btn-subscribe").html("Loading...").attr("disabled", !0);
            // alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;

            if (JSON.parse(res).success) {
                location = data;
                //window.open(data, "Dominoes Society", "status=1,toolbar=1");
                $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
            } else {
                $(".btn-subscribe").html("Subscribe to get full access").attr("disabled", !1);
                window.scrollTo(0, 0);
                message(data, 'error');
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
            var data = JSON.parse(res).data;

            if (JSON.parse(res).success) {
                $('.btn-verify').css('display', 'none').removeClass('.btn-verify');
                const confirmPropertyUpdate = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-success mx-2'
                    },
                    buttonsStyling: false
                })

                confirmPropertyUpdate.fire({
                    title: 'Well done',
                    text: `${messages}. Kindly proceed to login into your account`,
                    icon: 'success',
                    showCancelButton: false,
                    confirmButtonText: 'Yes!',
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.replace('/Home/signIn');
                    }
                });
                //$('.active-title').html(`${messages}. Kindly proceed to login into your account`);
            } else {
                $(".btn-verify").html("Verify").attr("disabled", !1);
                window.scrollTo(0, 0);
                $('.active-title').html(`<span class="text-danger">Payment verification failed, kindly contact administrator</span>`);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-verify").html("Verify").attr("disabled", !1);
    }
});

const propertyInvestment =  () => {
    let price = Number($('#price').text().replace(/[^0-9\.-]+/g, "").replace("₦", ""));
    let unit = $('#unit').val();
    const confirmPropertyUpdate = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mx-2',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    })

    confirmPropertyUpdate.fire({
        title: 'Are you sure?',
        text: "To proceed with this investment!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, invest it!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".btn-property-investment").html("Processing...").attr("disabled", !0);
            let urls = window.location.href.split("/");
            let id = urls[5];
            let params = {
                propertyUniqueId: id,
                units: unit
            }
            let xhr = new XMLHttpRequest();
            let url = "/invest";
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
                    if (JSON.parse(res).success) {
                        window.scrollTo(0, 0);
                        location = data;
                        $(".btn-property-investment").html("Invest").attr("disabled", !1);
                    } else {
                        var err = JSON.parse(res).message;
                        Swal.fire(
                            'Oops!',
                            err == "Forbiden" ?  "You don't have permission to perform this action" :  "Something went wrong, admin has been contacted",
                            'error'
                        );
                        $(".btn-property-investment").html("Invest").attr("disabled", !1);
                        window.scrollTo(0, 0);
                    }

                }
            } catch (err) { // instead of onerror
                //alert("Request failed");
                $(".btn-property-investment").html("Invest").attr("disabled", !1);
            }
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            //confirmPropertyUpdate.fire(
            //    'Cancelled',
            //    'No changes was made :)',
            //    'error'
            //)
        }
    });
};


$(".btn-change-password").on("submit", function () {
    const confirmPasswordChange = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mx-2',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    })

    confirmPasswordChange.fire({
        title: 'Are you sure?',
        text: "To change your password!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, continue!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".btn-change-password").html("Processing...").attr("disabled", !0);
            let t = false;
            var e = "";
            if (
                ($("#password-form")
                    .find("input")
                    .each(function () {
                        $(this).prop("required") && ($(this).val() || ((t = !0), (name = $(this).attr("name")), (e += name + ", ")));
                    }))
            )
                if (t) message("Validation error the following field are required " + e.substring(0, e.length - 2), 'error'), window.scrollTo(0, 0), $(".btn-change-password").attr("disabled", !1).html("Reset Password");
                else {
                    if ($("#password").val() != $("#confirm").val()) {
                        message("Password mismatch, kindly check and try again", "error");
                        return;
                    }

                    var a = {
                        CurrentPassword: $("#current").val(),
                        Password: $("#password").val(),
                        Confirm: $("#confirm").val(),
                    };
                    $.ajax({
                        type: "post",
                        url: "/change-password",
                        headers: { "Content-Type": "application/json" },
                        data: JSON.stringify(a),
                        success: function (t) {
                            var res = JSON.parse(t);
                            if (res.success) {
                                // message(res.message + '. Kindly check your mail to activate your account', 'success');
                                Swal.fire(
                                    'Good job!',
                                    res.message,
                                    'success'
                                )
                                $(".form-control").val(""),
                                    $("#firstName").focus(),
                                    window.scrollTo(0, 0),
                                    $(".btn-change-password").html("Reset Password").attr("disabled", !1),

                                    a = {};

                            } else {

                                message(res.data
                                    , 'error');
                                window.scrollTo(0, 0);
                                $(".btn-change-password").html("Reset Password").attr("disabled", !1);
                                a = {};
                            }

                        },
                        error: function (t) {
                            if (400 == t.status) return //alert("check your supply value and try again!"),
                            void $(".btn-change-password").html("Reset Password").attr("disabled", !1);
                            $(".btn-change-password").html("Reset Password").attr("disabled", !1);
                        },
                    });
                }
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            confirmPasswordChange.fire(
                'Cancelled',
                'No changes was made :)',
                'error'
            )
        }
    });
});

$(".btn-update-profile").on("click", function () {

    if ($('.catch_me').val() != "") {
        return;
    }
    const confirmPropertyUpdate = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mx-2',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    })

    confirmPropertyUpdate.fire({
        title: 'Are you sure?',
        text: "To update your profile!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, continue!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".btn-update-profile").html("Processing...").attr("disabled", !0);
            let t = false;
            var e = "";
            if (
                ($("#profile-form")
                    .find("input")
                    .each(function () {
                        $(this).prop("required") && ($(this).val() || ((t = !0), (name = $(this).attr("name")), (e += name + ", ")));
                    }))
            )
                if (t) message("Validation error the following field are required " + e.substring(0, e.length - 2), 'error'), window.scrollTo(0, 0), $(".btn-update-profile").attr("disabled", !1).html("Edit");
                else {
                    var a = {
                        AccountNumber: $("#account").val(),
                        BankName: $("#bank").val(),
                        Phone: $("#phone").val(),
                        Address: $("#address").val()
                    };

                    $.ajax({
                        type: "post",
                        url: "/update-profile",
                        headers: { "Content-Type": "application/json" },
                        data: JSON.stringify(a),
                        success: function (t) {
                            var res = JSON.parse(t);
                            if (res.success) {
                                Swal.fire(
                                    'Good job!',
                                    res.message,
                                    'success'
                                );
                                $("#firstName").focus();
                                window.scrollTo(0, 0);
                                $(".btn-update-profile").html("Edit").attr("disabled", !1);
                                a = {};
                            } else {
                                message(res.data
                                    , 'error'),
                                    window.scrollTo(0, 0),
                                    $(".btn-update-profile").html("Edit").attr("disabled", !1);
                                a = {};
                            }

                        },
                        error: function (t) {
                            if (400 == t.status) return //alert("check your supply value and try again!"),
                            void $(".btn-update-profile").html("Edit").attr("disabled", !1);
                            $(".btn-update-profile").html("Edit").attr("disabled", !1);
                        },
                    });
                }
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            confirmPropertyUpdate.fire(
                'Cancelled',
                'No changes was made :)',
                'error'
            )
        }
    });
});

$('.logout').click(() => {
    const confirmPropertyUpdate = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mx-2',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    })

    confirmPropertyUpdate.fire({
        title: 'Are you sure?',
        text: "You want to logout!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
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
                        window.location.replace('/');
                    }
                }
            } catch (err) { // instead of onerror
                //alert("Request failed");
            }
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            confirmPropertyUpdate.fire(
                'Cancelled',
                'Thanks for staying back :)',
                'error'
            )
        }
    });
});


const message = (msg, _class) => $('#msg').html(`<div class="alert alert-${_class == "error" ? 'danger' : 'success'} alert-dismissible fade show" role="alert">
							${msg}
						</div>`);

function formatToCurrency(amount) {
    return (amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

const currency = (value) => new Intl.NumberFormat().format(value);

const setSelectedOption = (id, value) => {

    $(id + ' option').each(function () {
        if ($(this).val() == value) {
            $(this).prop("selected", true);
        }
    });
}

//generate random of array of n
function getRandom(arr, n) {
    var result = new Array(n),
        len = arr.length,
        taken = new Array(len);
    if (n > len)
        throw new RangeError("getRandom: more elements taken than available");
    while (n--) {
        var x = Math.floor(Math.random() * len);
        result[n] = arr[x in taken ? taken[x] : x];
        taken[x] = --len in taken ? taken[len] : len;
    }
    return result;
}

function fundWallet(){
    (async () => {
        const { value: amount } = await Swal.fire({
            title: 'Fund Wallet',
            input: 'number',
            inputLabel: 'Amount',
            showCancelButton: true,
            inputValidator: (value) => {
                if (!value) {
                    return 'Input amount'
                }
            }
        })

        if (amount) {
            let param = { Amount: amount };
            let xhr = new XMLHttpRequest();
            let url = "/fund-wallet";
            xhr.open('POST', url, false);
            xhr.setRequestHeader("content-type", "application/json");
            xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
            try {

                xhr.send(JSON.stringify(param));
                if (xhr.status != 200) {
                    Swal.fire(
                        'Oops!',
                        'Error initiating transaction status,we will re-confirm and get back to you',
                        'error'
                    );
                    //alert('Something went wrong try again!');
                } else {
                    var res = JSON.parse(xhr.responseText);
                    var data = JSON.parse(res).data;
                    if (res) {
                        location = data;
                    }
                }
            } catch (err) { // instead of onerror
                //alert("Request failed");
            }
        }

    })()
}

$('.btn-request').click(() => {
    if ($('.fill-me').val() != "") {
        return;
    }

    if ($('#msg_subject').val() == "") {
        $('#msgSubmit').html(`<div class="alert alert-danger pt-2 pb-2" role="alert">
            Subject can't be empty!
            </div>`).removeClass('hidden');
        return;
    }

    if ($('#message').val() == "") {
        $('#msgSubmit').html(`<div class="alert alert-danger pt-2 pb-2" role="alert">
            Message body can't be empty!
            </div>`).removeClass('hidden');
        return;
    }

    $(".btn-request").html("Processing...").attr("disabled", !0);
    let urls = window.location.href.split("/");
    let token = urls[5];
    var a = {
        Subject: $("#msg_subject").val(),
        Message: $("#message").val(),
        PropertyRef: token,
        CustomerRef: $("#refId").val(),
    };

    $.ajax({
        type: "post",
        url: "/enquiry",
        headers: { "Content-Type": "application/json" },
        data: JSON.stringify(a),
        success: function (t) {
            var res = JSON.parse(t);
            if (res.success) {
                Swal.fire(
                    'Good job!',
                    res.message,
                    'success'
                );
                $(".form-control").val('');

                window.scrollTo(0, 0);
                $(".btn-request").html("Request Information").attr("disabled", !1);
                a = {};
            } else {
                message(res.data
                    , 'error'),
                    window.scrollTo(0, 0),
                    $(".btn-request").html("Request Information").attr("disabled", !1);
                a = {};
            }

        },
        error: function (t) {
            if (400 == t.status) return //alert("check your supply value and try again!"),
            void $(".btn-request").html("Request Information").attr("disabled", !1);
            $(".btn-request").html("Request Information").attr("disabled", !1);
        },
    });
});

const confirmTransaction = () => {
    let urls = window.location.href.split("/");
    let token = urls[3].split("?")[1];
    if (token != undefined) {
        if (token.includes("success")) {

            const confirmPropertyUpdate = Swal.mixin({
                customClass: {
                    confirmButton: 'btn btn-success mx-2'
                },
                buttonsStyling: false
            })

            confirmPropertyUpdate.fire({
                title: 'Well done',
                text: "Congratulation your transaction was successful, kindly proceed to your dashboard to verify",
                icon: 'success',
                showCancelButton: false,
                confirmButtonText: 'Yes!',
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.replace('/dashboard');
                }
            });
        } else {
            Swal.fire(
                'Oops!',
                'Transaction failed kindly contact admin for support',
                'error'
            );
        }
    }
}

$(document).ready(function () {
    $('.btn-news').click(function (e) {
        e.preventDefault();
        if ($('#email').val().trim() == "") {
            Swal.fire(
                'Oops!',
                'Kindlyt provide active email address',
                'error'
            );
            return;
        }

        $(".btn-news").attr("disabled", !0);
        let param = {
            email: $('#email').val().trim()
        }

        let xhr = new XMLHttpRequest();
        let url = "/news";
        xhr.open('POST', url, false);
        xhr.setRequestHeader("content-type", "application/json");
        xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
        try {
            xhr.send(JSON.stringify(param));
            if (xhr.status != 200) {
                //alert('Something went wrong try again!');
            } else {
                var res = JSON.parse(xhr.responseText);
                var message = JSON.parse(res).message;
                if (JSON.parse(res).success) {
                    $(".btn-news").attr("disabled", !1);
                    Swal.fire(
                        'Good job!',
                        message,
                        'success'
                    ).then(() => {
                        $('#email').val('');
                    });

                } else {
                    $(".btn-news").attr("disabled", !1);
                    Swal.fire(
                        'Oops!',
                        message,
                        'error'
                    );
                }
            }
        } catch (err) { // instead of onerror
            //alert("Request failed");
        }
    })
});

const getNewSubscribers = () => {
    let xhr = new XMLHttpRequest();
    let url = "/get-newsSubscribers";
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {
        xhr.send();
        if (xhr.status != 200) {
            //alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;
            if (JSON.parse(res).success) {
                $('#newsCount').text(`New Subscribers (${data.length})`);
                $('#description').val(data.join(", "));
            } else {
                Swal.fire(
                    'Oops!',
                    data,
                    'error'
                );
            }
        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}
const getEnquiries = () => {
    let xhr = new XMLHttpRequest();
    let url = "/get-enquiries";
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {
        xhr.send();
        if (xhr.status != 200) {
            //alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;
            if (JSON.parse(res).success) {
                enquiriesTmp(data);
            } else {
                Swal.fire(
                    'Oops!',
                    JSON.parse(res).message,
                    'error'
                );
            }
        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const enquiriesTmp = (data) => {
    $('#Table_ID tbody').html('');

    data.forEach(x => {
        let res = `<tr>
                    <td>${x.subject}</td>
                    <td><span class="badge badge-primary ${x.status == "New" ? 'bg-success': 'bg-info'}">${x.status}</span></td>
                    <td>${x.status == "New" ? '<a href="javascript:void(0)" class="default-btn" onclick="getEnquiry(${x.id})">View</a>' : '...'}</td>
                </tr>`;
        $('#Table_ID tbody').append(res);
    })
}

const getEnquiry = (id) => {
    let xhr = new XMLHttpRequest();
    let url = `/get-enquiry/${id}`;
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {
        xhr.send();
        if (xhr.status != 200) {
            //alert('Something went wrong try again!');
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).data;
            if (JSON.parse(res).success) {
                Swal.fire({
                    template: '#my-template'
                });
                $('#enquiryId').val(data.id);
                $('.enquiryMessage').text(data.message);

                $('.swal2-popup').css("width", '50%');

            } else {
                Swal.fire(
                    'Oops!',
                    JSON.parse(res).message,
                    'error'
                );
            }
        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const treatEnquiry = ()=> {
    let id = $('#enquiryId').val();
    if (id == "") {
        return;
    }
    $(".btn-treated").html('Processing...').attr("disabled", !0);
    let xhr = new XMLHttpRequest();
    let url = `/update-enquiry-status/${id}/CLOSED`;
    xhr.open('GET', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {
        xhr.send();
        if (xhr.status != 200) {
            //alert('Something went wrong try again!');
            $(".btn-treated").html('Mark as treated').attr("disabled", !1);
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).message;
            if (JSON.parse(res).success) {
                Swal.fire(
                    'Good job!',
                    data,
                    'success'
                );
                $(".btn-treated").html('Mark as treated').attr("disabled", !1);
            } else {
                $(".btn-treated").html('Mark as treated').attr("disabled", !1);
                Swal.fire(
                    'Oops!',
                    data,
                    'error'
                );
            }
        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
};

const typeModal = () => {
    Swal.fire({
        template: '#type-template',
        showCancelButton: false,
        showConfirmButton: false
    });
}


const addPropertyType = () => {
    let type = $('#property-type-name').val();
    if (type == "") {
        return;
    }
    let param = {
        type: type
    }

    //this.attr("disabled", !0);
    let xhr = new XMLHttpRequest();
    let url = `/property-type`;
    xhr.open('POST', url, false);
    xhr.setRequestHeader("content-type", "application/json");
    xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
    try {
        xhr.send(JSON.stringify(param));
        if (xhr.status != 200) {
            //alert('Something went wrong try again!');
            //this.attr("disabled", !1);
        } else {
            var res = JSON.parse(xhr.responseText);
            var data = JSON.parse(res).message;
            if (JSON.parse(res).success) {
                Swal.fire(
                    'Good job!',
                    data,
                    'success'
                );
                location.reload();
                //this.attr("disabled", !1)
               
            } else {
                //this.attr("disabled", !1);
                Swal.fire(
                    'Oops!',
                    data,
                    'error'
                );
            }
        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

const getTypes = () => {
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
                typeTemp(data);
            } else {
                window.scrollTo(0, 0);
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
        $(".btn-login").html("Login").attr("disabled", !1);
    }
}

const typeTemp = (data) => {
    $('#Table_property tbody').html('');

    data.forEach(x => {
        let res = `<tr>
                    <td>${x.name}</td>
                    <td>
                        <a href="#" class="text-warning mr-2"><i class="fa fa-pencil"></i></a>
                        <a href="#" class="text-danger"><i class="fa fa-trash"></i></a>
                    </td>
                </tr>`;
        $('#Table_property tbody').append(res);
    })
}

$('.signup').click(() => {
    $('#signup').removeClass('d-none');
    $('#signin').addClass('d-none');
});

$('.signin').click(() => {
    $('#signup').addClass('d-none');
    $('#signin').removeClass('d-none');
});

const sendOnboardingEmail = () => {
    var startDate = null;
    if ($('#startDate').val() != "") {
        startDate = $('#startDate').val();
    }
    let xhr = new XMLHttpRequest();
    let url = "/onboarding/" + startDate;
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
            console.log(data);
            if (JSON.parse(res).success) {
                Swal.fire(
                    'Good job!',
                    data,
                    'success'
                );
            } else {
                Swal.fire(
                    'Oops!',
                    data,
                    'error'
                );
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
}

function forgetPassword() {

    (async () => {
        
        const { value: email } = await Swal.fire({
            title: 'Forget password?',
            input: 'email',
            inputLabel: 'Enter your email address',
            inputPlaceholder: 'Enter your email address'
        })

        if (email) {
            let xhr = new XMLHttpRequest();
            let url = "/reset-password/" + email;
            xhr.open('GET', url, false);
            xhr.setRequestHeader("content-type", "application/json");
            xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
            try {
                xhr.send();
                if (xhr.status != 200) {
                    //alert('Something went wrong try again!');
                } else {
                    var res = JSON.parse(xhr.responseText);
                    var data = JSON.parse(res).data;
                    if (JSON.parse(res).success) {
                        Swal.fire(
                            'Good job!',
                             data,
                            'success'
                        );
                    } else {
                        Swal.fire(
                            'Oops!',
                             data,
                            'error'
                        );
                    }
                }
            } catch (err) { // instead of onerror
                //alert("Request failed");
            }
        }
    })()
};