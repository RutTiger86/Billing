# Billing Process

해당 문서는 Billing Server에 대한 Process 구상과 Type을 서술합니다.

## Transaction

Billing의 시작은 하나의 Transaction 으로 지정 실행합니다.  
Transaction 은 자동증가값의 _Bigint_ 로 관리되며 필요시 *GUID*를 사용합니다.  
*Bigint*를 초과하는 ID를 가지는 거래량에대해서는 해당 프로젝트에서는 고려되지 않습니다.

### TransactionType

각 Transaction 은 상위 Type과 하위 Type을 가집니다.  
명확한 관계를 표현하고자 상위 Type은 _TransactionType_ 하위 Type은 _TransactionSubType_ 으로 지정합니다.

1. TranscationType
   - WEB : Web 결재
   - IAP_Google : Google IAP
   - IAP_IOS : iOS IAP
   - POINT : 내부 포인트(내부 캐시) 관련
2. TransactinSubType
   - CONSUMABLE : 소모성 구매
   - NON_CONSUMABLE : 비 소모성 구매
   - SUBSCRIPTION_AUTO : 자동 갱신 구독
   - SUBSCRIPTION_NON_AUTO : 비 자동 갱신 구독
   - CHARGE : 유료 포인트 충전(유료/무료 포함)
   - REUND : 환불

### TransactionStatus

Transaction 의 상태는 isDone, isCancle로도 표현되지만 자세한 상태값은 Enum으로 표시합니다.

- INITIATED : 거래 생성
- COMPLETED : 거래 완료
- VALIDATION_PENDING : 검증 시작(영수증 검증 포함)
- VALIDATION : 검증 완료
- VALIDATION_FAILED : 검증 실패
- DELIVAERY_PENDING : 상품 전달 요청
- DELIVAERED : 상품 전달 완료
- DELIVERY_FAILED : 상품 전달 실패
- POINT_CHARGE_REQUESTED : 포인트 충전 요청
- POINT_CHARGE_END : 포인트 충전 완료
- POINT_CHARGE_FAILED : 포인트 충전 실패
- POINT_SPEND_REQUESTED : 포인트 소모 요청 (포인트 구매)
- POINT_SPEND : 포인트 소모 완료
- POINT_SPEND_FAILED : 포인트 소모 실패
- REFUNDED : 환불됨
- CANCELED : 취소됨
- EXPIRED : 만료됨
- CHARGEBACK : 기타 사유로 취소됨 (운영)

## Process
