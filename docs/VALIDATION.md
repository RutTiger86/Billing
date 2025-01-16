# Billing Purchase Validation Process

해당 문서는 Billing Server에 대한 결제 검증 방식 서술합니다.
특히 동시에 들어오는 이벤트를 염두하여 검증합니다.
검증의 기본 형식은 작업 진행 후 검증 및 Rollback 프로세스로 진행합니다. (ex : 구매-> 검증-> 롤백)

## 작업 진행 후 검증 프로세스를 진행하는 이유

검증 후 작업진행의 경우 단시간내 중복 전달되는 이벤트에대해 중복 이벤트들이 검증이 통과 될수 있습니다. 중복 전달되는 이벤트에대해 진행후 사후 검증시 전달되는 시간에 따라 모두 실패 혹은 1건 성공만 진행됩니다.

## Transaction Validation

Billing의 시작은 하나의 Transaction 으로 지정 실행합니다.  
구매시 Transaction 의 검증의 경우 다음과 같은 단계로 진행됩니다.

1. Transaction ID 존재 유무
2. Transaction 완료 상태 확인(IsDone)
3. 해당 Transaction ID + 초기상태의 Row에 대한 상태변경(ValidationStart)
4. 3번의 결과 행 확인 (1 이하시 시작 실패 - 이미 검증 진행중)

## Point Validation

Point로 구매 진행시 Point를 선 차감 후 검증합니다.

1. AccountID + PointType을 기반으로 Ledger 확인
2. Ledger 의 Balance에 대하여 Amount 만큼의 차감 진행
3. 차감 진행 후 Balance에 재조회 후 금액 확인(음수 여부 - 음수의 경우 롤백 처리 진행)

## Point Rollback Validation

Point를 충전/소모 한 경우의 RollBack 진행시 소모 롤백은 다음과 같습니다.

1. AccountID + PointType을 기반으로 Ledger 확인
2. PointHistoryID를 기반으로 이력 존재 및 isRollback 상태 확인
3. PointHistoryID를 기반으로 IsRollback 상태 true로 변경
4. 3번 결과행 확인 (1이하시 롤백 실패 - 이미 롤백 진행중)
5. Ledger 의 Balance에 대하여 Amount 만큼 충전진행

충전 롤백은 롤백 후 재검증을 진행하며 다음과 같습니다.

1. AccountID + PointType을 기반으로 Ledger 확인
2. PointHistoryID를 기반으로 이력 존재 및 isRollback 상태 확인
3. PointHistoryID를 기반으로 IsRollback 상태 true로 변경
4. 3번 결과행 확인 (1이하시 롤백 실패 - 이미 롤백 진행중)
5. Ledger 의 Balance에 대하여 Amount 만큼 차감 진행
6. Ledger 의 Balace 재조회 후 금액 확인 (음수여부)
7. 6번의 결과가 음수 일 경우
   - PointHistoryID를 기반으로 IsRollback 상태 false로 변경
   - Ledger 의 Balance에 대하여 Amount 만큼 충전 진행
   - Rollback 결과 -1로 전달(실패)
8. 6번의 결과가 양수일 경우 Rollback 결과 1로 전달(성공)
