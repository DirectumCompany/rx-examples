using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.Capture.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      InitializationLogger.Debug("Init: Grant rights on databooks to all users.");
      GrantRightsAllUsersOnDatabooks();
      
      InitializationLogger.Debug("Init: Create smart processing settings.");
      CreateSmartProcessingSettings();
    }
    
    /// <summary>
    /// Выдать права всем пользователям на справочники.
    /// </summary>
    public static void GrantRightsAllUsersOnDatabooks()
    {
      var allUsers = Roles.AllUsers;
      if (allUsers != null)
      {
        Capture.SmartProcessingSettings.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
        Capture.SmartProcessingSettings.AccessRights.Save();
      }
    }
    
    /// <summary>
    /// Создать настройки интеллектуальной обработки документов.
    /// </summary>
    public static void CreateSmartProcessingSettings()
    {
      var smartProcessingSettings = PublicFunctions.SmartProcessingSetting.GetSmartProcessingSettings();
      if (smartProcessingSettings == null)
        PublicFunctions.SmartProcessingSetting.Remote.CreateSmartProcessingSettings();
    }
  }

}
