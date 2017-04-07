using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.Media;
using Ez.Framework.Core.Media.Models;
using Ez.Framework.Core.Mvc.MvcResults.Image;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Files;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Services.SiteSettings;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class MediaController : BackendController
    {
        private readonly IMediaFileManager _mediaFileManager;
        private readonly IMediaService _mediaService;
        private readonly ISiteSettingService _siteSettingService;

        public MediaController(IMediaService mediaService, ISiteSettingService siteSettingService
            , IMediaFileManager mediaFileManager)
        {
            _mediaFileManager = mediaFileManager;
            _mediaService = mediaService;
            _siteSettingService = siteSettingService;
        }

        #region Media Browser

        public ActionResult MediaBrowser(string rootFolder, string imageUrl, MediaEnums.MediaBrowserSelectMode mode = MediaEnums.MediaBrowserSelectMode.All)
        {
            if (string.IsNullOrEmpty(rootFolder))
            {
                rootFolder = FrameworkConstants.MediaPath;
            }

            var model = new MediaBrowserSetupModel
            {
                RootFolder = rootFolder,
                Mode = mode
            };

            if (!string.IsNullOrEmpty(imageUrl))
            {
                model.FileTreeAttribute = new FileTreeAttribute
                {
                    Id = imageUrl.ToIdString(),
                    Path = imageUrl,
                    IsImage = imageUrl.IsImage(),
                };
            }

            return View(model);
        }

        #region Post

        [HttpPost]
        public ActionResult FileUpload(string qqfile, string dir)
        {
            var physicalPath = Server.MapPath(dir);
            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            string file;
            try
            {
                var stream = Request.InputStream;
                if (String.IsNullOrEmpty(Request["qqfile"]))
                {
                    // IE
                    var postedFile = Request.Files[0];
                    if (postedFile != null)
                    {
                        stream = postedFile.InputStream;
                        file = Path.Combine(physicalPath, Path.GetFileName(postedFile.FileName));
                    }
                    else
                    {
                        return Json(new ResponseModel
                        {
                            Success = false,
                            Message = T("Media_Message_FileUploadEmpty")
                        });
                    }
                }
                else
                {
                    //Webkit, Mozilla
                    file = Path.Combine(physicalPath, qqfile);
                }

                //Check if path exists or not, if exists then assign new path for image
                file = file.GetRightFilePathToSave();

                if (file.IsImage())
                {
                    using (var img = System.Drawing.Image.FromStream(stream))
                    {
                        var imageUploadSetting = _siteSettingService.LoadSetting<ImageUploadSetting>();
                        if (imageUploadSetting != null)
                        {
                            if (imageUploadSetting.MinWidth.HasValue && img.Width < imageUploadSetting.MinWidth)
                            {
                                return
                                    Json(
                                        new
                                        {
                                            Success = false,
                                            Message = TFormat("Media_Message_InvalidMinWidth", imageUploadSetting.MinWidth)
                                        },
                                        "text/html");
                            }
                            if (imageUploadSetting.MinHeight.HasValue && img.Height < imageUploadSetting.MinHeight)
                            {
                                return
                                    Json(
                                        new ResponseModel
                                        {
                                            Success = false,
                                            Message = TFormat("Media_Message_InvalidMinHeight", imageUploadSetting.MinHeight)
                                        },
                                        "text/html");
                            }
                            if (imageUploadSetting.MaxWidth.HasValue && img.Width > imageUploadSetting.MaxWidth)
                            {
                                return
                                    Json(
                                        new ResponseModel
                                        {
                                            Success = false,
                                            Message = TFormat("Media_Message_InvalidMaxWidth", imageUploadSetting.MaxWidth)
                                        },
                                        "text/html");
                            }
                            if (imageUploadSetting.MaxHeight.HasValue && img.Height > imageUploadSetting.MaxHeight)
                            {
                                return
                                    Json(
                                        new ResponseModel
                                        {
                                            Success = false,
                                            Message = TFormat("Media_Message_InvalidMaxHeight", imageUploadSetting.MaxHeight)
                                        },
                                        "text/html");
                            }
                        }
                        img.Save(file);
                    }
                }
                else
                {
                    FileUtilities.SaveFile(file, stream);
                }
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel(ex), "text/html");
            }

            var isImage = file.IsImage();

            var location = string.Format("{0}/{1}", dir, Path.GetFileName(file));
            return Json(new
            {
                Success = true,
                Message = T("Media_Message_UploadSuccessfully"),
                fileLocation = location,
                id = location.ToIdString(),
                isImage
            }, "text/html");
        }

        [HttpPost]
        public JsonResult GetTreeData(string rootFolder, string dir)
        {
            if (string.IsNullOrEmpty(rootFolder))
            {
                rootFolder = FrameworkConstants.MediaPath;
            }

            if (string.IsNullOrWhiteSpace(dir))
            {
                var rootNode = new FileTreeModel
                {
                    attr = new FileTreeAttribute
                    {
                        Id = rootFolder.ToIdString(),
                        Rel = "home",
                        Path = rootFolder
                    },
                    state = "open",
                    data = rootFolder.Replace("/", "").CamelFriendly().ToUpper()
                };
                _mediaService.PopulateTree(rootFolder, rootNode);
                return Json(rootNode);
            }
            return Json(_mediaService.PopulateChild(dir));
        }

        [HttpPost]
        public ActionResult MoveData(string path, string destination, bool copy)
        {
            return Json(_mediaService.MoveData(path, destination, copy));
        }

        [HttpPost]
        public JsonResult CreateFolder(string path, string folder)
        {
            return Json(_mediaService.CreateFolder(path, folder));
        }

        [HttpPost]
        public JsonResult Delete(string path)
        {
            return Json(_mediaService.DeletePath(path));
        }

        [HttpPost]
        public JsonResult Rename(string path, string name)
        {
            return Json(_mediaService.Rename(path, name));
        }

        [HttpPost]
        public JsonResult GetFileInfo(string path)
        {
            try
            {
                path = _mediaService.MapPath(path);

                // get the file attributes for file or directory
                var attr = System.IO.File.GetAttributes(path);
                FileInfoModel model;

                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var info = new DirectoryInfo(path);
                    model = new FileInfoModel
                    {
                        FileName = info.Name,
                        Created = info.CreationTimeUtc.ToLongDateString(),
                        LastUpdate = info.LastWriteTimeUtc.ToLongDateString(),
                        FileSize = string.Empty
                    };
                }
                else
                {
                    var info = new FileInfo(path);
                    model = new FileInfoModel
                    {
                        FileName = info.Name,
                        Created = info.CreationTimeUtc.ToLongDateString(),
                        LastUpdate = info.LastWriteTimeUtc.ToLongDateString(),
                        FileSize = string.Format("{0} Bytes", info.Length),
                    };
                }
                return Json(new ResponseModel
                {
                    Success = true,
                    Data = model
                });

            }
            catch (Exception exception)
            {
                return Json(new ResponseModel
                {
                    Success = true,
                    Message = exception.Message
                });
            }
        }

        [HttpPost]
        public JsonResult CheckPathValid(string path, MediaEnums.MediaBrowserSelectMode mode)
        {
            return Json(_mediaService.CheckPathValid(path, mode));
        }

        #endregion

        #endregion

        #region Image Editor

        public ActionResult ImageEditor(string virtualPath)
        {
            if (!virtualPath.StartsWith("/"))
            {
                virtualPath = "/" + virtualPath;
            }
            return View((object)virtualPath);
        }

        [HttpPost]
        public ActionResult ImageEditor(string virtualPath, string data, string newname, bool overwrite = false)
        {
            try
            {
                if (data == null)
                {
                    return Json(new { result = MediaEnums.EditImageStatus.SaveFail, message = "no data" });
                }
                if (!virtualPath.StartsWith("/"))
                {
                    virtualPath = "/" + virtualPath;
                }

                var imageFormat = virtualPath.GetImageFormatFromPath();

                var physicalPath = _mediaFileManager.GetPhysicalPathFromVirtualPath(virtualPath);
                if (newname != null)
                {
                    var filename = Path.GetFileName(physicalPath);
                    if (filename != null)
                    {
                        physicalPath = physicalPath.Substring(0, physicalPath.Length - filename.Length) + newname;
                        if (!overwrite && System.IO.File.Exists(physicalPath))
                        {
                            return Json(new { result = MediaEnums.EditImageStatus.OverWriteConfirm });
                        }
                    }
                }
                ImageUtilities.SaveImageFromBase64String(data,
                    physicalPath,
                    imageFormat);
                return Json(new { result = MediaEnums.EditImageStatus.SaveSuccess });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                return Json(new { result = MediaEnums.EditImageStatus.SaveFail, message = ex.Message });
            }
        }

        #endregion

        #region CkEditor

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            var vImagePath = String.Empty;
            var vMessage = String.Empty;

            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var extension = Path.GetExtension(upload.FileName);
                    if (extension != null && _mediaService.IsImage(upload.FileName))
                    {
                        var vFileName = DateTime.UtcNow.ToString(FrameworkConstants.UniqueDateTimeFormat) +
                                        extension.ToLower();
                        var vFolderPath = Server.MapPath(EzCMSContants.UploadFolder);

                        if (!Directory.Exists(vFolderPath))
                        {
                            Directory.CreateDirectory(vFolderPath);
                        }

                        string vFilePath = Path.Combine(vFolderPath, vFileName);
                        upload.SaveAs(vFilePath);

                        vImagePath = Url.Content(EzCMSContants.UploadFolder + vFileName);
                        vMessage = T("Media_Message_SaveImageSuccessfully");
                    }
                    else
                    {
                        vMessage = T("Media_Message_WrongFileType");
                    }
                }
            }
            catch
            {
                vMessage = T("Media_Message_UploadFailure");
            }
            var vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";

            return Content(vOutput);
        }

        #endregion

        public ImageResult Thumbnail(string path, int w = 0, int h = 0)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (!path.IsAbsoluteUrl())
                {
                    var filePath = Path.Combine("~", path);
                    filePath = Server.MapPath(filePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        return new ImageResult(filePath, w, h);
                    }
                }
            }

            var noImagePath = Server.MapPath(EzCMSContants.NoImage);
            return new ImageResult(noImagePath);
        }
    }
}
