using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace poundpay_test
{
    [TestClass]
    public class PaymentTests : TestBase
    {
        public PaymentTests()
        {
            base.Init();
        }

        private IDictionary<string, string> PaymentValues()
        {
            return new Dictionary<string, string>() {
                {"amount", "500"},
                {"description", "A description"},
                {"developer_sid", developerSid},
                {"payer_email_address", "payer@example.com"},
                {"payer_fee_amount", "200"},
                {"recipient_email_address", "recipient@example.com"},
                {"recipient_fee_amount", "0"},
            };
        }

        [TestMethod]
        public void CreateAPayment()
        {
            poundpay.PoundPay pp = new poundpay.PoundPay(developerSid, authToken, apiUrl, apiVersion);

            var payment = pp.Payment.Create();

            foreach (var item in PaymentValues())
            {
                payment[item.Key] = item.Value;
            }

            payment.Save();

            Assert.IsNotNull(payment["sid"]);

        }

        [TestMethod]
        public void GetPaymentVersions()
        {

            poundpay.PoundPay pp = new poundpay.PoundPay(developerSid, authToken, apiUrl, apiVersion);

            var payment = pp.Payment.Create();

            foreach (var item in PaymentValues())
            {
                payment[item.Key] = item.Value;
            }

            payment.Save();

            var allVersions = payment.Versions();

            Assert.IsTrue(allVersions.Count > 0);
        }

        [TestMethod]
        public void GetAllPayments()
        {
            poundpay.PoundPay pp = new poundpay.PoundPay(developerSid, authToken, apiUrl, apiVersion);

            var payment = pp.Payment.Create();

            foreach (var item in PaymentValues())
            {
                payment[item.Key] = item.Value;
            }

            payment.Save();

            var allPayments = pp.Payment.All();

            Assert.IsTrue(allPayments.Count > 0);

        }

        /// <summary>
        /// We cannot run this test because we lack a way to move
        /// the payment from STAGED to AUTHORIZED (e.g. customer
        /// doing the payment)
        /// </summary>
        [TestMethod, Ignore]
        public void WholePaymentProcess()
        {
            poundpay.PoundPay pp = new poundpay.PoundPay(developerSid, authToken, apiUrl, apiVersion);

            var payment = pp.Payment.Create();

            foreach (var item in PaymentValues())
            {
                payment[item.Key] = item.Value;
            }

            payment.Save();

            Assert.IsNotNull(payment["sid"]);
            Assert.AreEqual("STAGED", payment["status"]);

            payment.Escrow();

            Assert.AreEqual("", payment["status"]);

            payment.Release();

            Assert.AreEqual("", payment["status"]);

        }

        [TestMethod]
        public void TestEscrowThrowsExceptionIfNotAuthorized()
        {
            poundpay.PoundPay pp = new poundpay.PoundPay(developerSid, authToken, apiUrl, apiVersion);

            var payment = pp.Payment.Create();

            foreach (var item in PaymentValues())
            {
                payment[item.Key] = item.Value;
            }

            payment["status"] = "AUTHORIZED";

            try
            {
                payment.Escrow();
            }
            catch (poundpay.PaymentEscrowError) { }
        }
        
/*
 * TODO: Implement these (from Python project)
    def test_escrow_throws_exception_if_not_AUTHORIZED(self):
        payment = Payment(**self.payment_arguments)
        for status in PAYMENT_STATUSES:
            if status == 'AUTHORIZED':
                continue
            payment.status = status
            with self.assertRaises(poundpay.payments.PaymentEscrowError):
                payment.escrow()

    def test_release_throws_exception_if_not_ESCROWED(self):
        payment = Payment(**self.payment_arguments)
        for status in PAYMENT_STATUSES:
            if status == 'ESCROWED':
                continue
            payment.status = status
            with self.assertRaises(poundpay.payments.PaymentReleaseError):
                payment.release()

    def test_cancel_throws_exception_if_not_ESCROWED(self):
        payment = Payment(**self.payment_arguments)
        for status in PAYMENT_STATUSES:
            if status == 'ESCROWED':
                continue
            payment.status = status
            with self.assertRaises(poundpay.payments.PaymentCancelError):
                payment.cancel()

    def test_cancel_sets_status_to_cancel_and_issues_save(self):
        kwargs = self.payment_arguments
        kwargs['status'] = 'ESCROWED'
        payment = Payment(**kwargs)
        with mock.patch.object(Payment, 'save') as patched_save:
            payment.cancel()

        patched_save.assert_called_once_with()
        self.assertEqual(payment.status, 'CANCELED')

    def test_release_sets_status_to_released_and_issues_save(self):
        kwargs = self.payment_arguments
        kwargs['status'] = 'ESCROWED'
        payment = Payment(**kwargs)
        with mock.patch.object(Payment, 'save') as patched_save:
            payment.release()

        patched_save.assert_called_once_with()
        self.assertEqual(payment.status, 'RELEASED')

    def test_escrow_sets_status_to_escrowed_and_issues_save(self):
        kwargs = self.payment_arguments
        kwargs['status'] = 'AUTHORIZED'
        payment = Payment(**kwargs)
        with mock.patch.object(Payment, 'save') as patched_save:
            payment.escrow()

        patched_save.assert_called_once_with()
        self.assertEqual(payment.status, 'ESCROWED')
*/
    }
}
