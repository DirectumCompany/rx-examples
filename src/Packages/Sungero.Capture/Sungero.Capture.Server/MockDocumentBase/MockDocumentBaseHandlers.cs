using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockDocumentBase;

namespace Sungero.Capture
{
  partial class MockDocumentBaseConvertingFromServerHandler
  {

    public override void ConvertingFrom(Sungero.Domain.ConvertingFromEventArgs e)
    {
    }
  }

  partial class MockDocumentBaseServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
    }

    public override void BeforeSaveHistory(Sungero.Content.DocumentHistoryEventArgs e)
    {
    }

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
    }

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
    }
  }

}