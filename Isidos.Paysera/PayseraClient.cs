using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Isidos.Paysera
{
    public class PayseraClient
    {
        protected static string projectId;
        protected static string signPassword;
        public static PayseraClient Instance;
        
        // Lock synchronization object
        private static readonly object syncLock = new object();

        public static PayseraClient Init()
        {
            return Init(AppSettings.ProjectId, AppSettings.SignPassword);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pId">Project Id - Unique project number. Only approved projects can accept payments</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static PayseraClient Init(string pId, string password = "")
        {

            // Support multithreaded applications through
            // 'Double checked locking' pattern which (once
            // the instance exists) avoids locking each
            // time the method is invoked
            if (Instance == null)
            {
                lock (syncLock)
                {
                    if (Instance == null)
                    {
                        Instance = new PayseraClient(pId, password);
                    }
                }
            }

            return Instance;
        }
        
        protected PayseraClient(string pId, string password)
        {
            if (pId.ToInt(-1) < 0)
            {
                throw new ArgumentOutOfRangeException("pId", "Project Id should be initialized. Unique project number. Only activated projects can accept payments");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password", "Sign password missing");
            }
            projectId = pId;
            signPassword = password;
        }

        public string BuildRequestUrl(PayseraRequest request)
        {
            // If request don't provide some data we are trying get it either from client initialization or from application settings

            if (string.IsNullOrEmpty(request.ProjectId))
            {
                request.ProjectId = projectId;
            }
            if (string.IsNullOrEmpty(request.Version))
            {
                request.Version = AppSettings.Version;
            }
            if (string.IsNullOrEmpty(request.AcceptUrl))
            {
                request.AcceptUrl = AppSettings.AcceptUrl;
            }
            if (string.IsNullOrEmpty(request.CancelUrl))
            {
                request.CancelUrl = AppSettings.CancelUrl;
            }
            if (string.IsNullOrEmpty(request.CallbackUrl))
            {
                request.CallbackUrl = AppSettings.CallbackUrl;
            }
            
            var validationResult = request.Validate();

            if (validationResult.HasError)
            {
                var errorsOfRequestObject = validationResult.Errors.Select(x => x.ErrorMessage);
                var combinedMessage = errorsOfRequestObject.Aggregate((i, j) => i + "\n" + j);
                throw new ArgumentException("request", combinedMessage);
            }

            var data = request.ToBase64String();
            var sign = CryptoUtility.CalculateMd5(data + signPassword);

            var requestQueryParams = new NameValueCollection
            {
                {"data", data},
                {"sign", sign}
            };
            var requestQuery = requestQueryParams.ToQueryString();
            return string.Format("{0}?{1}", AppSettings.PayUrl, requestQuery);
        }
        
        public string BuildRepeatRequestUrl(string orderId)
        {
            var request = new PayseraRequest
            {
                ProjectId = projectId,
                OrderId = orderId, 
                RepeatRequest = true,
                Version = AppSettings.Version
            };
            return BuildRequestUrl(request);
        }

        private PayseraResponse GetCallbackData(NameValueCollection query)
        {
            var dataAsBase64 = query["data"];
            var ss2AsBase64 = query["ss2"];

            if (string.IsNullOrEmpty(dataAsBase64))
            {
                throw new ArgumentNullException("query", "Sorry, we are missing query params like : pId, version, ... Please correct your request before next request");
            }

            if (string.IsNullOrEmpty(ss2AsBase64))
            {
                throw new ArgumentNullException("query",
                    @"Sorry, we cannot establish secure channel from response. That's important part for communication though");
            }

            bool isValidCommunication;

            try
            {
                var publicKeyPemFileContents = CryptoUtility.DownloadPublicKey();
                var publicKeyRawData = CryptoUtility.GetPublicKeyRawDataFromPemFile(publicKeyPemFileContents);
                var ss2 = CryptoUtility.DecodeBase64UrlSafeAsByteArray(ss2AsBase64);

                isValidCommunication = CryptoUtility.VerifySs2(dataAsBase64, ss2, publicKeyRawData);
                
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error occured during communication channel validation", e);
            }

            if (!isValidCommunication)
            {
                throw new InvalidOperationException("Signed data validation failed (SS2).");
            }

            var dataQueryParams = HttpUtility.ParseQueryString(dataAsBase64.DecodeBase64UrlSafe());

            var payseraCallbackData = new PayseraResponse(dataQueryParams);
            
            if (payseraCallbackData == null || payseraCallbackData.ProjectId != projectId)
            {
                throw new Exception("Bad project Id. Should be " + projectId + ".");
            }

            return payseraCallbackData;
        }

    }
}