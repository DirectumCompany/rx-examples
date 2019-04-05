
/// ==================================================================
/// ModuleFunctions.g.cs
/// ==================================================================

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sungero.Capture.Functions
{
  internal static partial class Module
  {
    internal static class Remote
    {
      /// <redirect project="Sungero.Capture.Server" type="Sungero.Capture.Server.ModuleFunctions" />
      internal static global::Sungero.Docflow.ISimpleDocument GetSimpleDocument(global::System.Int32 documentId)
      {
        return global::Sungero.Domain.Shared.RemoteFunctionExecutor.ExecuteWithResult<global::Sungero.Docflow.ISimpleDocument>(
          global::System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb"),
          "GetSimpleDocument(global::System.Int32)", documentId);
      }
      /// <redirect project="Sungero.Capture.Server" type="Sungero.Capture.Server.ModuleFunctions" />
      internal static global::Sungero.Docflow.ISimpleDocument CreateSimpleDocument()
      {
        return global::Sungero.Domain.Shared.RemoteFunctionExecutor.ExecuteWithResult<global::Sungero.Docflow.ISimpleDocument>(
          global::System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb"),
          "CreateSimpleDocument()");
      }
      /// <redirect project="Sungero.Capture.Server" type="Sungero.Capture.Server.ModuleFunctions" />
      internal static void GrantRightsToDocument(global::System.Int32 documentId, global::System.Int32 responsibleId, global::System.Guid rightType)
      {
      global::Sungero.Domain.Shared.RemoteFunctionExecutor.Execute(
          global::System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb"),
          "GrantRightsToDocument(global::System.Int32,global::System.Int32,global::System.Guid)", documentId, responsibleId, rightType);
      }
      /// <redirect project="Sungero.Capture.Server" type="Sungero.Capture.Server.ModuleFunctions" />
      internal static global::Sungero.Workflow.ISimpleTask CreateSimpleTask(global::System.String taskName, global::System.Int32 documentId, global::System.Int32 responsibleId)
      {
        return global::Sungero.Domain.Shared.RemoteFunctionExecutor.ExecuteWithResult<global::Sungero.Workflow.ISimpleTask>(
          global::System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb"),
          "CreateSimpleTask(global::System.String,global::System.Int32,global::System.Int32)", taskName, documentId, responsibleId);
      }

    }
    private static object GetFunctionsContainer()
    {
      return new global::Sungero.Capture.Shared.ModuleFunctions();
    }

    private static object GetFinalFunctionsContainer(global::Sungero.Metadata.ModuleProjectType projectType)
    {
      var moduleId = new global::System.Guid("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb");
      var finalModuleMetadatda = global::Sungero.Metadata.MetadataExtension.GetFinal(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(moduleId) ?? global::Sungero.Metadata.Services.MetadataSearcher.FindLayerModuleMetadata(moduleId));
      var assemblyName = finalModuleMetadatda.GetAssemblyName(projectType);
      var moduleFunctionsType = global::System.Type.GetType(global::System.String.Format("{0}.{1}, {2}", finalModuleMetadatda.FunctionNamespace, "Module", assemblyName));
      return moduleFunctionsType.GetMethod("GetFunctionsContainer", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static).Invoke(null, null);
    }
  }
}

/// ==================================================================
/// ModuleHyperlinks.g.cs
/// ==================================================================

namespace Sungero.Capture
{
  public static class CaptureClientFunctionHyperlinksExtensions
  {
  }
}

/// ==================================================================
/// ModuleResources.g.cs
/// ==================================================================

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sungero.Capture.Shared
{
  public class ModuleResources : global::Sungero.Capture.IModuleResources
  {
    public virtual global::CommonLibrary.LocalizedString InvalidResponsibleId
    {
      get
      {
        return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")) , "InvalidResponsibleId", false);
      }
    }

    public virtual global::CommonLibrary.LocalizedString InvalidResponsibleIdFormat(params object[] args)
    {
      return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")), "InvalidResponsibleId", false, args);
    }
    public virtual global::CommonLibrary.LocalizedString FileNotFound
    {
      get
      {
        return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")) , "FileNotFound", false);
      }
    }

    public virtual global::CommonLibrary.LocalizedString FileNotFoundFormat(params object[] args)
    {
      return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")), "FileNotFound", false, args);
    }
    public virtual global::CommonLibrary.LocalizedString NoFilesInfoInPackage
    {
      get
      {
        return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")) , "NoFilesInfoInPackage", false);
      }
    }

    public virtual global::CommonLibrary.LocalizedString NoFilesInfoInPackageFormat(params object[] args)
    {
      return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")), "NoFilesInfoInPackage", false, args);
    }
    public virtual global::CommonLibrary.LocalizedString DocumentName
    {
      get
      {
        return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")) , "DocumentName", false);
      }
    }

    public virtual global::CommonLibrary.LocalizedString DocumentNameFormat(params object[] args)
    {
      return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")), "DocumentName", false, args);
    }
    public virtual global::CommonLibrary.LocalizedString TaskName
    {
      get
      {
        return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")) , "TaskName", false);
      }
    }

    public virtual global::CommonLibrary.LocalizedString TaskNameFormat(params object[] args)
    {
      return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")), "TaskName", false, args);
    }
    public virtual global::CommonLibrary.LocalizedString DocumentNotFound
    {
      get
      {
        return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")) , "DocumentNotFound", false);
      }
    }

    public virtual global::CommonLibrary.LocalizedString DocumentNotFoundFormat(params object[] args)
    {
      return global::Sungero.Domain.Shared.ResourceService.Instance.GetString(global::Sungero.Metadata.Services.MetadataSearcher.FindModuleMetadata(System.Guid.Parse("4c7444e6-c4af-46c1-9cfa-d9194d3ff0cb")), "DocumentNotFound", false, args);
    }

  }
}

/// ==================================================================
/// ModuleFoldersFilterStates.g.cs
/// ==================================================================

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sungero.Capture.Shared
{
}

/// ==================================================================
/// ModuleSharedPublicFunctions.g.cs
/// ==================================================================

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sungero.Capture.Shared
{
  public partial class ModuleSharedPublicFunctions : global::Sungero.Capture.Shared.IModuleSharedPublicFunctions
  {
    public global::Sungero.Docflow.ISimpleDocument Remote_CreateSimpleDocument()
    {
      return global::Sungero.Capture.Functions.Module.Remote.CreateSimpleDocument();
    }
    public global::Sungero.Workflow.ISimpleTask Remote_CreateSimpleTask(global::System.String taskName, global::System.Int32 documentId, global::System.Int32 responsibleId)
    {
      return global::Sungero.Capture.Functions.Module.Remote.CreateSimpleTask(taskName, documentId, responsibleId);
    }
    public global::Sungero.Docflow.ISimpleDocument Remote_GetSimpleDocument(global::System.Int32 documentId)
    {
      return global::Sungero.Capture.Functions.Module.Remote.GetSimpleDocument(documentId);
    }
    public void Remote_GrantRightsToDocument(global::System.Int32 documentId, global::System.Int32 responsibleId, global::System.Guid rightType)
    {
global::Sungero.Capture.Functions.Module.Remote.GrantRightsToDocument(documentId, responsibleId, rightType);
    }
  }
}

/// ==================================================================
/// ModuleWidgetParameters.g.cs
/// ==================================================================

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sungero.Capture.Shared
{  
}
