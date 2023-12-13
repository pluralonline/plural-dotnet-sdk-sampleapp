using System.Net;
using System.Text;
using System.Text.Json;

namespace PineLabsSdk;

public class EMI
{
    private readonly bool isTest;
    private readonly string merchantAccessCode;
    private readonly string merchantId;

    public EMI(string merchantId, string merchantAccessCode, bool isTest)
    {
        this.merchantId = merchantId;
        this.merchantAccessCode = merchantAccessCode;
        this.isTest = isTest;
    }

    public async Task<RootObject?> CalculateEmi(string amountInPaisa, product_details[] productDetails)
    {
        try
        {
            var body = new
            {
                merchant_data = new
                {
                    merchant_id = merchantId,
                    merchant_access_code = merchantAccessCode
                },
                payment_data = new
                {
                    amount_in_paisa = amountInPaisa
                },
                product_details = productDetails
            };

            var endpoint = isTest ? "https://uat.pinepg.in/api/" : "https://pinepg.in/api/";
            var url = endpoint + "v2/emi/calculator";

            using (var httpClient = new HttpClient())
            {
                var jsonBody = JsonSerializer.Serialize(body);

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                    return JsonSerializer.Deserialize<RootObject>(responseContent);

                throw new Exception(responseContent);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex?.Message ?? "Something went wrong");
        }
    }
}

// Define the product_details class as needed.
public class product_details
{
    // Define the properties of the product_details class based on your requirements.
    // For example:
    public string product_code { get; set; }
    public decimal product_amount { get; set; }
}