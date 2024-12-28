namespace Billing.Api.Enums
{
    public enum BillingErrorCode
    {
        Success = 0,
        SYSTEM_EXCEPTION, // 익셉션 에러
        TX_VALIDATION_FAILED, // TX 검증 실패 
    }
}
