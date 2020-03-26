using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Examples.DocumentTemplate;

namespace Sungero.Examples.Server
{
  partial class DocumentTemplateFunctions
  {
    
    /// <summary>
    /// Отфильтровать шаблоны по критериям.
    /// </summary>
    /// <param name="documentTemplates">Коллекция шаблонов.</param>
    /// <param name="document">Документ, который создается из шаблона.</param>
    /// <param name="documentKind">Вид документа.</param>
    /// <param name="businessUnit">Наша организация.</param>
    /// <param name="department">Подразделение.</param>
    /// <returns>Коллекция отфильтрованных шаблонов.</returns>
    [Public]
    public static IQueryable<IDocumentTemplate> FilterTemplatesByCriteria(IQueryable<IDocumentTemplate> documentTemplates,
                                                                          Docflow.IOfficialDocument document,
                                                                          Docflow.IDocumentKind documentKind,
                                                                          Company.IBusinessUnit businessUnit,
                                                                          Company.IDepartment department,
                                                                          Docflow.IDocumentGroupBase documentGroup,
                                                                          bool isCalledFromDocument)
    {
      // Вид документа.
      var allDocumentTypeGuid = Guid.Parse(Docflow.PublicConstants.DocumentTemplate.AllDocumentTypeGuid);
      if (documentKind != null)
      {
        var typeGuid = Guid.Parse(documentKind.DocumentType.DocumentTypeGuid);
        documentTemplates = documentTemplates.Where(d => !d.DocumentKinds.Any() &&
                                                    (d.DocumentType == allDocumentTypeGuid ||
                                                     d.DocumentType == typeGuid) ||
                                                    d.DocumentKinds.Any(k => Equals(k.DocumentKind, documentKind)));
      }
      else if (isCalledFromDocument)
      {
        var typeGuid = document.TypeDiscriminator;
        var availableKinds = Docflow.PublicFunctions.DocumentKind.GetAvailableDocumentKinds(document);
        documentTemplates = documentTemplates.Where(d => !d.DocumentKinds.Any() &&
                                                    (d.DocumentType == allDocumentTypeGuid ||
                                                     d.DocumentType == typeGuid) ||
                                                    availableKinds.Any(x => string.Equals(x.DocumentType.DocumentTypeGuid,
                                                                                          typeGuid.ToString(),
                                                                                          StringComparison.InvariantCultureIgnoreCase)));
      }
      
      // НОР.
      if (businessUnit != null)
        documentTemplates = documentTemplates.Where(d => !d.BusinessUnits.Any() || d.BusinessUnits.Any(t => Equals(t.BusinessUnit, businessUnit)));
      else if (isCalledFromDocument)
        documentTemplates = documentTemplates.Where(d => !d.BusinessUnits.Any());
      
      // Подразделение.
      if (department != null)
        documentTemplates = documentTemplates.Where(d => !d.Departments.Any() || d.Departments.Any(t => Equals(t.Department, department)));
      else if (isCalledFromDocument)
        documentTemplates = documentTemplates.Where(d => !d.Departments.Any());
      
      // Категория договора.
      if (documentGroup != null)
        documentTemplates = documentTemplates.Where(d => !d.DocumentGroups.Any() || d.DocumentGroups.Any(t => Equals(t.DocumentGroup, documentGroup)));
      else if (isCalledFromDocument)
        documentTemplates = documentTemplates.Where(d => !d.DocumentGroups.Any());
      
      return documentTemplates;
    }

  }
}