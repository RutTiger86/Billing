namespace Billing.Core.Enums
{
    public enum PointType
    {
        /// <summary>
        /// 미지정
        /// </summary>
        None,
        /// <summary>
        /// 유료
        /// </summary>
        Paid,
        /// <summary>
        /// 무료
        /// </summary>
        Free,
        /// <summary>
        /// 마일리지
        /// </summary>
        Mileage,
    }

    public enum PointOperationType
    {
        /// <summary>
        /// 생성
        /// </summary>
        CREATE,
        /// <summary>
        /// 충전
        /// </summary>
        CHARGE,
        /// <summary>
        /// 소모
        /// </summary>
        WITHDRAW
    }
}
