namespace Billing.Core.Enums
{
    public enum BillTxTypes
    {
        /// <summary>
        /// 웹 결재 
        /// </summary>
        WEB_PG,
        /// <summary>
        /// 구글 결재
        /// </summary>
        IAP_Google,
        /// <summary>
        /// IOS 결재
        /// </summary>
        IAP_IOS,
        /// <summary>
        /// 포인트 결재 
        /// </summary>
        POINT
    }

    public enum BillTxStatus
    {
        /// <summary>
        /// Tx 생성
        /// </summary>
        INITIATED,
        /// <summary>
        /// Tx 완료
        /// </summary>
        COMPLETED,
        /// <summary>
        /// 상품 전달 실패 
        /// </summary>
        DELIVERY_FAILED,
        /// <summary>
        /// 포인트 충전 시작 
        /// </summary>
        POINT_CHARGE_START,
        /// <summary>
        /// 포인트 충전 완료 
        /// </summary>
        POINT_CHARGE_END,
        /// <summary>
        /// 포인트 충전 실패 
        /// </summary>
        POINT_CHARGE_FAILED,
        /// <summary>
        /// 포인트 소모(결재) 시작
        /// </summary>
        POINT_SPEND_START,
        /// <summary>
        /// 포인트 소모 완료 
        /// </summary>
        POINT_SPEND_END,
        /// <summary>
        /// 포인트 소모 실패 
        /// </summary>
        POINT_SPEND_FAILED,
        /// <summary>
        /// IAP 영수증 검증 요청
        /// </summary>
        IAP_RECEIPT_PENDING,
        /// <summary>
        /// IAP 영수증 검증 완료 
        /// </summary>
        IAP_RECEIPT_VALID,
        /// <summary>
        /// IAP 영수증 검증 실패 
        /// </summary>
        IAP_RECEIPT_INVALID,
        /// <summary>
        /// 환불됨
        /// </summary>
        REFUNDED,
        /// <summary>
        /// 취소됨
        /// </summary>
        CANCLED,
        /// <summary>
        /// 만료됨
        /// </summary>
        EXPIRED
    }
}
