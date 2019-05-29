using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockWaybill;

namespace Sungero.Capture
{
  partial class MockWaybillGoodsSharedCollectionHandlers
  {
    public virtual void GoodsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.Goods.Max(a => a.Number) ?? 0) + 1;
    }
  }

  partial class MockWaybillSharedHandlers
  {

  }
}