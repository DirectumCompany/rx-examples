using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Capture.MockIncomingLetter;

namespace Sungero.Capture
{
  partial class MockIncomingLetterClientHandlers
  {

    public virtual void AddresseesValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Addressees.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void RecipientTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.RecipientTrrc.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void RecipientTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.RecipientTin.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void RecipientValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Recipient.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void ContactValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Contact.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void SignatoryValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Signatory.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void InResponseToValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.InResponseTo.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void InNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.InNumber.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void DatedValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Dated.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void CorrespondentTrrcValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.CorrespondentTrrc.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void CorrespondentTinValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.CorrespondentTin.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

    public virtual void CorrespondentValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      this._obj.State.Properties.Correspondent.HighlightColor = Sungero.Core.Colors.Highlights.Empty;
    }

  }
}