using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PineLabsSdk;

public class Payment
{
    private readonly bool isTest;
    private readonly string merchantAccessCode;
    private readonly string merchantId;
    private readonly string merchantSecret;

    public Payment(string merchantId, string merchantAccessCode, string merchantSecret, bool isTest)
    {
        this.merchantId = merchantId;
        this.merchantAccessCode = merchantAccessCode;
        this.merchantSecret = merchantSecret;
        this.isTest = isTest;
    }

    public async Task<CreateOrderReturnType?> Create(txn_data txnData, customer_data customerData,
        billing_data billingData,
        shipping_data shippingData, udf_data udfData, payment_mode paymentModes, product_details[] products = null)
    {
        try
        {
            var methods = GetPaymentMethods(paymentModes);
            var body = new
            {
                merchant_data = new
                {
                    merchant_id = merchantId,
                    merchant_access_code = merchantAccessCode,
                    unique_merchant_txn_id = txnData?.txn_id,
                    merchant_return_url = txnData?.callback
                },
                payment_data = new
                {
                    txnData?.amount_in_paisa
                },
                txn_data = new
                {
                    navigation_mode = 2,
                    payment_mode = string.Join(",", methods),
                    transaction_type = 1
                },
                customer_data = new
                {
                    customerData,
                    billing_data = billingData,
                    shipping_data = shippingData
                },
                udf_data = udfData,
                product_details = products ?? Array.Empty<product_details>()
            };

            var endpoint = isTest ? "https://uat.pinepg.in/api/" : "https://pinepg.in/api/";
            var url = endpoint + "v2/accept/payment";

            using (var httpClient = new HttpClient())
            {
                var jsonBody = JsonSerializer.Serialize(body);
                var base64Body = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonBody));
                var hash = Hash.GenerateCreateOrderHash(base64Body, merchantSecret);
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("X-VERIFY", hash);
                request.Content = new StringContent("{\"request\":\"" + base64Body + "\"}", Encoding.UTF8,
                    "application/json");
                var response = await httpClient.SendAsync(request);
                request.Dispose();
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var data = JsonSerializer.Deserialize<CreateOrderResponse>(responseContent);
                    var returnData = new CreateOrderReturnType
                    {
                        status = true,
                        url = data?.redirect_url ?? "",
                        token = data?.token ?? ""
                    };
                    return returnData;
                }

                throw new Exception(responseContent);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex?.Message ?? "Something went wrong");
        }
    }

    public async Task<PaymentResponse?> Fetch(string txnId, int txnType = 3)
    {
        try
        {
            var body = new
            {
                ppc_MerchantID = merchantId,
                ppc_MerchantAccessCode = merchantAccessCode,
                ppc_TransactionType = txnType,
                ppc_UniqueMerchantTxnID = txnId
            };

            var endpoint = isTest ? "https://uat.pinepg.in/api/" : "https://pinepg.in/api/";
            var url = endpoint + "PG/V2";

            using (var httpClient = new HttpClient())
            {
                var hash = Hash.GenerateFetchOrderHash(body, merchantSecret);
                var dataString = CreateDataString(new
                {
                    ppc_MerchantID = merchantId,
                    ppc_MerchantAccessCode = merchantAccessCode,
                    ppc_TransactionType = txnType,
                    ppc_UniqueMerchantTxnID = txnId,
                    ppc_DIA_SECRET = hash,
                    ppc_DIA_SECRET_TYPE = "sha256"
                });
                var content = new StringContent(dataString, Encoding.UTF8,
                    MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"));
                var response = await httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return JsonSerializer.Deserialize<PaymentResponse>(responseContent);

                throw new Exception(responseContent);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex?.Message ?? "Something went wrong");
        }
    }

    private string CreateDataString(object data)
    {
        var sortedKeys = data.GetType().GetProperties()
            .Select(property => new { Key = property.Name, Value = property.GetValue(data) })
            .OrderBy(item => item.Key)
            .Select(item => $"{item.Key}={item.Value}")
            .ToArray();

        return string.Join("&", sortedKeys);
    }

    private string GetPaymentMethods(payment_mode paymentMode)
    {
        var methods = new List<string>();
        var conditions =
            new[]
            {
                new { condition = paymentMode.cards, value = "1" },
                new { condition = paymentMode.netbanking, value = "3" },
                new { condition = paymentMode.emi, value = "4" },
                new { condition = paymentMode.cardless_emi, value = "19" },
                new { condition = paymentMode.upi, value = "10" },
                new { condition = paymentMode.wallet, value = "11" },
                new { condition = paymentMode.debit_emi, value = "14" },
                new { condition = paymentMode.prebooking, value = "16" },
                new { condition = paymentMode.bnpl, value = "17" },
                new { condition = paymentMode.paybypoints, value = "20" }
            };

        foreach (var condition in conditions)
            if (condition.condition)
                methods.Add(condition.value);

        return string.Join(',', methods);
    }
}