function createPayment () {
  var args = {};
  var inputs = $('#paymentsTable input').each(function(i, item) {
    args[item.id] = item.value;
  });
  var request = {};
  request.url = "payment/create";
  request.type = "POST";
  request.data = $.param(args);
  request.success = function(data) {
    $('#payment_id').val(data.sid);
    $('#operating_payment_sid').val(data.sid);
  };
  $.ajax(request);
}

function escrowPayment () {
  var args = {};
  args.sid = $('#operating_payment_sid').val()
  var request = {};
  request.url = "payment/escrow";
  request.type = "POST";
  request.data = $.param(args);
  request.success = function(data) {
    $('#operation_results').append(data);
  };
  $.ajax(request);
}

function releasePayment () {
  var args = {};
  args.sid = $('#operating_payment_sid').val()
  var request = {};
  request.url = "payment/release";
  request.type = "POST";
  request.data = $.param(args);
  request.success = function(data) {
    $('#operation_results').append(data);
  };
  $.ajax(request);
}

function startIFrame() {
  // invoke Pound iframe
  var args = {
    success: paymentSuccessCallback,
    error: paymentErrorCallback,
    payment_sid: $('#payment_id').val(),
    server: $('#server').val(),
    name: $('#cardholder_name').val(),
    address_street: '',
    address_city: '',
    address_state: '',
    address_zip: ''
  };
  PoundPay.init(args);
}

function paymentSuccessCallback() {
  $("#pound-root").hide();
  $('#paymentComplete').show();
}

function paymentErrorCallback() {
  $("#pound-root").hide();
  alert("an error occurred");
}
