using System;
using Sungero.Core;

namespace Sungero.Examples.Constants.Docflow
{
  public static class Memo
  {
        // Sid отметки "Утверждено".
    [Public]
    public const string SignMarkKindSid = "eeb0239e-0c82-42fc-b3f7-ce937a8b6c4a";

    // Полное имя класса, из которого вызывается метод получения отметки "Утверждено".
    [Public]
    public const string SignMarkKindClass = "Sungero.Examples.Server.MemoFunctions";

    // Имя метода получения отметки "Утверждено".
    [Public]
    public const string SignMarkKindMethod = "GetMemoSignMarkAsHtml";
  }
}