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
    /// Получить отметку о подписании документа.
    /// </summary>
    /// <returns>Строка в формате html.</returns>
    public override string GetSignatureMark(Sungero.Docflow.IOfficialDocument document, int versionId)
    {
      string html = Resources.HtmlStampTemplateCustom;
      
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForStamp(document, versionId);
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
      html = html.Replace("{SignDate}", signature.SigningDate.ToString("d"));
      html = html.Replace("{SignTime}", signature.SigningDate.ToString("t"));
      
      return html;
    }
  }
}