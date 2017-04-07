using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Media.Models;
using Ez.Framework.IoC.Attributes;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ez.Framework.Core.Media
{
    [Register(Lifetime.PerInstance)]
    public interface IMediaService
    {
        #region File Manager

        string MapPath(string virtualPath);

        DirectoryInfo DataDirectory { get; }

        /// <summary>
        /// Gets the current site media directory
        /// </summary>
        /// <returns></returns>
        Boolean IsImage(string filename);

        /// <summary>
        /// Copy a directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination</param>
        /// <param name="copySubDirs">Enable copy sub directories or not</param>
        void CopyDirectory(string sourcePath, string destinationPath, bool copySubDirs);

        /// <summary>
        /// Delete a directory and all of sub-folders/items
        /// </summary>
        /// <param name="path">Path to the directory to delete</param>
        void DeleteDirectory(string path);

        /// <summary>
        /// Check if current path is valid
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        ResponseModel CheckPathValid(string path, MediaEnums.MediaBrowserSelectMode mode);

        #endregion

        #region Media

        /// <summary>
        /// Populate master tree
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="node"></param>
        void PopulateTree(string relativePath, FileTreeModel node);

        /// <summary>
        /// Populate child from relative path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        List<FileTreeModel> PopulateChild(string relativePath);

        /// <summary>
        /// Move data from source to target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="isCopy"></param>
        /// <returns></returns>
        ResponseModel MoveData(string source, string target, bool isCopy);

        /// <summary>
        /// Move directory from souce to target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="isCopy"></param>
        void MoveDirectory(string source, string target, bool isCopy);

        /// <summary>
        /// Create folder
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        ResponseModel CreateFolder(string path, string folder);

        /// <summary>
        /// Delete file/folder
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        ResponseModel DeletePath(string relativePath);

        /// <summary>
        /// Rename file/folder
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        ResponseModel Rename(string relativePath, string name);

        /// <summary>
        /// Get valid name for saving file/folder
        /// </summary>
        /// <param name="targetFolder"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        string GetRightFileNameToSave(string targetFolder, string file);

        #endregion
    }
}