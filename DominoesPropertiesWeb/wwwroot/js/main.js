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
                   // message(res.Message + '. Kindly check your mail to activate your account', 'success');
                    Swal.fire(
                        'Good job!',
                        res.Message + '. Kindly check your mail to activate your account',
                        'success'
                    )
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
                console.log(data);
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
                console.log(data);
                $('.total-property').text(data.TotalProperty);
                $('.total-investment').text(data.TotalInvestment);
                $('.active-investment').text(data.ActiveInvestment);
                $('.total-customer').text(data.TotalCustomer);
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
    } else {
        $('.fullName').text(data.firstName + " " + data.lastName);
        $('.walletId').html("wallet ID ( <strong>" + data.walletId + "</strong> )");
        $('.walletBalance').html('&#8358; ' + data.walletBalance);
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
					<p class="text-muted mb-0">${data.accountNumber}</p>
					</div>
				</div>
				<hr>
                <div class="row">
					<div class="col-sm-3">
					<h6 class="mb-0">Bank Name</h6>
					</div>
					<div class="col-sm-9">
					<p class="text-muted mb-0">${data.bankName}</p>
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

        $('.add-property').html(`<a href="/property/create" class="default-btn rounded"><i class="fa fa-plus"></i> New Property</a>`);
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
                    adminPropertTmp(data);
                } else if (type == "landing") {
                    LandingPagePropertyTmp(data.slice(0, 7));
                    var i = getRandom(data, 1);
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
                       `)
                    console.log(i);
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

$('.btn-filter-property').click(() => {
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
        MaxPrice: maxPrice
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
            console.log(res);
            if (JSON.parse(res).success) {
                $('#property-count').html(data.length + ' Results Found')
                propertiesTmp(data);
            } else {
                Swal.fire(
                    'Opps!',
                    'No data match your request',
                    'error'
                );
            }

        }
    } catch (err) { // instead of onerror
        //alert("Request failed");
    }
});

const adminPropertTmp = (data) => {
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-lg-4 col-md-4">
									<div class="single-featured-item">
 <a href="/property/viewproperty/${x.uniqueId}">
										<div class="featured-img mb-0">
											<img src="/images/featured/featured-2.jpg" alt="Image">
										</div>
										<div class="featured-content style-three">
											<div class="d-flex justify-content-between">
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

const propertiesTmp = (data) => {
    $('#properties').html('');

    data.forEach(x => {
        let res = `<div class="col-lg-6 col-md-6">
									<div class="single-featured-item">
 <a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">
										<div class="featured-img mb-0">
											<img src="/images/featured/featured-2.jpg" alt="Image">
										</div>
										<div class="featured-content style-three">
											<div class="d-flex justify-content-between">
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
        let res = `<div class="col-lg-3 col-md-6 ">
									<div class="single-featured-item">
                                        <a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">
										    <div class="featured-img mb-0">
											    <img src="/images/featured/featured-2.jpg" alt="Image">
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

    if ($('#refId').val() == null || $('#refId').val() == "") {
        location = "/Home/signin";
        return;
    }

    //if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
    //    Swal.fire({
    //        icon: 'info',
    //        title: 'Oops...',
    //        text: 'Property details can only be view by subscribed users, kindly subscribe to get full access ',
    //        footer: `<a href="javascript:void(0)" class="default-btn rounded" onclick="onSubscribe()">Subcribe Now</a>`
    //    })
    //    return;
    //}

    location = '/Home/PropertyDetails/' + id;
}

$('.edit-property').click(() => {
    let urls = window.location.href.split("/");
    let id = urls[5];
    let url = `/property/edit/${id}`;
    location = url;
});

const getSingleProperty = () => {

    //if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
    //    location = '/Dashboard/Profile';
    //    return;
    //}
    
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
                $('.closeDate').html("Investment End On " + moment(data.closingDate).format('MMMM Do YYYY'));
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
                $('.summary').text(`${data.summary}`);

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

                //checkout

                $('#propertyName').text(data.name);
                $('#checkoutPrice').html("&#8358; " + formatToCurrency(data.unitPrice) + " / Unit");
                $('.checkoutPrice').html(formatToCurrency(data.unitPrice));
                $('#availableUnit').html("Available Unit : " +  data.unitAvailable);
                $('#closeDate').html("Investment End On " + moment(data.closingDate).format('MMMM Do YYYY'));
                $('.total').html("&#8358; " + formatToCurrency(data.unitPrice));
                let price = Number($('#price').text().replace(/[^0-9\.-]+/g, "").replace("₦", ""));
                $('.groundTotal').html("&#8358; " + formatToCurrency(price * $('#unit').val()));
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

$('#unit').on('input', () => {
    $('.unit').text($('#unit').val());
    let price = Number($('#price').text().replace(/[^0-9\.-]+/g, "").replace("₦", ""));

    $('.total').html("&#8358; " + formatToCurrency(price * $('#unit').val()));
    $('.groundTotal').html("&#8358; " + formatToCurrency(price * $('#unit').val()));

})


const editSingleProperty = () => {

    //if ($('#isSubcribed').val() == "False" && $('#refId').val() != "") {
    //    location = '/Dashboard/Profile';
    //    return;
    //}

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
                InterestRate: Number($("#interest").val()),
                Longitude: Number($("#logitude").val()),
                Latitude: Number($("#latitude").val()),
                Summary: $("#description").val(),
                Account: $("#account").val(),
                Bank: $("#bank").val(),
                MaxUnitPerCustomer: Number($("#maxCustomerUnit").val()),
                ClosingDate: $("#closingDate").val()

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
                    var Message = JSON.parse(res).Message;
                    console.log(res);
                    if (JSON.parse(res).Success) {
                        window.scrollTo(0, 0);
                        //message(JSON.parse(res).Message, "success");
                        $('.form-control').val('');
                        $(".btn-property").html("Submit").attr("disabled", !1);
                        Swal.fire(
                            'Good job!',
                            Message,
                            'success'
                        );

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
                MaxUnitPerCustomer: Number($("#maxCustomerUnit").val()),
                Summary: $("#description").val(),
                ClosingDate: $("#closingDate").val()

            };

            console.log(params);
            let xhr = new XMLHttpRequest();
            let url = "/update-property/" + id;
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
                    console.log(res);
                    if (JSON.parse(res).Success) {
                        window.scrollTo(0, 0);
                        //message(JSON.parse(res).Message, "success");
                        //$('.form-control').val('');
                        $(".btn-update-property").html("Edit").attr("disabled", !1);
                        Swal.fire(
                            'Good job!',
                            JSON.parse(res).Message,
                            'success'
                        );
                        //setTimeout(() => {
                        //    location.reload();
                        //}, 2000);
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

    //const confirmPropertyUpdate = Swal.mixin({
    //    customClass: {
    //        confirmButton: 'btn btn-success',
    //        cancelButton: 'btn btn-danger'
    //    },
    //    buttonsStyling: false
    //})

    //confirmPropertyUpdate.fire({
    //    title: 'Are you sure?',
    //    text: "To make changes to this property data!",
    //    icon: 'warning',
    //    showCancelButton: true,
    //    confirmButtonText: 'Yes, update it!',
    //    cancelButtonText: 'No, cancel!',
    //    reverseButtons: true
    //}).then((result) => {
    //    if (result.isConfirmed) {

    //    } else if (
    //        /* Read more about handling dismissals below */
    //        result.dismiss === Swal.DismissReason.cancel
    //    ) {
    //        confirmPropertyUpdate.fire(
    //            'Cancelled',
    //            'No changes was made :)',
    //            'error'
    //        )
    //    }
    //});
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
                    var data = JSON.parse(res).Data;
                    console.log(res);
                    if (JSON.parse(res).Success) {
                        window.scrollTo(0, 0);
                        //message("Description updated successfully", "success");
                        $('#descriptionModal').modal('hide');
                        //$('.form-control').val('');
                        Swal.fire(
                            'Good job!',
                            JSON.parse(res).Message,
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
    $("#btnUpload").attr("disabled", !0).html(`Processing...`)
    
    var fileUpload = $("#fileUpload").get(0);

    var files = fileUpload.files;

    if (files.length == 0) {
        $('#msg').html(message("Property image is required!", 'error'));
        return;
    }

    
    var formData = new FormData();

    for (var i = 0; i != files.length; i++) {
        formData.append("files", files[i]);
    }

    $.ajax(
        {
            url: "/upload-property/" + id,
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                alert("Files Uploaded!");
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
                console.log(data);
                investmentTmp(data);
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

    data.forEach(x => {
        let res = `<div class="col-lg-6 col-md-6">
									<div class="single-featured-item">
                                    <a href="javascript:void(0)" onclick="propertyDetails('${x.uniqueId}')">
										<div class="featured-img mb-0">
											<img src="/images/featured/featured-2.jpg" alt="Image">
										</div>
										<div class="featured-content style-three">
											<div class="d-flex btn-subscribejustify-content-between">
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

        $('#invetments').append(res);
    });
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

//$('#property-link').click(() => {
//    if ($('#refId').val() == null || $('#refId').val() == "") {
//        location = "/Home/signin";
//        return;
//    }
//    location = "/Home/properties";
//});

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

$('.btn-property-investment').on('click', () => {
    let price = Number($('#price').text().replace(/[^0-9\.-]+/g, "").replace("₦", ""));
    
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
                units: $('#unit').val()
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
                    var data = JSON.parse(res).Data;
                    console.log(res);
                    if (JSON.parse(res).Success) {
                        window.scrollTo(0, 0);
                        //message(JSON.parse(res).Message, "success");
                        //Swal.fire(
                        //    'Good job!',
                        //    JSON.parse(res).Message,
                        //    'success'
                        //);
                        location = data;
                        $(".btn-property-investment").html("Invest").attr("disabled", !1);
                    } else {
                        Swal.fire(
                            'Opps!',
                            data,
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
});


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
                            if (res.Success) {
                                // message(res.Message + '. Kindly check your mail to activate your account', 'success');
                                Swal.fire(
                                    'Good job!',
                                    res.Message,
                                    'success'
                                )
                                $(".form-control").val(""),
                                    $("#firstName").focus(),
                                    window.scrollTo(0, 0),
                                    $(".btn-change-password").html("Reset Password").attr("disabled", !1),

                                    a = {};

                            } else {

                                message(res.Data
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
                            if (res.Success) {
                                Swal.fire(
                                    'Good job!',
                                    res.Message,
                                    'success'
                                );
                                $("#firstName").focus();
                                window.scrollTo(0, 0);
                                $(".btn-update-profile").html("Edit").attr("disabled", !1);
                                a = {};
                            } else {
                                message(res.Data
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
                        location = '/';
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
            title: 'Enter An Amount',
            input: 'text',
            //inputLabel: 'Enter An Amount',
            showCancelButton: true,
            inputValidator: (value) => {
                if (!value || !Number(value)) {
                    return 'You need to enter a valid amount'
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
                    //alert('Something went wrong try again!');
                } else {
                    var res = JSON.parse(xhr.responseText);
                    var data = JSON.parse(res).Data;
                    console.log(data);
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