using System;
using Sungero.Core;

namespace Sungero.Examples.Module.Exchange.Constants
{
  public static class Module
  {
    public static class NonformalizedKind
    {
      public const string IncomingInvoice = "ProformaInvoice";
      
      public const string Title = "Title";
    }
    
    public static class MetadataKeyIncomingInvoice
    {
      public const string DocumentDate = "DocumentDate";
      
      public const string DocumentNumber = "DocumentNumber";
      
      public const string TotalSum = "TotalSum";
      
      public const string TotalVat = "TotalVat";

      public const string Grounds = "Grounds";
    }
  }
}