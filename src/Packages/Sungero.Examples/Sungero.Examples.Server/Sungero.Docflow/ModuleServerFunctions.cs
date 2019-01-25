using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Examples.Module.Docflow.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Получить отметку об ЭП для подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// <returns>Изображение отметки об ЭП для подписи в виде html.</returns>
    /// <description>
    /// Пример перекрытия логики наложения отметки о ПЭП для всех документов.
    /// В отметке о ПЭП изменёны логотип и пропорции заголовка. 
    /// Также в отметку добавлены дата и время подписания.
    /// Цвет отметки изменён на фиолетовый.
    /// </description>
    public override string GetSignatureMarkForSignatureAsHtml(Sungero.Domain.Shared.ISignature signature)
    {
      if (signature == null)
        return string.Empty;
      
      var signatoryFullName = signature.SignatoryFullName;
      var signatoryId = signature.Signatory.Id;
      
      string html = Resources.HtmlStampTemplateForSignatureCustom;
      html = html.Replace("{SignatoryFullName}", signatoryFullName);
      html = html.Replace("{SignatoryId}", signatoryId.ToString());
      html = html.Replace("{SigningDate}", signature.SigningDate.ToString("g"));
      return html;
    }
    
    /// <summary>
    /// Получить отметку об ЭП для сертификата из подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// <returns>Изображение отметки об ЭП для сертификата в виде html.</returns>
    /// <description>
    /// Пример перекрытия логики наложения отметки о КЭП для всех документов.
    /// В отметке о КЭП изменёны логотип и пропорции заголовка. 
    /// Также в отметку добавлены дата и время подписания.
    /// Цвет отметки изменён на фиолетовый.
    /// </description>
    public override string GetSignatureMarkForCertificateAsHtml(Sungero.Domain.Shared.ISignature signature)
    {
      if (signature == null)
        return string.Empty;
      
      var certificate = signature.SignCertificate;
      if (certificate == null)
        return string.Empty;
      
      var certificateSubject = this.GetCertificateSubject(signature);
      
      var signatoryFullName = string.Format("{0} {1}", certificateSubject.Surname, certificateSubject.GivenName).Trim();
      if (string.IsNullOrEmpty(signatoryFullName))
        signatoryFullName = certificateSubject.CounterpartyName;
      
      string html = Resources.HtmlStampTemplateForCertificateCustom;
      html = html.Replace("{SignatoryFullName}", signatoryFullName);
      html = html.Replace("{Thumbprint}", certificate.Thumbprint.ToLower());
      var validity = string.Format("{0} {1} {2} {3}",
                                   Company.Resources.From,
                                   certificate.NotBefore.Value.ToShortDateString(),
                                   Company.Resources.To,
                                   certificate.NotAfter.Value.ToShortDateString());
      html = html.Replace("{Validity}", validity);
      html = html.Replace("{SigningDate}", signature.SigningDate.ToString("g"));
      return html;
    }
  }
}