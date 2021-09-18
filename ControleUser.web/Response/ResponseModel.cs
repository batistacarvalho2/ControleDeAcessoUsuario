namespace ControleUser.web.Response
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public object Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}