namespace Billing.Core.Enums
{
    public enum BillingError
    {
        /// <summary>
        /// 에러없음
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 시스템 에러
        /// </summary>
        SYSTEM_ERROR,  
        /// <summary>
        /// 존재하지 않는 Tx
        /// </summary>
        TX_NOT_FOUND,       
        /// <summary>
        /// 이미 완료된 Tx
        /// </summary>
        TX_ALREADY_IS_DONE,
        /// <summary>
        /// 취소된 Tx
        /// </summary>
        TX_CANCLED,  
        /// <summary>
        /// 지급 진행주인 Tx
        /// </summary>
        TX_ALREADY_INPROGRESS,
        /// <summary>
        /// 검증 불가 타입
        /// </summary>
        TX_UNVERIFIABLE_TYPE,
        /// <summary>
        /// 구매 검증 실패
        /// </summary>
        PURCHASE_VALIDATION_FAILED,
        /// <summary>
        /// 구글 검증 에러 
        /// </summary>
        PURCHASE_GOOGLE_VALIDATE_ERROR,
        /// <summary>
        /// 포인트 검증/구매 에러 
        /// </summary>
        PURCHASE_POINT_VALIDATE_ERROR,
        /// <summary>
        /// 빌링 정보 생성 에러 
        /// </summary>
        BILL_CREATE_DETAIL_FAILED,
        /// <summary>
        /// 빌링 정보 완료 에러 
        /// </summary>
        BILL_COMPLETE_DETAIL_FAILED,
        /// <summary>
        /// Tx 완료 에러
        /// </summary>
        TX_COMPLETE_FAILED,
        /// <summary>
        /// 존재 하지 않는 Tx
        /// </summary>
        SUBSCRIPTION_NOTFFOUND,
        /// <summary>
        /// 알려지지 않은 에러
        /// </summary>
        UNKNOWN                
    }


    public enum BillProductType
    {
        /// <summary>
        /// 소모성 상품
        /// </summary>
        CONSUMABLE,
        /// <summary>
        /// 비 소모성 상품 
        /// </summary>
        NON_CONSUMABLE,
        /// <summary>
        /// 자동 구독 상품
        /// </summary>
        SUBSCRIPTION_AUTO,
        /// <summary>
        /// 구독 상품 
        /// </summary>
        SUBSCRIPTION_NON_AUTO,
        /// <summary>
        /// 환불
        /// </summary>
        REFUND
    }


    public enum SubScriptionState
    {
        /// <summary>
        /// 지정되지 않은 정기 결제 상태입니다.
        /// </summary>
        SUBSCRIPTION_STATE_UNSPECIFIED,
        /// <summary>
        /// 정기 결제가 생성되었지만 가입하는 동안 결제를 기다리고 있습니다. 이 상태에서는 모든 항목이 결제를 기다리고 있습니다.
        /// </summary>
        SUBSCRIPTION_STATE_PENDING,
        /// <summary>
        /// 구독이 활성 상태입니다. - (1) 정기 결제가 자동 갱신 요금제인 경우 하나 이상의 항목이 autoRenewEnabled이고 만료되지 않았습니다. - (2) 정기 결제가 선불 요금제인 경우 하나 이상의 항목이 만료되지 않았습니다.
        /// </summary>
        SUBSCRIPTION_STATE_ACTIVE,
        /// <summary>
        /// 정기 결제가 일시중지되었습니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 이 상태에서는 모든 항목이 일시중지된 상태입니다.
        /// </summary>
        SUBSCRIPTION_STATE_PAUSED,
        /// <summary>
        /// 정기 결제가 유예 기간입니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 이 상태에서는 모든 항목이 유예 기간입니다.
        /// </summary>
        SUBSCRIPTION_STATE_IN_GRACE_PERIOD,
        /// <summary>
        /// 정기 결제가 보류 중 (일시중지됨)입니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 이 상태에서는 모든 항목이 보류 중입니다.
        /// </summary>
        SUBSCRIPTION_STATE_ON_HOLD,
        /// <summary>
        /// 정기 결제가 취소되었지만 아직 만료되지 않았습니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 모든 항목에서 autoRenewEnabled가 false로 설정되어 있습니다.
        /// </summary>
        SUBSCRIPTION_STATE_CANCELED,
        /// <summary>
        /// 정기 결제가 만료되었습니다. 모든 항목의 expiryTime이 지났습니다.
        /// </summary>
        SUBSCRIPTION_STATE_EXPIRED,
        /// <summary>
        /// 정기 결제의 대기 중인 거래가 취소되었습니다. 이 대기 중인 구매가 기존 정기 결제에 관한 것이라면 linkedPurchaseToken을 사용하여 해당 정기 결제의 현재 상태를 가져옵니다.
        /// </summary>
        SUBSCRIPTION_STATE_PENDING_PURCHASE_CANCELED
    }

}
