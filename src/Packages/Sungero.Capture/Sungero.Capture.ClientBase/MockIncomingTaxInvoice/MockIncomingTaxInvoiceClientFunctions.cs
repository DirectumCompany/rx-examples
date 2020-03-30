using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockIncomingTaxInvoice;

namespace Sungero.Capture.Client
{
  partial class MockIncomingTaxInvoiceFunctions
  {
    /// <summary>
    /// Изменить отображение вкладки "Номенклатура" в зависимости от статуса верификации.
    /// </summary>
    public void ChangeGoodsVerificationView()
    {
      if (!_obj.VerificationState.HasValue || _obj.VerificationState.Value == VerificationState.Completed)
        _obj.State.Controls.GoodsPreview.IsVisible = false;
    }
    
    /// <summary>
    /// Дополнительная подсветка.
    /// </summary>
    /// <param name="documentRecognitionInfo">Результат распознавания документа.</param>
    /// <param name="highlightActivationStyle">Параметры отображения фокусировки подсветки.</param>
    public override void SetAdditionalHighlight(Commons.IEntityRecognitionInfo documentRecognitionInfo,
                                             Docflow.Structures.Module.IHighlightActivationStyle highlightActivationStyle)
    {
      base.SetAdditionalHighlight(documentRecognitionInfo, highlightActivationStyle);
      
      // Подсветка номенклатуры.
      var incomingTaxInvoice = MockIncomingTaxInvoices.As(_obj);
      HighlightCollection(incomingTaxInvoice.State.Controls.GoodsPreview,
                          documentRecognitionInfo, incomingTaxInvoice.Goods,
                          highlightActivationStyle);
    }
  }
}