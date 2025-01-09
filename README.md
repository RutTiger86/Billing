# Billing Server 프로젝트

Billing Server는 게임 서버의 결제 및 구독 기능을 처리하기 위해 설계된 C# 및 .NET 8 기반의 서버 구현체입니다. Google IAP, iOS IAP, PG사 연동 등 다양한 결제 방식을 지원하며, 확장성과 모듈성을 고려하여 설계되었습니다.

## 주요 기능

1. Google IAP 연동
   - Google Play에서의 인앱 결제를 원활하게 처리.
   - 구매 토큰을 검증하고 Google Cloud Billing API를 통해 거래 보안 보장.
2. iOS IAP 연동 (예정)
   - Apple App Store 거래 처리를 위한 통합 예정.
3. PG사 연동 (예정)

   - 제3자 PG(결제 게이트웨이)를 지원하여 추가 결제 방법 제공.

4. 구독 관리

   - 검증 로직을 3단계로 분류:
     1. 구매 검증: 구독 구매를 검증.
     2. 복구 검증: 구독 복원을 처리.
     3. 변경 검증: 구독 플랜 업그레이드 및 다운그레이드를 검증.

5. 메모리 데이터셋

   - 인메모리 DataSet 구조를 사용하여 신속한 프로토타이핑과 테스트 지원.
   - 클래스 정의를 기반으로 동적으로 테이블 생성.

6. 유연한 상품 관리  
   상품(Product), 관련 아이템(ProductItem), 메타데이터 관리 지원.

## 기술 세부 사항

### 기술 스텍

- 언어 : C#
- 프레임워크 : .NET 8
- 클라우드 : Google Cloud Platform (GCP) - Google IAP 연동
- 아키텍처 : 모듈형 및 확장 가능

### 핵심 클래스

- `BillTx` : 모든 거래의 트렌잭션 을 나타냅니다.
- `BillDetail` : 각 트렌잭션의 거래 상세 내용을 나타냅니다.
- `Product` : 구매하는 Product 정보를 나타냅니다. 여러 item을 가질수 있습니다.
- `ProductItem` : Product와 여러 Item의 연결정보를 나타냅니다.
- `Item` : 실제 지급되는 Item정보를 나타냅니다.
- `Subscription` : 구독관련 정보를 나타냅니다.
- `Ledger` : 각 계정별 Point 정보를 나타냅니다. 한 계정당 PointType에 따라 여러 Row가 존재합니다.

### 인메모리 데이터셋 설계

- 클래스 정의에서 동적으로 `DataTable` 생성.
- 프로토타이핑 과정에서 데이터 조작 간소화.

  ```
  private void AddTable<T>(string idName) where T : class
  {
    var tableName = typeof(T).Name;
    DataTable table = new(tableName);

    foreach (var prop in typeof(T).GetProperties())
    {
        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
    }

    if (table.Columns.Contains(idName))
    {
        table.PrimaryKey = new[] { table.Columns[idName] };
    }

    memoryDataSet.Tables.Add(table);
  }
  ```

## 로드맵

1. Point 구매 지원
   - Ledger Point 내용을 기반한 구매 프로세스 작성.
2. RollBack 프로세스 작성
   - 충전/소모시 단계별 RollBack 프로세스 작성
3. Service 형태 작성
   - Protobuf를 통하여 API, gRPC 형태 개발
4. iOS IAP 연동
   - Apple App Store와의 안전한 거래 검증 통합.
5. PG사 연동 지원
   - 다양한 결제 게이트웨이를 추가하여 폭넓은 결제 옵션 제공.
6. 구독 기능 강화
   - 구독 변경 및 복구 워크플로우 고도화.

## 기타정보

[프로세스 설명](docs/PROCESS.md)  
[Todo](docs/TODO.md)
