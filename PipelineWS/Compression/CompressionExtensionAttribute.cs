using System;
using System.Web.Services;
using System.Web.Services.Protocols;
namespace PipelineWS.Compression
{
/// <summary>
  /// Summary description for CompressionExtensionAttribute.
  /// </summary>
  // Make the Attribute only Applicable to Methods
  [AttributeUsage(AttributeTargets.Method)]
  public class CompressionExtensionAttribute : SoapExtensionAttribute
  {

    private int priority;

    // Override the base class properties
    public override Type ExtensionType
    {
      get { return typeof(CompressionExtension); }
    }

    public override int Priority
    {
      get
      {
        return priority;
      }
      set
      {
        priority = value;
      }
    }

  }
}

