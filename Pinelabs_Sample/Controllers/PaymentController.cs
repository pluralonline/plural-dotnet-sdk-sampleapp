using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PineLabsSdk;

namespace Pinelabs_Sample.Controllers;

class FetchResponseModal
{
    public string? data;
}

public class PaymentController : Controller
{
    private Payment? _payment;
    private EMI? _emi;
    private Hash? _hash;

    // Payment Create Page
    public IActionResult Index()
    {
        return View();
    }

    // Fetch Method Page
    public IActionResult Fetch()
    {
        ViewBag.data = TempData["fetch_data"] as string ?? "";
        return View();
    }

    public IActionResult Emi()
    {
        ViewBag.data = TempData["data"] as string ?? "";
        return View();
    }

    public IActionResult Hash()
    {
        ViewBag.data = TempData["data"] as string ?? "";
        ViewBag.isValid = TempData["isValid"] ?? false;
        return View();
    }

    // API For Fetching Payment Status
    [HttpPost("/payment/fetch/status")]
    public IActionResult Fetch([FromForm] FetchRequestModel data)
    {
        try
        {
            this._payment ??= new Payment(data.merchantId, data.access_code, data.secret, data.pg_mode);
            var response = this._payment.Fetch(data.txn_id).Result;
            TempData["fetch_data"] = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            return RedirectToAction("Fetch");
        }
        catch (Exception exception)
        {
            return Redirect("/payment/fetch");
        }
    }

    // API For Payment Creation
    [HttpPost("/payment/submit")]
    public IActionResult Submit([FromForm] PaymentRequestModel data)
    {
        try
        {
            this._payment ??= new Payment(data.merchantId, data.access_code, data.secret, data.pg_mode);

            // Txn data
            txn_data txnData = new txn_data();
            txnData.amount_in_paisa = data.amount_in_paisa.ToString();
            txnData.txn_id = data.txn_id;
            txnData.callback = data.callback_url;

            // Customer data
            customer_data customerData = new customer_data();
            customerData.customer_id = data.customer_id;
            customerData.first_name = data.first_name;
            customerData.last_name = data.last_name;
            customerData.email_id = data.email;
            customerData.mobile_no = data.phone;

            // Billing data
            billing_data billingData = new billing_data();
            billingData.address1 = data.address1;
            billingData.address2 = data.address2;
            billingData.address3 = data.address3;
            billingData.city = data.city;
            billingData.state = data.state;
            billingData.country = data.country;
            billingData.pincode = data.billing_pincode;

            // Shipping data
            shipping_data shippingData = new shipping_data();
            shippingData.address1 = data.shipping_address1;
            shippingData.address2 = data.shipping_address2;
            shippingData.address3 = data.shipping_address3;
            shippingData.first_name = data.shipping_firstname;
            shippingData.last_name = data.shipping_lastname;
            shippingData.mobile_no = data.shipping_phone;
            shippingData.city = data.shipping_city;
            shippingData.state = data.shipping_state;
            shippingData.country = data.shipping_country;
            shippingData.pincode = data.shipping_pincode;

            udf_data udfs = new udf_data();
            udfs.udf_field_1 = data.udf1;
            udfs.udf_field_2 = data.udf2;
            udfs.udf_field_3 = data.udf3;
            udfs.udf_field_4 = data.udf4;
            udfs.udf_field_5 = data.udf5;

            // Payment modes
            payment_mode modes = new payment_mode();
            modes.cards = data.payment_mode.Contains("card");
            modes.netbanking = data.payment_mode.Contains("netbanking");
            modes.wallet = data.payment_mode.Contains("wallet");
            modes.upi = data.payment_mode.Contains("upi");
            modes.emi = data.payment_mode.Contains("emi");
            modes.debit_emi = data.payment_mode.Contains("debit_emi");
            modes.cardless_emi = data.payment_mode.Contains("cardless_emi");
            modes.bnpl = data.payment_mode.Contains("bnpl");
            modes.bnpl = data.payment_mode.Contains("bnpl");
            modes.prebooking = data.payment_mode.Contains("prebooking");

            product_details[] products = data.products != null ? JsonSerializer.Deserialize<product_details[]>(data.products) : Array.Empty<product_details>();

            var response = this._payment.Create(txnData, customerData, billingData, shippingData, udfs, modes, products).Result;
            Console.Write(response?.url);
            return Redirect(response?.url ?? "/payment");
        }
        catch (Exception error)
        {
            Console.Write(error?.Message);
            return Redirect("/payment");
        }
    }

    // Emi Offer Generation Method
    [HttpPost("/payment/emi")]
    public IActionResult CalculateEmi([FromForm] EmiRequestModel data)
    {
        try
        {
            this._emi ??= new EMI(data.merchantId, data.access_code, data.pg_mode);
            var values = JsonSerializer.Deserialize<List<product_details>>(data.products).ToArray();
            var response = this._emi.CalculateEmi(data.product_amount, values);
            var formattedResponse = JsonSerializer.Serialize(response);
            TempData["data"] = formattedResponse;
            return RedirectToAction("Emi");
        }
        catch (Exception error)
        {
            Console.Write(error?.Message);
            return Redirect("/payment/emi");
        }
    }

    // Hash Verification Method
    [HttpPost("/payment/verify")]
    public IActionResult VerifyHash([FromForm] PaymentResponseModel data)
    {
        try
        {
            var response = new
            {
                ppc_MerchantID = data.merchant_id,
                ppc_MerchantAccessCode = data.access_code,
                ppc_PinePGTxnStatus = data.ppc_PinePGTxnStatus,
                ppc_TransactionCompletionDateTime = data.ppc_TransactionCompletionDateTime,
                ppc_UniqueMerchantTxnID = data.txn_id,
                ppc_Amount = data.ppc_Amount,
                ppc_TxnResponseCode = data.ppc_TxnResponseCode,
                ppc_TxnResponseMessage = data.ppc_TxnResponseMessage,
                ppc_PinePGTransactionID = data.ppc_PinePGTransactionID,
                ppc_CapturedAmount = data.ppc_CapturedAmount,
                ppc_RefundedAmount = data.ppc_RefundedAmount,
                ppc_AcquirerName = data.ppc_AcquirerName,
                ppc_PaymentMode = data.ppc_PaymentMode,
                ppc_Parent_TxnStatus = data.ppc_Parent_TxnStatus,
                ppc_ParentTxnResponseCode = data.ppc_ParentTxnResponseCode,
                ppc_ParentTxnResponseMessage = data.ppc_ParentTxnResponseMessage,
                ppc_CustomerMobile = data.ppc_CustomerMobile,
                ppc_UdfField1 = data.ppc_UdfField1,
                ppc_UdfField2 = data.ppc_UdfField2,
                ppc_UdfField3 = data.ppc_UdfField3,
                ppc_UdfField4 = data.ppc_UdfField4,
                ppc_AcquirerResponseCode = data.ppc_AcquirerResponseCode,
                ppc_AcquirerResponseMessage = data.ppc_AcquirerResponseMessage
            };
            var result = PineLabsSdk.Hash.VerifyHash(data.hash, response,
                data.secret);
            if (result)
            {
                TempData["data"] = "Hash is valid";
                TempData["isValid"] = true;
            }
            else
            {
                TempData["data"] = "Hash is invalid";
                TempData["isValid"] = false;
            }

            return RedirectToAction("Hash");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Redirect("/payment/hash");
        }
    }
}

public class PaymentRequestModel
{
    public string __RequestVerificationToken { get; set; }
    public string merchantId { get; set; }
    public string access_code { get; set; }
    public string secret { get; set; }
    public bool pg_mode { get; set; }
    public string txn_id { get; set; }
    public int amount_in_paisa { get; set; }
    public string callback_url { get; set; }
    public string products { get; set; }
    public List<string> payment_mode { get; set; }
    public string customer_id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string address3 { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public string billing_pincode { get; set; }
    public string shipping_firstname { get; set; }
    public string shipping_lastname { get; set; }
    public string shipping_phone { get; set; }
    public string shipping_address1 { get; set; }
    public string shipping_address2 { get; set; }
    public string shipping_address3 { get; set; }
    public string shipping_city { get; set; }
    public string shipping_state { get; set; }
    public string shipping_country { get; set; }
    public string shipping_pincode { get; set; }
    public string udf1 { get; set; }
    public string udf2 { get; set; }
    public string udf3 { get; set; }
    public string udf4 { get; set; }
    public string udf5 { get; set; }
    public string pay_now { get; set; }
}

public class FetchRequestModel
{
    public string __RequestVerificationToken { get; set; }
    public string merchantId { get; set; }
    public string access_code { get; set; }
    public string secret { get; set; }
    public bool pg_mode { get; set; }
    public string txn_id { get; set; }
}

public class EmiRequestModel
{
    public string __RequestVerificationToken { get; set; }

    public string merchantId { get; set; }

    public string access_code { get; set; }

    public bool pg_mode { get; set; }
    public string product_amount { get; set; }
    public string products { get; set; }
 
    public bool PayNow { get; set; }
}

public class PaymentResponseModel
{
    public string __RequestVerificationToken { get; set; }
    public string merchant_id { get; set; }
    public string access_code { get; set; }
    public string hash { get; set; }
    public string secret { get; set; }
    public bool pg_mode { get; set; }
    public string txn_id { get; set; }
    public string ppc_PinePGTransactionID { get; set; }
    public string ppc_PinePGTxnStatus { get; set; }
    public string ppc_TransactionCompletionDateTime { get; set; }
    public string ppc_Amount { get; set; }
    public string ppc_TxnResponseCode { get; set; }
    public string ppc_TxnResponseMessage { get; set; }
    public string ppc_CapturedAmount { get; set; }
    public string ppc_RefundedAmount { get; set; }
    public string ppc_AcquirerName { get; set; }
    public string ppc_PaymentMode { get; set; }
    public string ppc_Parent_TxnStatus { get; set; }
    public string ppc_ParentTxnResponseCode { get; set; }
    public string ppc_ParentTxnResponseMessage { get; set; }
    public string ppc_CustomerMobile { get; set; }
    public string ppc_AcquirerResponseCode { get; set; }
    public string ppc_AcquirerResponseMessage { get; set; }
    public string ppc_UdfField1 { get; set; }
    public string ppc_UdfField2 { get; set; }
    public string ppc_UdfField3 { get; set; }
    public string ppc_UdfField4 { get; set; }
    public string pay_now { get; set; }
}