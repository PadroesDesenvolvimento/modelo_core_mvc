using System;

namespace modelo_core_mvc.Errors
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public Exception Exception { get; set; } // Adicione esta propriedade
    }
}
