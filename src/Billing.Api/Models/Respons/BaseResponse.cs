namespace Billing.Api.Models.Respons
{
    public class BaseResponse<T>
    {
        /// <summary>
        /// 결과값
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 에러코드
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 에러 메세지
        /// </summary>
        public string? ErrorMessage { get; set; }


        /// <summary>
        /// 전송 데이터
        /// </summary>
        public T? Data { get; set; }
    }
}
