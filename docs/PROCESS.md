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
Status는 세분화 하여 구분되어 있습니다. 실무에 따라 상태값은 추가/삭제 합니다.

- INITIATED : 거래 생성
- COMPLETED : 거래 완료
- VALIDATION_PENDING : 검증 시작
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
- IAP_RECEIPT_PENDING : IAP 영수증 검증 대기 중
- IAP_RECEIPT_VALIDATION : IAP 영수증 검증 중
- IAP_RECEIPT_VALID : IAP 영수증 검증 성공
- IAP_RECEIPT_INVALID : IAP 영수증 검증 실패
- REFUNDED : 환불됨
- CANCELED : 취소됨
- EXPIRED : 만료됨
- CHARGEBACK : 기타 사유로 취소됨 (운영)

## Process

### IAP Purchase Process

IAP(iOS, Google Etc...) 상품 구매 프로세스

![01.Bill IAP Purchase Process](img/01.Bill%20IAP%20Purchase%20Process.png)

### Web PG Purchase Process

Web 기반 PG 연결 상품 구매 프로세스

![02.Bill Web PG Purchase Process](img/02.Bill%20Web%20PG%20Purchase%20Process.png)

### Web Charge Point Process

Web 기반 내부 포인트 충전 프로세스

![03.Bill Web Charge Point Process](img/03.Bill%20Web%20Charge%20Point%20Process.png)

### App Point Purchase Process

App 기반 내부 포인트 사용 구매 프로세스

![04.Bill App Point Purchase Process](img/04.Bill%20App%20Point%20Purchase%20Process.png)

### Web Point Purchase Process

Web 기반 내부 포인트 사용 구매 프로세스

![05.Bill Web Point Purchase Process](img/05.Bill%20Web%20Point%20Purchase%20Process.png)
