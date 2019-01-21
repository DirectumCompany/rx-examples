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
    /// Получить отметку об ЭП.
    /// </summary>
    /// <returns>Изображение отметки об ЭП в виде html.</returns>
    /// <remarks>
    /// В отметку об ЭП добавлены логотип, дата и время подписания, цвет отметки изменён на фиолетовый.
    /// </remarks>
    public override string GetSignatureMarkAsHtml(Sungero.Docflow.IOfficialDocument document, int versionId)
    {
      string html = Resources.HtmlStampTemplateCustom;
      
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForMark(document, versionId);
      if (signature == null)
        throw new Exception(Sungero.Docflow.OfficialDocuments.Resources.LastVersionNotApproved);
      
      #warning Заглушка, убрать в процессе реализации отметки для ПЭП.
      if (signature.SignCertificate == null)
        throw new Exception("Отсутствует сертификат");
      var certificateSubject = this.GetCertificateSubject(signature);      
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