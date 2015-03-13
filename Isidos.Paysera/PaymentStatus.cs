namespace Isidos.Paysera
{
    /// <summary>
    /// Payment status:
    /// 0 - payment has no been executed
    /// 1 - payment successful
    /// 2 - payment order accepted, but not yet executed
    /// 3 - additional payment information
    /// </summary>
    public enum PaymentStatus
    {
        NotExecuted = 0,
        Successful = 1,
        AcceptedButNotExecuted = 2,
        AdditionalPaymentInformation = 3
    }
}