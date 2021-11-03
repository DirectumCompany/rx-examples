using System;
using Sungero.Core;

namespace Sungero.Examples.Module.Exchange.Constants
{
  public static class Module
  {
    // Виды документов в Диадоке.
    public static class NonformalizedKindDiadoc
    {
      // Входящий счет.
      public const string IncomingInvoice = "ProformaInvoice";
      
      // Письмо.
      public const string Title = "Title";
    }
    
    // Виды документов в СБИС.
    public static class NonformalizedKindSbis
    {      
      // Входящий счет.
      public const string IncomingInvoice ="ЭДОСч";
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