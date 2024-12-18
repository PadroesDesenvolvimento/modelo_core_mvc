using System;
using System.Diagnostics.CodeAnalysis;

namespace modelo_core_mvc.Errors;

[ExcludeFromCodeCoverage]
public class ErrorViewModel
{
    public string RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public Exception Exception { get; set; } 
}
