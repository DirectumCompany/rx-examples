﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockWaybill;

namespace Sungero.Capture.Client
{
  partial class MockWaybillFunctions
  {
    /// <summary>
    /// Изменить отображение вкладки "Номенклатура" в зависимости от статуса верификации.
    /// </summary>
    public void ChangeGoodsVerificationView()
    {
      if (!_obj.VerificationState.HasValue || _obj.VerificationState.Value == VerificationState.Completed)
        _obj.State.Controls.GoodsPreview.IsVisible = false;
    }
  }
}