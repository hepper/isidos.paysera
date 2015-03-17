using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Isidos.Paysera
{
    public class PayseraRequest
    {
        public PayseraRequest()
        {
            AdditionalParameters = new NameValueCollection();
            AllowedPayments = new List<string>();
            DisallowedPayments = new List<string>();
        }

        /// <summary>
        ///  Unique project number. Only activated projects can accept payments.
        /// </summary>
        [Required]
        [StringLength(11)]
        public string ProjectId { get; set; }

        ///// <summary>
        ///// Project password, which can be found by logging in to Paysera system using your user data, selecting “Service management” and “General settings” by a specific project.
        ///// </summary>
        //[Required]
        //[StringLength(11)]
        //public string SignPassword { get; set; }

        /// <summary>
        /// Full address (URL), to which the client is directed after a successful payment.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string AcceptUrl { get; set; }

        /// <summary>
        /// Full address (URL), to which a seller will get information about performed payment.
        /// Script must return text "OK". Only then our system will register, that information about the payment has been received.
        ///  If there is no answer "OK", the message will be sent 4 times (when we get it, after an hour, after three hours and after 24 hours).
        /// </summary>
        [Required]
        [StringLength(255)]
        public string CallbackUrl { get; set; }
        
        /// <summary>
        /// Full address (URL), to which the client is directed after an unsuccessful payment or cancellation.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string CancelUrl { get; set; }

        /// <summary>
        /// The version number of Paysera system specification (API).
        /// </summary>
        [Required]
        [StringLength(9)]
        public string Version { get; set; }

        /// <summary>
        /// Order number from your system.
        /// </summary>
        [StringLength(40)]
        public string OrderId { get; set; }

        public bool? RepeatRequest { get; set; }

        /// <summary>
        /// The parameter, which allows to test the connection. 
        /// The payment is not executed, but the result is returned immediately, 
        /// as if the payment has been made. 
        /// To test, it is necessary to activate the mode for a particular project by logging in and selecting: 
        /// "Manage projects" -> "Payment gateway" (for a specific project) -> "Allow test payments" (check).
        /// </summary>
        public bool? Test { get; set; }
        public DateTime TimeLimit { get; set; }
        [StringLength(255)]
        public string PersonCode { get; set; }
        
        
        
        /// <summary>
        /// It is possible to indicate the user language. 
        /// If Paysera does not support the selected language, 
        /// the system will automatically choose a language according to the IP address or ENG language by default. 
        /// (LIT, LAV, EST, RUS, ENG, GER, POL).
        /// </summary>
        [StringLength(3)]
        public string Language { get; set; }
        
        /// <summary>
        /// Amount in cents the client has to pay
        /// </summary>
        public int? Amount { get; set; }
        
        /// <summary>
        /// Payment currency (USD, EUR) you want the client to pay in. 
        /// If the selected currency cannot be accepted by a specific payment method, the system will convert it automatically to the acceptable currency, 
        /// according to the currency rate of the day. Payamount and paycurrency answers will be sent to your website.
        /// </summary>
        [StringLength(3)]
        public string Currency { get; set; }

        /// <summary>
        /// Payment type. If provided, the payment will be made by the method specified (for example by using the specified bank). 
        /// If not specified, the payer will be immediately provided with the payment types to choose from. 
        /// You can get payment types in real time by using WebToPay library.
        /// </summary>
        [StringLength(20)]
        public string Payment { get; set; }

        /// <summary>
        /// Payer's country (LT, EE, LV, GB, PL, DE). 
        /// All possible types of payment in that country are immediately indicated to the payer, after selecting a country.
        /// </summary>
        [StringLength(2)]
        public string Country { get; set; }
        
        /// <summary>
        /// Payment purpose visible when making the payment. If not specified, default text is used:
        /// Payment for goods and services (for nb. [order_nr]) ([site_name]).
        /// If you specify the payment purpose, it is necessary to include the following variables, which will be replaced with the appropriate values in the final purpose text:
        /// [order_nr] - payment number.
        /// [site_name] or [owner_name] - website address or company name.
        /// If these variables are not specified, the default purpose text will be used.
        /// Example of a payment purpose:
        /// Payment for goods made to order [order_nr] in website [site_name].
        /// </summary>
        [StringLength(255)]
        public string PayText { get; set; }
        
        /// <summary>
        /// Payer's name. Requested in the majority of payment methods. Necessary for certain payment methods.
        /// </summary>
        [StringLength(255)]
        public string PayerFirstName { get; set; }
        
        /// <summary>
        /// Payer's surname. Requested in the majority of payment methods. Necessary for certain payment methods.
        /// </summary>
        [StringLength(255)]
        public string PayerLastName { get; set; }
        
        /// <summary>
        /// Payer's email address is necessary. If the email address is not received, the client will be requested to enter it. At this email address Paysera will inform the payer about the payment status.
        /// </summary>
        [StringLength(255)]
        public string PayerEmail { get; set; }

        /// <summary>
        /// Payer's address, to which goods will be sent (e.g.: Mėnulio g. 7 - 7). Necessary for certain payment methods.
        /// </summary>
        [StringLength(255)]
        public string PaypalStreet { get; set; }
        
        /// <summary>
        /// Payer's city, to which goods will be sent (e.g.: Vilnius). Necessary for certain payment methods.
        /// </summary>
        [StringLength(255)]
        public string PayerCity { get; set; }

        /// <summary>
        /// Payer's state code (necessary, when buying in USA). Necessary for certain payment methods.
        /// </summary>
        [StringLength(255)]
        public string PayerState { get; set; }

        /// <summary>
        /// Payer's postal code. Lithuanian postal codes can be found here. Necessary for certain payment methods.
        /// </summary>
        [StringLength(255)]
        public string PayerZip { get; set; }

        /// <summary>
        /// Payer's country code. The list with country codes can be found here. Necessary for certain payment methods.
        /// </summary>
        [StringLength(255)]
        public string PayerCountryCode { get; set; }

        /// <summary>
        /// Show only those payment methods that are separated by commas.
        /// </summary>
        public List<string> AllowedPayments { get; set; }

        /// <summary>
        /// Hide payment methods separated by comma.
        /// </summary>
        public List<string> DisallowedPayments { get; set; }
        public NameValueCollection AdditionalParameters { get; set; }
        
        public string ToBase64String()
        {
            var dateFormat = string.IsNullOrEmpty(AppSettings.PayseraDateFormat) ? "yyyy-MM-dd HH:mm:ss" : AppSettings.PayseraDateFormat;
            if (TimeLimit == DateTime.MinValue)
            {
                TimeLimit = DateTime.Now.AddMinutes(60);
            }

            var @params = new NameValueCollection
            {
                {"projectid", ProjectId},
                {"orderid", OrderId},
                {"accepturl", AcceptUrl},
                {"cancelurl", CancelUrl},
                {"callbackurl", CallbackUrl},
                {"version", Version},
                {"lang", Language},
                {"amount", Amount.HasValue ? Amount.Value.ToString(CultureInfo.InvariantCulture) : string.Empty},
                {"currency", Currency},
                {"payment", Payment},
                {"country", Country},
                {"paytext", PayText},
                {"p_firstname", PayerFirstName},
                {"p_lastname", PayerLastName},
                {"p_email", PayerEmail},
                {"p_street", PaypalStreet},
                {"p_city", PayerCity },
                {"p_state", PayerState},
                {"p_zip", PayerZip},
                {"p_countrycode", PayerCountryCode},
                {"time_limit", TimeLimit.ToString(dateFormat)},
                {"personcode", PersonCode},
                {"test", Test.ToString("1")},
                {"repeatrequest", RepeatRequest.ToString("0")}
            };

            if (AllowedPayments.Any())
            {
                @params.Add("only_payments", AllowedPayments.Aggregate((i, j) => i + "," + j));
                
            }
            if (DisallowedPayments.Any())
            {
                @params.Add("disalow_payments", DisallowedPayments.Aggregate((i, j) => i + "," + j));
            }

            // Add additional parameter if he is not already in the list of params
            var items = AdditionalParameters.AllKeys.SelectMany(AdditionalParameters.GetValues, (k, v) => new { key = k, value = v });
            
            foreach (var item in items)
            {
                if (!@params.AllKeys.Contains(item.key))
                {
                    @params.Add(item.key, item.value);
                }
            }

            return @params.ToQueryString().EncodeBase64UrlSafe();
        }

        
    }

}