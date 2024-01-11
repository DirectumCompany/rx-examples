using System.Collections.Generic;

namespace Sungero.Integration1CExtensions
{
  /// <summary>
  /// Данные об организации в 1С.
  /// </summary>
  public class BusinessUnit1C
  {
    /// <summary>
    /// ИНН.
    /// </summary>
    public string ИНН { get; set; }

    /// <summary>
    /// КПП.
    /// </summary>
    public string КПП { get; set; }

    /// <summary>
    /// Идентификатор.
    /// </summary>
    public string Ref_Key { get; set; }
  }

  /// <summary>
  /// Данные о списке организаций в 1С.
  /// </summary>
  public class BusinessUnitList1C
  {
    /// <summary>
    /// Список организаций.
    /// </summary>
    public List<BusinessUnit1C> Value { get; set; }
  }
}
