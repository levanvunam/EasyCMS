using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Ez.Framework.Core.Media
{
    public class MediaFileManager : IMediaFileManager
    {
        private DirectoryInfo _mediaFolder;

        /// <summary>
        /// Gets the Media folder
        /// </summary>
        public DirectoryInfo MediaFolder
        {
            get
            {
                if (_mediaFolder != null)
                    return _mediaFolder;
                const string fullFolderPath = "/Media/";
                _mediaFolder = !Directory.Exists(fullFolderPath)
                                   ? Directory.CreateDirectory(fullFolderPath)
                                   : new DirectoryInfo(fullFolderPath);
                return _mediaFolder;
            }
        }

        /// <summary>
        /// Gets the Images folder
        /// </summary>
        public DirectoryInfo ImagesFolder
        {
            get
            {
                var folderPath = Path.Combine(MediaFolder.FullName, "Images");

                return Directory.Exists(folderPath)
                           ? new DirectoryInfo(folderPath)
                           : Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// Gets the Files folder
        /// </summary>
        public DirectoryInfo FilesFolder
        {
            get
            {
                var folderPath = Path.Combine(MediaFolder.FullName, "Files");

                return Directory.Exists(folderPath)
                           ? new DirectoryInfo(folderPath)
                           : Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// Gets the virtual path from physical path
        /// </summary>
        /// <param name="physicalPath">The physical path</param>
        /// <returns></returns>
        public string GetVirtualMediaPathFromPhysicalPath(string physicalPath)
        {
            if (Path.IsPathRooted(physicalPath))
            {
                if (HttpContext.Current.Request.PhysicalApplicationPath != null)
                    physicalPath = physicalPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, String.Empty);

                physicalPath = physicalPath.Replace("\\", "/");

                var folder = string.Format("Media/");
                physicalPath = physicalPath.Replace(folder, "/Media").Trim('/');

                if (!physicalPath.StartsWith("/"))
                {
                    physicalPath = "/" + physicalPath;
                }
            }

            return physicalPath;
        }

        /// <summary>
        /// Gets physical path from a virtual path which has the tenant name trimmed.
        /// </summary>
        /// <param name="virtualPath">The virtual path with tenant name trimmed</param>
        /// <returns></returns>
        public string GetPhysicalPathFromVirtualPath(string virtualPath)
        {
            return HttpContext.Current.Server.MapPath("~/" + virtualPath);
        }

        /// <summary>
        /// Detect if a file is image
        /// </summary>
        /// <param name="filename">File name for testing</param>
        /// <returns>true if file is image, otherwise false</returns>
        public Boolean IsImage(string filename)
        {
            const string imagePattern = @"^.*\.(jpg|JPG|gif|GIF|png|PNG|jpeg|JPEG|tif|TIF)$";
            return Regex.IsMatch(filename, imagePattern);
        }
    }
}
