using Silverlight_ImageUpload.Web.Data;
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
    // NOTE: If you change the class name "WCFImageService" here, you must also update the reference to "WCFImageService" in Web.config.
    public class WCFImageService : IWCFImageService
    {
        public bool Upload(Picture imageFile)
        {
            using (ImageDataClassesDataContext db = new ImageDataClassesDataContext())
            {
                db.images.InsertOnSubmit(new image { imagecontent = imageFile.ImageStream, imagename = imageFile.ImageTitle });
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    db.SubmitChanges();
                }
                return true;
            }
        }

        public List<Picture> ListImages()
        {
            using (ImageDataClassesDataContext db = new ImageDataClassesDataContext())
            {
                List<image> selectedImages = db.images.ToList<image>();
                List<Picture> pictures = new List<Picture>();
                if (selectedImages.Count > 0)
                    foreach (image img in selectedImages)
                        pictures.Add(new Picture { ImageID = img.imagesid, ImageTitle = img.imagename, ImageStream = img.imagecontent.ToArray() });
                return pictures;
            }
        }

        public Picture Download(int imageId)
        {
            using (ImageDataClassesDataContext db = new ImageDataClassesDataContext())
            {
                image selectedImage = db.images.Where(r => r.imagesid == imageId) as image;
                return new Picture { ImageID = selectedImage.imagesid, ImageTitle = selectedImage.imagename, ImageStream = selectedImage.imagecontent.ToArray() };
            }
        }
    }

}
