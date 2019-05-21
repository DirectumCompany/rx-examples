﻿using System;
using Sungero.Core;

namespace Sungero.Capture.Constants
{
  public static class Module
  {
    public const string SimpleRelationRelationName = "Simple relation";
        
    public const string ArioUrlKey = "ArioUrl";
    
    public const string MinFactProbabilityKey = "MinFactProbability";
    
    public const string CaptureMockModeKey = "CaptureMockMode";
    
    public const string LetterClassName = "Письмо";
    
    public const string ContractStatementClassName = "Акт выполненных работ";
    
    public static class Initialize
    {
      public static readonly Guid MockIncommingLetterKind = Guid.Parse("E37D0916-7814-441E-84EF-904B7B643497");
      
      public static readonly Guid MockContractStatementKind = Guid.Parse("C149F090-CE5C-4786-BACE-AA38BB51635A");
    }
  }
}