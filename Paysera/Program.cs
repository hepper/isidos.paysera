using System;
using Isidos.Paysera;

namespace Paysera
{
    class Program
    {
        static void Main(string[] args)
        {
            var request = new PayseraRequest
            {
                OrderId = "ORDER0001",
                Amount = 1000,
                Currency = "LTL",
                Country = "LT",
                Test = true,
                Version = "1.6"
            };



            var requestUrl = PayseraClient.Init().BuildRequestUrl(request);
            var repeatUrl = PayseraClient.Init().BuildRepeatRequestUrl("ORDER0001");
            

            Console.WriteLine("RequestUrl" + requestUrl);
            Console.WriteLine("RepeatUrl" + repeatUrl);
            Console.ReadLine();

        }
    }
}
