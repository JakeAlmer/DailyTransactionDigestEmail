using System;

namespace PlaidApi.Entities
{
    public class GetTransactionsResult
    {
        public Account[] accounts { get; set; }
        public Item item { get; set; }
        public string request_id { get; set; }
        public int total_transactions { get; set; }
        public Transaction[] transactions { get; set; }

        public class Item
        {
            public string[] available_products { get; set; }
            public string[] billed_products { get; set; }
            public string error { get; set; }
            public string institution_id { get; set; }
            public string item_id { get; set; }
            public string webhook { get; set; }
        }

        public class Account
        {
            public string account_id { get; set; }
            public Balances balances { get; set; }
            public string mask { get; set; }
            public string name { get; set; }
            public string official_name { get; set; }
            public string subtype { get; set; }
            public string type { get; set; }
        }

        public class Balances
        {
            public double? available { get; set; }
            public double? current { get; set; }
            public string iso_currency_code { get; set; }
            public double? limit { get; set; }
            public object unofficial_currency_code { get; set; }
        }

        public class Transaction
        {
            public string account_id { get; set; }
            public string account_owner { get; set; }
            public double? amount { get; set; }
            public string[] category { get; set; }
            public string category_id { get; set; }
            public DateTime date { get; set; }
            public string iso_currency_code { get; set; }
            public Location location { get; set; }
            public string name { get; set; }
            public Payment_Meta payment_meta { get; set; }
            public bool pending { get; set; }
            public string pending_transaction_id { get; set; }
            public string transaction_id { get; set; }
            public string transaction_type { get; set; }
            public string unofficial_currency_code { get; set; }
        }

        public class Location
        {
            public string address { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public decimal? lat { get; set; }
            public decimal? lon { get; set; }
            public string postal_code { get; set; }
            public string region { get; set; }
            public string store_number { get; set; }
        }

        public class Payment_Meta
        {
            public string by_order_of { get; set; }
            public string payee { get; set; }
            public string payer { get; set; }
            public string payment_method { get; set; }
            public string payment_processor { get; set; }
            public string ppd_id { get; set; }
            public string reason { get; set; }
            public string reference_number { get; set; }
        }
    }



    public class GetAccountsResult
    {
        public Account[] accounts { get; set; }
        public string request_id { get; set; }

        public class Account
        {
            public string account_id { get; set; }
            public Balances balances { get; set; }
            public string mask { get; set; }
            public string name { get; set; }
            public string official_name { get; set; }
            public string subtype { get; set; }
            public string type { get; set; }
            public object verification_status { get; set; }
        }

        public class Balances
        {
            public int? available { get; set; }
            public int current { get; set; }
            public int? limit { get; set; }
            public string iso_currency_code { get; set; }
            public object unofficial_currency_code { get; set; }
        }
    }
}
