using Ez.Framework.Configurations;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Ez.Framework.Utilities.Files
{
    /// <summary>
    /// File utilities
    /// </summary>
    public static class FileUtilities
    {
        #region Extension

        const string ImagePattern = @"^.*\.(jpg|JPG|gif|GIF|png|PNG|jpeg|JPEG|tif|TIF)$";

        /// <summary>
        /// Get extension from fole name
        /// </summary>
        /// <param name="path"></param>
        /// <param name="removeDot"></param>
        /// <returns></returns>
        public static string GetExtension(this string path, bool removeDot = false)
        {
            var extension = Path.GetExtension(path);
            if (extension != null && removeDot) return extension.Replace(".", "");

            return extension;
        }

        /// <summary>
        /// Get mime mapping
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetMimeMapping(this string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            return MimeMapping.GetMimeMapping(extension);
        }

        /// <summary>
        /// Get mime type
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string GetMimeMapping(this FileInfo fileInfo)
        {
            var extension = fileInfo.Extension;

            return GetMimeMapping(extension);
        }

        /// <summary>
        /// Check if url is static file request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsStaticFileRequest(this string url)
        {
            var extension = GetExtension(url);

            if (!string.IsNullOrEmpty(extension))
            {
                var mimeType = MimeMapping.GetMimeMapping(extension);
                if (!string.IsNullOrEmpty(mimeType))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Images

        /// <summary>
        /// Get img format from path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormatFromPath(this string path)
        {
            string extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
                return ImageFormat.Png;

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    return ImageFormat.Png;
            }
        }

        /// <summary>
        /// Detect if a file is image
        /// </summary>
        /// <param name="filename">File name for testing</param>
        /// <returns>true if file is image, otherwise false</returns>
        public static Boolean IsImage(this string filename)
        {
            return Regex.IsMatch(filename, ImagePattern);
        }

        #endregion

        #region Directory

        /// <summary>
        /// Copy directory
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="copySubDirs"></param>
        public static void CopyDirectory(this string sourcePath, string destinationPath, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourcePath);
            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourcePath);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destinationPath, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (!copySubDirs) return;

            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destinationPath, subdir.Name);
                CopyDirectory(subdir.FullName, temppath, true);
            }
        }

        /// <summary>
        /// Delete a directory and all of sub-folders/items
        /// </summary>
        /// <param name="path">Path to the directory to delete</param>
        public static void DeleteDirectory(this string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
                throw new Exception("Folder not found at path " + path);

            DeleteDirectory(directoryInfo);
        }

        /// <summary>
        /// Delete a directory and all of sub-folders/items
        /// </summary>
        /// <param name="directory">Directory to delete</param>
        public static void DeleteDirectory(this DirectoryInfo directory)
        {
            foreach (var childDirectory in directory.GetDirectories())
            {
                DeleteDirectory(childDirectory);
            }
            foreach (var f in directory.GetFiles())
            {
                SetFileAttributeAsArchive(f.FullName);
                f.Delete();
            }
            directory.Delete();
        }

        /// <summary>
        /// Set file attr as archive
        /// </summary>
        /// <param name="filepath"></param>
        private static void SetFileAttributeAsArchive(this string filepath)
        {
            if ((File.GetAttributes(filepath)
                   & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(filepath, FileAttributes.Archive);
            }
        }

        #endregion

        /// <summary>
        /// Convert path to relative path
        /// </summary>
        /// <param name="physicalPath"></param>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public static string ToRelativePath(this string physicalPath, string defaultPath)
        {
            if (Path.IsPathRooted(physicalPath))
            {
                if (HttpContext.Current.Request.PhysicalApplicationPath != null)
                    physicalPath = physicalPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, String.Empty);
                physicalPath = physicalPath.Replace("\\", "/");
            }

            if (!physicalPath.StartsWith("/"))
            {
                physicalPath = "/" + physicalPath;
            }

            var mediaPath = physicalPath.Replace(defaultPath, "");
            physicalPath = string.Format("{0}{1}", defaultPath, mediaPath);
            return physicalPath;
        }

        /// <summary>
        /// Get name of file/directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathName(this string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                return Path.GetFileNameWithoutExtension(path);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get right path to save
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRightFilePathToSave(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            if (File.Exists(path))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                fileName += DateTime.UtcNow.ToString(FrameworkConstants.UniqueDateTimeFormat);

                path = Path.Combine(Path.GetDirectoryName(path), fileName) + Path.GetExtension(path);
            }

            return path;
        }

        #region Save Files / Images

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        public static void SaveFile(string path, Stream stream)
        {
            using (var fileStream = File.Create(path))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }

        #endregion
    }
}
