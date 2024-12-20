using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingInvoice;
using IncomingInvoiceConstants = Sungero.Examples.PublicConstants.Contracts.IncomingInvoice;

namespace Sungero.Examples.Server
{
  partial class IncomingInvoiceFunctions
  {
    /// <summary>
    /// Преобразовать документ в PDF с простановкой отметок.
    /// </summary>
    /// <param name="versionId">ИД версии, на которую будут проставлены отметки.</param>
    /// <returns>Результат преобразования.</returns>
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult ConvertToPdfWithMarks(long versionId)
    {
      /// Пример перекрытия, в котором при выполнении действия
      /// "Создать PDF-документ с отметками" для входящих счетов с состоянием "Оплачен"
      /// добавляется отметка "Оплачено" на преобразованный PDF-документ.
      this.UpdateInvoicePaymentMark();
      return base.ConvertToPdfWithMarks(versionId);
    }
    
    /// <summary>
    /// Сохранить отметку для простановки на PDF документе с состоянием "Оплачено".
    /// Удалить отметку в случае, если состояние документа было изменено
    /// с "Оплачено" на другое.
    /// </summary>
    [Public]
    public virtual void UpdateInvoicePaymentMark()
    {
      if (_obj.LifeCycleState == Sungero.Contracts.IncomingInvoice.LifeCycleState.Paid)
      {
        var mark = GetOrCreateMark(IncomingInvoiceConstants.PaymentMarkKindSid);
        mark.XIndent = 12;
        mark.YIndent = 20;
        mark.Page = -1;
        mark.Save();
      }
      else
      {
        var paymentMark = GetVersionMarks(_obj.LastVersion.Id, IncomingInvoiceConstants.PaymentMarkKindSid).SingleOrDefault();
        Docflow.PublicFunctions.Module.DeleteMark(_obj, paymentMark);
      }
    }
    
    /// <summary>
    /// Получить отметку для счета с состоянием "Оплачено".
    /// </summary>
    /// <returns>Изображение отметки в виде html.</returns>
    public virtual string GetPaymentMarkAsHtml(long versionId)
    {
      return Examples.IncomingInvoices.Resources.HtmlMarkTemplatePayment;
    }
    
    /// <summary>
    /// Получить отметку об ЭП.
    /// </summary>
    /// <param name="versionId">Id версии, для генерации.</param>
    /// <returns>Изображение отметки об ЭП в виде html.</returns>
    public override string GetSignatureMarkAsHtml(long versionId)
    {
      var signature = this.GetSignatureForMark(versionId);
      if (signature == null)
        throw new Exception(Sungero.Docflow.OfficialDocuments.Resources.LastVersionNotApproved);
      
      // В случае квалифицированной ЭП информацию для отметки брать из атрибутов субъекта сертификата.
      if (signature.SignCertificate != null)
        return this.GetSignatureMarkForCertificateAsHtml(signature);
      
      // В случае простой ЭП информацию для отметки брать из атрибутов подписи.
      return this.GetSignatureMarkForSignatureAsHtml(signature);
    }
    
    /// <summary>
    /// Получить отметку об ЭП для подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// <returns>Изображение отметки об ЭП для подписи в виде html.</returns>
    /// <description>
    /// Пример перекрытия логики наложения отметки о простой ЭП для входящего счета.
    /// В отметке о простой ЭП изменены логотип и пропорции заголовка.
    /// Также в отметку добавлены дата и время подписания.
    /// Цвет отметки изменен на красный.
    /// Текст отметки изменен на "ПРИНЯТ К ОПЛАТЕ".
    /// </description>
    public virtual string GetSignatureMarkForSignatureAsHtml(Sungero.Domain.Shared.ISignature signature)
    {
      if (signature == null)
        return string.Empty;
      
      var signatoryFullName = signature.SignatoryFullName;
      var signatoryId = signature.Signatory.Id;
      
      string html;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Examples.IncomingInvoices.Resources.HtmlStampTemplateForSignatureCustom;
        html = html.Replace("{SignatoryFullName}", signatoryFullName);
        html = html.Replace("{SignatoryId}", signatoryId.ToString());
        html = html.Replace("{SigningDate}", signature.SigningDate.ToString("g"));
      }
      return html;
    }
    
    /// <summary>
    /// Получить отметку об ЭП для сертификата из подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// <returns>Изображение отметки об ЭП для сертификата в виде html.</returns>
    /// <description>
    /// Пример перекрытия логики наложения отметки о квалифицированной ЭП для входящего счета.
    /// В отметке о квалифицированной ЭП изменены логотип и пропорции заголовка.
    /// Также в отметку добавлены дата и время подписания.
    /// Цвет отметки изменен на красный.
    /// Текст отметки изменен на "ПРИНЯТ К ОПЛАТЕ".
    /// </description>
    public virtual string GetSignatureMarkForCertificateAsHtml(Sungero.Domain.Shared.ISignature signature)
    {
      if (signature == null)
        return string.Empty;
      
      var certificate = signature.SignCertificate;
      if (certificate == null)
        return string.Empty;
      
      var certificateSubject = Docflow.PublicFunctions.Module.GetCertificateSubject(signature);
      
      var signatoryFullName = string.Format("{0} {1}", certificateSubject.Surname, certificateSubject.GivenName).Trim();
      if (string.IsNullOrEmpty(signatoryFullName))
        signatoryFullName = certificateSubject.CounterpartyName;
      
      string html;
      string validity;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Examples.IncomingInvoices.Resources.HtmlStampTemplateForCertificateCustom;
        html = html.Replace("{SignatoryFullName}", signatoryFullName);
        html = html.Replace("{Thumbprint}", certificate.Thumbprint.ToLower());
        validity = string.Format("{0} {1} {2} {3}",
                                 Company.Resources.From,
                                 certificate.NotBefore.Value.ToShortDateString(),
                                 Company.Resources.To,
                                 certificate.NotAfter.Value.ToShortDateString());
        html = html.Replace("{Validity}", validity);
        html = html.Replace("{SigningDate}", signature.SigningDate.ToString("g"));
      }
      return html;
    }
  }
}