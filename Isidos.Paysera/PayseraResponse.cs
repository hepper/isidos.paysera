using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace Isidos.Paysera
{
    public class PayseraResponse
    {
        public NameValueCollection AdditionalParameters { get; set; }


        public PayseraResponse(NameValueCollection @params)
        {
            AdditionalParameters = new NameValueCollection();

            var boundParams = new List<string>();

            foreach (var prop in typeof(PayseraResponse).GetProperties())
            {
                var queryParam = @params[prop.Name];
                if (queryParam == null) 
                    continue;
                
                prop.SetValue(this, queryParam, null);
                boundParams.Add(prop.Name);
            }

            var additionalQueryParameters = @params.AllKeys.Where(x => !boundParams.Contains(x));
            foreach (var parameter in additionalQueryParameters)
            {
                AdditionalParameters.Add(parameter, @params[parameter]);
            }
        }
        
        /// <summary>
        /// Unique project number. Only activated projects can accept payments.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Order number from your system.
        /// </summary>
        public string OrderId { get; set; }


        /// <summary>
        /// It is possible to indicate the user language. If Paysera does not support the selected language, the system will automatically choose a language according to the IP address or ENG language by default. (LIT, LAV, EST, RUS, ENG, GER, POL).
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Amount in cents the client has to pay.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Payment currency (USD, EUR) you want the client to pay in. 
        /// If the selected currency cannot be accepted by a specific payment method, 
        /// the system will convert it automatically to the acceptable currency, 
        /// according to the currency rate of the day.
        /// Payamount and paycurrency answers will be sent to your website.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Payment type. 
        /// If provided, the payment will be made by the specified method (for example by using the specified bank). 
        /// If not specified, the payer will be immediately provided with the payment types to choose from. 
        /// You can get payment types in real time by using WebToPay library.
        /// </summary>
        public string Payment { get; set; }

        /// <summary>
        /// Payer's country (LT, EE, LV, GB, PL, DE). All possible types of payment in that country are immediately indicated to the payer, after selecting a country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Payment purpose visible when making the payment.
        /// </summary>
        public string PayText { get; set; }
        
        /// <summary>
        /// Payer's name received from the payment system. Sent only if the payment system provides such.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Payer's surname received from the payment system. Sent only if the payment system provides such.
        /// </summary>
        public string Surename { get; set; }

        /// <summary>
        /// Payment status:
        /// 0 - payment has no been executed
        /// 1 - payment successful
        /// 2 - payment order accepted, but not yet executed
        /// 3 - additional payment information
        /// </summary>
        public int Status { get; set; }

        public PaymentStatus PaymentStatus
        {
            get
            {
                PaymentStatus status;
                Enum.TryParse(Status.ToString(CultureInfo.InvariantCulture), out status);
                return status;
            }
        }

        /// <summary>
        /// The parameter, which allows to test the connection. 
        /// The payment is not executed, but the result is returned immediately, as if the payment has been made. 
        /// To test, it is necessary to activate the mode for a particular project by logging in and selecting: "Manage projects" -> "Payment gateway" (for a specific project) -> "Allow test payments" (check).
        /// </summary>
        public bool Test { get; set; }

        /// <summary>
        /// Payer's email address is necessary. If the email address is not received, the client will be requested to enter it. At this email address Paysera will inform the payer about the payment status.
        /// </summary>
        public string P_email { get; set; }
        
        /// <summary>
        /// It is a request number, which we receive when the user presses on the logo of the bank. 
        /// We transfer this request number to the link provided in the "callbackurl" field.
        /// </summary>
        public int RequestId { get; set; }
        
        /// <summary>
        /// Amount of the transfer in cents. It can differ, if it was converted to another currency.
        /// </summary>
        public int PayAmount { get; set; }

        /// <summary>
        /// The transferred payment currency (USD, EUR). 
        /// It can differ from the one you requested, if the currency could not be accepted by the selected payment method.
        /// </summary>
        public string PayCurrency { get; set; }

        /// <summary>
        /// A version number of Paysera system specification (API).
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Account number from which payment has been made.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// If you have provided personcode parameter when making the request, this parameter indicates whether the given personal code matches the real one. Possible values:
        /// 0 - personal code is yet unknown
        /// 1 - personal code matches
        /// 2 - personal code does not match
        /// 3 - personal code is unknown
        /// If the personal code is unknown at the moment callback is made, another callback will be made with status parameter set to 3, as soon as the personal code will be known
        /// </summary>
        public int? PersonCodeStatus { get; set; }
    }
}