using System.IO;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;

namespace Ez.Framework.Core.Media
{
    [Register(Lifetime.PerInstance)]
    public interface IMediaFileManager
    {
        /// <summary>
        /// Gets the Media folder of the particular tenant
        /// </summary>
        DirectoryInfo MediaFolder { get; }

        /// <summary>
        /// Gets the Images folder of the particular tenant
        /// </summary>
        DirectoryInfo ImagesFolder { get; }

        /// <summary>
        /// Gets the Files folder of the particular tenant
        /// </summary>
        DirectoryInfo FilesFolder { get; }

        /// <summary>
        /// Gets the virtual path from physical path
        /// </summary>
        /// <param name="physicalPath">The physical path</param>
        /// <returns></returns>
        string GetVirtualMediaPathFromPhysicalPath(string physicalPath);

        /// <summary>
        /// Gets physical path from a virtual path which has the tenant name trimmed.
        /// </summary>
        /// <param name="virtualPath">The virtual path with tenant name trimmed</param>
        /// <returns></returns>
        string GetPhysicalPathFromVirtualPath(string virtualPath);

        /// <summary>
        /// Detect if a file is image
        /// </summary>
        /// <param name="filename">File name for testing</param>
        /// <returns>true if file is image, otherwise false</returns>
        bool IsImage(string filename);
    }
}
