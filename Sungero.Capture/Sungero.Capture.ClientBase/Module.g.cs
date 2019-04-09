
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
    /// <redirect project="Sungero.Capture.ClientBase" type="Sungero.Capture.Client.ModuleFunctions" />
    internal static void ImportDocument(global::System.String senderLine, global::System.String instanceInfos, global::System.String deviceInfo, global::System.String filesInfo, global::System.String folder, global::System.String responsibleId)
    {
    global::Sungero.Capture.Client.ModuleFunctions.ImportDocument(senderLine, instanceInfos, deviceInfo, filesInfo, folder, responsibleId);
    }
    /// <redirect project="Sungero.Capture.ClientBase" type="Sungero.Capture.Client.ModuleFunctions" />
    internal static System.Xml.Linq.XDocument GetXDocumentFromFile(global::System.String path)
    {
        return global::Sungero.Capture.Client.ModuleFunctions.GetXDocumentFromFile(path);
    }

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
      return new global::Sungero.Capture.Client.ModuleFunctions();
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
/// ModuleClientPublicFunctions.g.cs
/// ==================================================================

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sungero.Capture.Client
{
  public partial class ModuleClientPublicFunctions : global::Sungero.Capture.Client.IModuleClientPublicFunctions
  {
  }
}

/// ==================================================================
/// ModuleWidgetHandlers.g.cs
/// ==================================================================

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sungero.Capture.Client
{
}
