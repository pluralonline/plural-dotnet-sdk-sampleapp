# PineLabs .Net SDK

This SDK offers simple to use api for integrating PineLabs api in your .net applications. It provide several easy
methods for creating, fetching orders and calculate EMIs and verify hash.

## Installation

In order to install this SDK locally you'll need to follow the following steps.

1. Download the sdk and extract it in a folder somewhere in your system.
2. Setup a new project or open a existing project in your system.
3. Now add reference to the SDK's DLL file on this path `SDK\bin\Debug\net7.0\PineLabsSdk.dll`
4. Add reference to the file in your `.csproj` file like following

```xml
<ItemGroup>
    <Reference Include="PineLabsSdk">
        <HintPath>SDK\bin\Debug\net7.0\PineLabsSdk.dll</HintPath>
    </Reference>
</ItemGroup>
```

5. That's it now you should be able to use it in your project.

---

## Usage For SDK

### Create Instance of PineLabs SDK

Add `using PineLabsSdk;` at the top of the file to import the sdk. Now create instance of the `payment` class as
following it takes several parameters

1. Merchant ID (string) : Merchant ID provided by PineLabs
2. Merchant Access Code (string) : Merchant Access Code Provided by PineLabs
3. Merchant Secret (string) : Merchant Secret
4. isTest (boolean) : If using test mode then set this to `true`

```csharp
    var pinelabs = new Payment("{merchantId}", "{merchantAccessCode}", "{merchantSecret}", isTestMode); 
```

---

### Create Order

This section explains how to create order for payment processing. There are a couple of things required in order to
create an order.

### Parameters  Required & Optional

```csharp
    // Transaction Data ( Mandatory )
var txn_data = new txn_data(){
    txn_id= "", // String
    callback= '', // String
    amount_in_paisa= '1000', // String
}
```

```csharp
    // Customer Data ( Optional )
var customer_data = new customer_data(){
    email_id= "", // String
    first_name= "", // String
    last_name= "", // String
    mobile_no= "", // String
    customer_id= "", // String
}
```

```csharp
    // Billing Data ( Optional )
var billing_data = new billing_data(){
    address1: "", // String
    address2: "", // String
    address3: "", // String
    pincode: "", // String
    city: "", // String
    state: "", // String
    country: "", // String
}
```

```csharp
    // Shipping Data ( Optional )
var shipping_data = new shipping_data(){
    first_name: "", // String
    last_name: "", // String
    mobile_no: "", // String
    address1: "", // String
    address2: "", // String
    address3: "", // String
    pincode: "", // String
    city: "", // String
    state: "", // String
    country: "", // String
}
```

```csharp
    // UDF data ( Optional )
var udf_data = new udf_data(){
    udf_field_1: "", // String
    udf_field_2: "", // String
    udf_field_3: "", // String
    udf_field_4: "", // String
    udf_field_5: "", // String
}
```

```csharp
    // Payment Modes That Needs To Be Shown ( Mandatory )
var payment_mode = new payment_mode(){
    netbanking: true, // Boolean
    cards: true, // Boolean
    emi: true, // Boolean
    upi: true, // Boolean
    cardless_emi: true, // Boolean
    wallet: true, // Boolean
    debit_emi: true, // Boolean
    prebooking: true, // Boolean
    bnpl: true, // Boolean
    paybypoints: false, // Boolean
}
```

```csharp
    // Products ( required only for multi cart ) If only EMI is selected then the amount needs to be equal to the sum of all product_amounts in this array and no other payment mode should be selected otherwise EMI offers will be shown.
product_details[] products = new[]
{
    new product_details()
    {
        product_code = "",
        product_amount = 20000
    },
    new product_details()
    {
        product_code = "",
        product_amount = 20000
    }
};
```

---

### Order Creation

Using the instance of the SDK we created above we will call the `.create()` method on the `payment` interface for
creating an order with the provided parameters. It takes the following positional arguments

1. Transaction Data
2. Payment Modes
3. Customer Data
4. Billing Data
5. Shipping Data
6. UDF Data
7. Product Details

The `create()` method returns a promise with the response or else throws an error if something went wrong.

```csharp
// Create Order
var response = await pinelabs.Create(new txn_data(), new customer_data(), new billing_data(), new shipping_data(), new udf_data(), new payment_mode(), new product_details[]);

Console.WriteLine(JsonSerializer.Serialize(response));
```

---

#### Success Response

```json
{
  "status": true,
  "url": "https://uat.pinepg.in/pinepg/v2/process/payment?token=S01wPSlIH%2bopelRVif7m7e4SgrTRIcKYx25YDYfmgtbPOE%3d"
}
```

#### Failure Response

```text
Fatal error: Uncaught Exception: MERCHANT PASSWORD DOES NOT MATCH
```

---

### Fetch Order

Using the instance of the SDK we created above we will call the `.fetch()` method on the `payment` interface for
fetching
an order details with the provided transaction id and transaction type. It takes the following positional arguments

1. Transaction ID
2. Transaction Type

```csharp
var response = await pinelabs.Fetch("650acb67d3752", 3);
Console.WriteLine(JsonSerializer.Serialize(response));
```

---

#### Success Response

```json
{
  "ppc_MerchantID": "106600",
  "ppc_MerchantAccessCode": "bcf441be-411b-46a1-aa88-c6e852a7d68c",
  "ppc_PinePGTxnStatus": "7",
  "ppc_TransactionCompletionDateTime": "20\/09\/2023 04:07:52 PM",
  "ppc_UniqueMerchantTxnID": "650acb67d3752",
  "ppc_Amount": "1000",
  "ppc_TxnResponseCode": "1",
  "ppc_TxnResponseMessage": "SUCCESS",
  "ppc_PinePGTransactionID": "12069839",
  "ppc_CapturedAmount": "1000",
  "ppc_RefundedAmount": "0",
  "ppc_AcquirerName": "BILLDESK",
  "ppc_DIA_SECRET": "D640CFF0FCB8D42B74B1AFD19D97A375DAF174CCBE9555E40CC6236964928896",
  "ppc_DIA_SECRET_TYPE": "SHA256",
  "ppc_PaymentMode": "3",
  "ppc_Parent_TxnStatus": "4",
  "ppc_ParentTxnResponseCode": "1",
  "ppc_ParentTxnResponseMessage": "SUCCESS",
  "ppc_CustomerMobile": "7737291210",
  "ppc_UdfField1": "",
  "ppc_UdfField2": "",
  "ppc_UdfField3": "",
  "ppc_UdfField4": "",
  "ppc_AcquirerResponseCode": "0300",
  "ppc_AcquirerResponseMessage": "NA"
}
```

#### Failure Response

```json
{
  "ppc_MerchantID": "106600",
  "ppc_MerchantAccessCode": "bcf441be-411b-46a1-aa88-c6e852a7d68c",
  "ppc_PinePGTxnStatus": "-6",
  "ppc_TransactionCompletionDateTime": "21\/09\/2023 11:29:48 PM",
  "ppc_UniqueMerchantTxnID": "106600_2109202323294890763",
  "ppc_TxnResponseCode": "-40",
  "ppc_TxnResponseMessage": "INVALID DATA",
  "ppc_CapturedAmount": "0",
  "ppc_RefundedAmount": "0",
  "ppc_DIA_SECRET": "4B9DD62C1CE94C354E368A2DA1C51C2E8ED16ABDC46414B8AAA60F378CDCE390",
  "ppc_DIA_SECRET_TYPE": "SHA256"
}
```

#### Incorrect Merchant Details

```textmate
"IP Access Denied"
```

---

### Creating Instance Of EMI Class

With the EMI calculation you'll need to create an instance of the EMI class. It takes the same arguments as Payment
class except it doesn't require merchant secret which are as follows:

1. Merchant ID (string) : Merchant ID provided by PineLabs
2. Merchant Access Code (string) : Merchant Access Code Provided by PineLabs
3. isTest (boolean) : If using test mode then set this to `true`

```csharp
var emi = new EMI("{merchantId}", "{merchantAccessCode}", isTestMode);
```

---

### EMI Calculator

Using the instance of the SDK we created above we will call the `.calculate()` method on the `emi` interface for
fetching
offers for EMI with the provided product details. It takes the following positional arguments

1. Total amount of all products in the array
2. Product details ( Array of products )

```csharp
// Create a product
var product1 = new product_details();
product1.product_code = "testSKU1"
product1.product_amount = 500000

// Add that product in a array of products
product_details[] details = {product1}

// pass it the total amount of the product and the array of products
var response = await emi.CalculateEmi("500000", details)
    
Console.WriteLine(JsonSerializer.Serialize(response))
```

---

#### Success Response

```json
{
  "issuer": [
    {
      "list_emi_tenure": [
        {
          "offer_scheme": {
            "product_details": [
              {
                "schemes": [],
                "product_code": "testproduct02",
                "product_amount": 10000,
                "subvention_cashback_discount": 0,
                "product_discount": 0,
                "subvention_cashback_discount_percentage": 0,
                "product_discount_percentage": 0,
                "subvention_type": 3,
                "bank_interest_rate_percentage": 150000,
                "bank_interest_rate": 251
              }
            ],
            "emi_scheme": {
              "scheme_id": 48040,
              "program_type": 105,
              "is_scheme_valid": true
            }
          },
          "tenure_id": "3",
          "tenure_in_month": "3",
          "monthly_installment": 3417,
          "bank_interest_rate": 150000,
          "interest_pay_to_bank": 251,
          "total_offerred_discount_cashback_amount": 0,
          "loan_amount": 10000,
          "auth_amount": 10000
        },
        {
          "offer_scheme": {
            "product_details": [
              {
                "schemes": [],
                "product_code": "testproduct02",
                "product_amount": 10000,
                "subvention_cashback_discount": 0,
                "product_discount": 0,
                "subvention_cashback_discount_percentage": 0,
                "product_discount_percentage": 0,
                "subvention_type": 3,
                "bank_interest_rate_percentage": 150000,
                "bank_interest_rate": 440
              }
            ],
            "emi_scheme": {
              "scheme_id": 48040,
              "program_type": 105,
              "is_scheme_valid": true
            }
          },
          "tenure_id": "6",
          "tenure_in_month": "6",
          "monthly_installment": 1740,
          "bank_interest_rate": 150000,
          "interest_pay_to_bank": 440,
          "total_offerred_discount_cashback_amount": 0,
          "loan_amount": 10000,
          "auth_amount": 10000
        }
      ],
      "issuer_name": "HDFC",
      "is_debit_emi_issuer": false
    }
  ],
  "response_code": 1,
  "response_message": "SUCCESS"
}
```

#### Failure Response

```text
Fatal error: Uncaught Exception: INVALID DATA,MISMATCH_IN_TOTAL_CART_AMOUNT_AND_TOTAL_PRODUCT_AMOUNT
```
---
### Verify Hash

Using the instance of the SDK we created above we will call the `.VerifyHash()` method on the `hash` class for
verifying
a hash received in the response of callback and webhooks. It takes the following positional arguments

1. Hash Received in Response ( `DIA_SECRET` )
2. Response Received ( Not including `DIA_SECRET` and `DIA_SECRET_TYPE` )

```csharp
var isVerified = Hash.VerifyHash("D640CFF0FCB8D42B74B1AFD19D97A375DAF174CCBE9555E40CC6236964928896", new {
    ppc_MerchantID = "106600",
    ppc_MerchantAccessCode = "bcf441be-411b-46a1-aa88-c6e852a7d68c",
    ppc_PinePGTxnStatus = "7",
    ppc_TransactionCompletionDateTime = "20/09/2023 04:07:52 PM",
    ppc_UniqueMerchantTxnID = "650acb67d3752",
    ppc_Amount = "1000",
    ppc_TxnResponseCode = "1",
    ppc_TxnResponseMessage = "SUCCESS",
    ppc_PinePGTransactionID = "12069839",
    ppc_CapturedAmount = "1000",
    ppc_RefundedAmount = "0",
    ppc_AcquirerName = "BILLDESK",
    ppc_PaymentMode = "3",
    ppc_Parent_TxnStatus = "4",
    ppc_ParentTxnResponseCode = "1",
    ppc_ParentTxnResponseMessage = "SUCCESS",
    ppc_CustomerMobile = "7737291210",
    ppc_UdfField1 = "",
    ppc_UdfField2 = "",
    ppc_UdfField3 = "",
    ppc_UdfField4 = "",
    ppc_AcquirerResponseCode = "0300",
    ppc_AcquirerResponseMessage = "NA"
}, "9A7282D0556544C59AFE8EC92F5C85F6");

Console.WriteLine(isVerified);
```
