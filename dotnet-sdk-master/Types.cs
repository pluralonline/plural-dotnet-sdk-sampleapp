namespace PineLabsSdk
{
    public class Types
    {
    }
}


// Define the data structures for txn_data, customer_data, billing_data, shipping_data, udf_data, and payment_mode
public class txn_data
{
    public string txn_id { get; set; }
    public string callback { get; set; }
    public string amount_in_paisa { get; set; }
}

public class merchant_data
{
    public string merchant_id { get; set; }
    public string merchant_access_code { get; set; }
    public bool is_test { get; set; }
}

public class customer_data
{
    public string email_id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string mobile_no { get; set; }
    public string customer_id { get; set; }
}

public class billing_data
{
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string address3 { get; set; }
    public string pincode { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
}

public class shipping_data
{
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string mobile_no { get; set; }
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string address3 { get; set; }
    public string pincode { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
}

public class udf_data
{
    public string udf_field_1 { get; set; }
    public string udf_field_2 { get; set; }
    public string udf_field_3 { get; set; }
    public string udf_field_4 { get; set; }
    public string udf_field_5 { get; set; }
}

public class payment_mode
{
    public bool netbanking { get; set; }
    public bool cards { get; set; }
    public bool emi { get; set; }
    public bool upi { get; set; }
    public bool cardless_emi { get; set; }
    public bool wallet { get; set; }
    public bool debit_emi { get; set; }
    public bool prebooking { get; set; }
    public bool bnpl { get; set; }
    public bool paybypoints { get; set; }
}


public class CreateOrderResponse
{
    public string token { get; set; }
    public string redirect_url { get; set; }
    public string response_message { get; set; }
    public int response_code { get; set; }
}

public class CreateOrderReturnType
{
    public string token { get; set; }
    public string url { get; set; }
    public bool status { get; set; }
}

public class PaymentResponse
{
    public string ppc_MerchantID { get; set; }
    public string ppc_MerchantAccessCode { get; set; }
    public string ppc_PinePGTxnStatus { get; set; }
    public string ppc_TransactionCompletionDateTime { get; set; }
    public string ppc_UniqueMerchantTxnID { get; set; }
    public string ppc_Amount { get; set; }
    public string ppc_TxnResponseCode { get; set; }
    public string ppc_TxnResponseMessage { get; set; }
    public string ppc_PinePGTransactionID { get; set; }
    public string ppc_CapturedAmount { get; set; }
    public string ppc_RefundedAmount { get; set; }
    public string ppc_AcquirerName { get; set; }
    public string ppc_DIA_SECRET { get; set; }
    public string ppc_DIA_SECRET_TYPE { get; set; }
    public string ppc_PaymentMode { get; set; }
    public string ppc_Parent_TxnStatus { get; set; }
    public string ppc_ParentTxnResponseCode { get; set; }
    public string ppc_ParentTxnResponseMessage { get; set; }
    public string ppc_CustomerMobile { get; set; }
    public string ppc_UdfField1 { get; set; }
    public string ppc_UdfField2 { get; set; }
    public string ppc_UdfField3 { get; set; }
    public string ppc_UdfField4 { get; set; }
    public string ppc_AcquirerResponseCode { get; set; }
    public string ppc_AcquirerResponseMessage { get; set; }
}


public class OfferScheme
{
    public List<object> product_details { get; set; }
    public string product_code { get; set; }
    public int product_amount { get; set; }
    public int subvention_cashback_discount { get; set; }
    public int product_discount { get; set; }
    public int subvention_cashback_discount_percentage { get; set; }
    public int product_discount_percentage { get; set; }
    public int subvention_type { get; set; }
    public int bank_interest_rate_percentage { get; set; }
    public int bank_interest_rate { get; set; }
}

public class EmiScheme
{
    public int scheme_id { get; set; }
    public int program_type { get; set; }
    public bool is_scheme_valid { get; set; }
}

public class ListEmiTenure
{
    public OfferScheme offer_scheme { get; set; }
    public string tenure_id { get; set; }
    public string tenure_in_month { get; set; }
    public int monthly_installment { get; set; }
    public int bank_interest_rate { get; set; }
    public int interest_pay_to_bank { get; set; }
    public int total_offerred_discount_cashback_amount { get; set; }
    public int loan_amount { get; set; }
    public int auth_amount { get; set; }
    public EmiScheme emi_scheme { get; set; }
}

public class Issuer
{
    public List<ListEmiTenure> list_emi_tenure { get; set; }
    public string issuer_name { get; set; }
    public bool is_debit_emi_issuer { get; set; }
}

public class RootObject
{
    public List<Issuer> issuer { get; set; }
    public int response_code { get; set; }
    public string response_message { get; set; }
}