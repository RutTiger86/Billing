namespace Billing.Api.Enums
{
    public enum BillingErrorCode
    {
        SUCESS = 0,
        SYSTEM_EXCEPTION, // 익셉션 에러
        TX_VALIDATION_FAILED, // TX 검증 실패 
        TX_UNVERIFIABLE_TYPE, // 검증할 수 없는 Transaction Type
        PURCHASE_VALIDATION_FAILED, // 구매 검증 실패 
    }
}
