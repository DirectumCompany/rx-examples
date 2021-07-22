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
    /// Пример перекрытия логики наложения отметки о простой ЭП для всех документов.
    /// В отметке о простой ЭП изменены логотип и пропорции заголовка.
    /// Также в отметку добавлены дата и время подписания.
    /// Цвет отметки изменён на фиолетовый.
    /// </description>
    public override string GetSignatureMarkForSimpleSignatureAsHtml(Sungero.Domain.Shared.ISignature signature)
    {
      if (signature == null)
        return string.Empty;
      
      var signatoryFullName = signature.SignatoryFullName;
      var signatoryId = signature.Signatory.Id;
      
      string html;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Resources.HtmlStampTemplateForSignatureCustom;
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
    /// Пример перекрытия логики наложения отметки о квалифицированной ЭП для всех документов.
    /// В отметке о квалифицированной ЭП изменены логотип и пропорции заголовка.
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
      
      string html;
      string validity;
      using (Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
      {
        html = Resources.HtmlStampTemplateForCertificateCustom;
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
    
    /// <summary>
    /// Перекрытие. Все jpg изображения > 1мб обрабатываются интерактивно.
    /// </summary>
    /// <param name="document">Документ для преобразования.</param>
    /// <returns>True - возможно преобразовать интерактивно, иначе - False.</returns>
    public override bool CanConvertToPdfInteractively(Sungero.Docflow.IOfficialDocument document)
    {
      var jpgFormatsList = new List<string>() { "jpg", "jpeg" };
      if (jpgFormatsList.Contains(document.LastVersion.BodyAssociatedApplication.Extension.ToLower()))
        return Locks.GetLockInfo(document).IsLockedByMe || !Locks.GetLockInfo(document).IsLocked;
      
      return base.CanConvertToPdfInteractively(document);
    }
    
    /// <summary>
    /// Перекрытие. Получить запрос создания временной таблицы с развернутыми политиками.
    /// </summary>
    /// <param name="now">Время старта фонового процесса.</param>
    /// <returns>Текст запроса.</returns>
    public override string GetStoragePolicySettingsQuery(DateTime now)
    {
      return string.Format(Docflow.Queries.Module.CreateStoragePolicySettings, Sungero.Docflow.Constants.Module.StoragePolicySettingsTableName, now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
    
    /// <summary>
    /// Перекрытие. Получить запрос получения документов для перемещения.
    /// </summary>
    /// <returns>Текст запроса.</returns>
    public override string GetDocumentsToTransferQuery()
    {
      return string.Format(Docflow.Queries.Module.SelectDocumentsToTransfer, Sungero.Docflow.Constants.Module.StoragePolicySettingsTableName);
    }
  }
}