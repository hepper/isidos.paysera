namespace Isidos.Paysera
{
    using System;
    using System.Net;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    internal class CryptoUtility
    {
        public static string CalculateMd5(string text)
        {
            var md5 = MD5.Create();
            var textAsBytes = Encoding.UTF8.GetBytes(text);
            var hash = md5.ComputeHash(textAsBytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static byte[] DecodeBase64UrlSafeAsByteArray(string encodedData)
        {
            encodedData = encodedData.Replace('-', '+');
            encodedData = encodedData.Replace('_', '/');
            var data = Convert.FromBase64String(encodedData);
            return data;
        }


        public static string DownloadPublicKey()
        {
            using (var client = new WebClient())
            {
                try
                {
                    var pemUrl = AppSettings.PayseraPublicKeyUrl;
                    if (string.IsNullOrEmpty(pemUrl))
                    {
                        pemUrl = "http://downloads.webtopay.com/download/public.key";
                    }
                    return client.DownloadString(pemUrl);
                }
                catch (Exception ex)
                {
                    var messaage = string.Format("{0} : {1}", "Enable to download public key file :", ex.Message);
                    throw new Exception(messaage, ex);
                }
            }
        }


        public static byte[] GetPublicKeyRawDataFromPemFile(string pemFileContents)
        {
            const string startCertificateMark = "-----BEGIN CERTIFICATE-----";
            const string endCertificateMark = "-----END CERTIFICATE-----";
            var startCertificateIndex = pemFileContents.IndexOf(startCertificateMark, StringComparison.Ordinal);
            var endCertificateIndex = pemFileContents.IndexOf(endCertificateMark, StringComparison.Ordinal);
            var publicKeyBase64 = pemFileContents.Substring(startCertificateIndex + startCertificateMark.Length, endCertificateIndex - startCertificateIndex - endCertificateMark.Length - 2);
            publicKeyBase64 = publicKeyBase64.Trim();
            var publicKeyRawData = Convert.FromBase64String(publicKeyBase64);
            return publicKeyRawData;
        }


        public static bool VerifySs2(string data, byte[] signature, byte[] publicKeyRawData)
        {
            var c = new X509Certificate2(publicKeyRawData);
            var p = new RSACryptoServiceProvider();
            p.FromXmlString(c.PublicKey.Key.ToXmlString(false));
            var valid = p.VerifyData(Encoding.UTF8.GetBytes(data), CryptoConfig.MapNameToOID("SHA1"), signature);
            return valid;
        }
    }
}