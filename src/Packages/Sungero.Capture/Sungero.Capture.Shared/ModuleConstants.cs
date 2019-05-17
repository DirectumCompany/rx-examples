using System;
using Sungero.Core;

namespace Sungero.Capture.Constants
{
  public static class Module
  {
    public const string SimpleRelationRelationName = "Simple relation";
        
    public const string ArioUrlKey = "ARIO_UR";
    
    public const string ArioURL = "http://smart:61100";
    
    public const string CaptureMockModeKey = "CaptureMockMode";
    
    public const string LetterClassName = "Письмо";
    
    public static class Initialize
    {
      public static readonly Guid MockIncommingLetterKind = Guid.Parse("E37D0916-7814-441E-84EF-904B7B643497");
    }
  }
}