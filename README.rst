PoundPay
--------

PoundPay enables developers to build apps which facilitate
transactions between two of their users. PoundPay is designed
specifically for these types of transactions, as opposed to direct
payments from customer to business. In short, PoundPay is the payments
platform for marketplaces.

Serving IFRAME
``````````````

::

    <script src="https://www.poundpay.com/js/poundpay.js"></script>

    <div id="pound-root"></div>

    <script>
      function handlePaymentSuccess() {
        // do something
      }

      function handlePaymentError() {
        // handle error
      }

      PoundPay.init({
        payment_sid: "<%= payment_sid %>",
        success: handlePaymentSuccess,
        error: handlePaymentError,
        name: "Fred Nietzsche", // Optional
        address_street: "990 Guerrero St", // Optional
        address_city: "San Francisco", // Optional
        address_state: "California", // Optional
        address_zip: "94110", // Optional
        server: "https://www-sandbox.poundpay.com"  // Exclude for production
      });
    </script>



Links
`````

* `Developer Documentation <https://dev.poundpay.com/>`_
* `Website  <https://poundpay.com/>`_