using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockContractStatement;

namespace Sungero.Capture.Client
{
  partial class MockContractStatementFunctions
  {
    /// <summary>
    /// Изменить отображение вкладки "Номенклатура" в зависимости от статуса верификации.
    /// </summary>
    public void ChangeGoodsVerificationView()
    {
      if (!_obj.VerificationState.HasValue || _obj.VerificationState.Value == VerificationState.Completed)
        _obj.State.Controls.GoodsPreview.IsVisible = false;
    }
    
    public override void HighlightGoodsInMockMode(Commons.IEntityRecognitionInfo documentRecognitionInfo,
                                             Docflow.Structures.Module.IHighlightActivationStyle highlightActivationStyle)
    {
      base.HighlightGoodsInMockMode(documentRecognitionInfo, highlightActivationStyle);
      
      var contractStatement = MockContractStatements.As(_obj);
      HighlightCollection(contractStatement.State.Controls.GoodsPreview,
                          documentRecognitionInfo, contractStatement.Goods,
                          highlightActivationStyle);
    }    
  }
}