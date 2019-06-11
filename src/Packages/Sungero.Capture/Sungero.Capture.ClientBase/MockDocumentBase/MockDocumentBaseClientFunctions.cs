using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockDocumentBase;

namespace Sungero.Capture.Client
{
  partial class MockDocumentBaseFunctions
  {
    /// <summary>
    /// Подсветить указанные свойства в карточке документа.
    /// </summary>
    /// <param name="propertyNames">Список имён свойств.</param>
    /// <param name="color">Цвет.</param>
    public virtual void HighlightProperties(List<string> propertyNames, Sungero.Core.Color color)
    {
      foreach (var property in propertyNames)
      {
        this._obj.State.Properties[property].HighlightColor = color;
      }
    }
  }
}