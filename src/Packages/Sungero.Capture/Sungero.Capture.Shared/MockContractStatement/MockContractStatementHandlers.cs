using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockContractStatement;

namespace Sungero.Capture
{
  partial class MockContractStatementVersionsSharedCollectionHandlers
  {

    public override void VersionsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      base.VersionsAdded(e);
      // Убрать автозаполнение Содержания, т.к. заполняется наименованием вида при создании версии.
      _obj.Subject = string.Empty;
    }
  }

}