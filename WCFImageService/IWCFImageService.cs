using Silverlight_ImageUpload.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFImageService
{
    [ServiceContract]
    public interface IWCFImageService
    {
        [OperationContract]
        bool Upload(Picture imageFile);

        [OperationContract]
        List<Picture> ListImages();

        [OperationContract]
        Picture Download(int imageId);
    }
}
