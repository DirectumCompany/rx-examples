using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.IncomingInvoice;

namespace Sungero.Examples.Server
{
  partial class IncomingInvoiceFunctions
  {
    /// <summary>
    /// Получить отметку об ЭП.
    /// </summary>
    /// <returns>Изображение отметки об ЭП в виде html.</returns>
    /// <description>
    /// Пример перекрытия логики наложения отметки об ЭП для входящего счета.
    /// В отметку об ЭП добавлены логотип, дата и время подписания, цвет отметки изменён на красный, текст изменен на "ПРИНЯТ К ОПЛАТЕ".
    /// </description>
    public override string GetSignatureMarkAsHtml(int versionId)
    {
      string html = Examples.IncomingInvoices.Resources.HtmlStampTemplateCustom;
      
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForMark(_obj, versionId);
      if (signature == null)
        throw new Exception(Sungero.Docflow.OfficialDocuments.Resources.LastVersionNotApproved);
      
      #warning Заглушка, убрать в процессе реализации отметки для ПЭП.
      if (signature.SignCertificate == null)
        throw new Exception("Отсутствует сертификат");
      var certificateSubject = Docflow.PublicFunctions.Module.GetCertificateSubject(signature);
      var signatoryName = string.Format("{0} {1}", certificateSubject.Surname, certificateSubject.GivenName).Trim();
      if (string.IsNullOrEmpty(signatoryName))
        signatoryName = certificateSubject.CounterpartyName;
      
      html = html.Replace("{SignatoryFullName}", signatoryName);
      html = html.Replace("{Thumbprint}", signature.SignCertificate.Thumbprint.ToLower());
      html = html.Replace("{Validity}", string.Format("{0} {1} {2} {3}",
                                                      Company.Resources.From,
                                                      signature.SignCertificate.NotBefore.Value.ToShortDateString(),
                                                      Company.Resources.To,
                                                      signature.SignCertificate.NotAfter.Value.ToShortDateString())
                         );
      html = html.Replace("{SigningDate}", signature.SigningDate.ToString("g"));
      
      return html;
    }
  }
}