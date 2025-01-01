namespace Billing.Core.Enums
{
    public enum BillTxTypes
    {
        WEB_PG,
        IAP_Google,
        IAP_IOS,
        POINT
    }

    public enum BillTxSubTypes
    {
        CONSUMABLE,
        NON_CONSUMABLE,
        SUBSCRIPTION_AUTO,
        SOBSCRIPTION_NON_AUTO,
        CHARGE,
        REFUND
    }

    public enum BillTxStatus
    {
        INITIATED,
        COMPLETED,
        DELIVERY_FAILED,
        POINT_CHARGE_REQUESTED,
        POINT_CHARGE_END,
        POINT_CHARGE_FAILED,
        POINT_SPEND_REQUESTED,
        POINT_SPEND,
        POINT_SPEND_FAILED,
        IAP_RECEIPT_PENDING,
        IAP_RECEIPT_VALID,
        IAP_RECEIPT_INVALID,
        REFUNDED,
        CANCLED,
        EXPIRED,
        CHARGEBACK
    }
}
