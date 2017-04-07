using System;
using System.Collections.Generic;
using System.Web;

namespace Ez.Framework.Core.Mvc.Context
{
    /// <summary>
    /// A context in which to add references to style files and blocks of style
    /// to be rendered to the view at a later point.
    /// </summary>
    public class StyleContext : IDisposable
    {
        public const string StyleContextItem = ":::StyleContextItem:::";
        public const string StyleContextItems = ":::StyleContextItems:::";

        public const string RequiredKey = "RequiredKey";
        public const string IncludedKey = "IncludedKey";

        private readonly IList<string> _styleBlocks = new List<string>();
        private readonly IDictionary<string, IList<string>> _styleSheetPaths = new Dictionary<string, IList<string>>
            {
                {RequiredKey, new List<string>()},
                {IncludedKey, new List<string>()}
            };

        /// <summary>
        /// Gets the Style blocks
        /// </summary>
        public IList<string> StyleBlocks
        {
            get { return _styleBlocks; }
        }

        /// <summary>
        /// Gets the Style files
        /// </summary>
        public IDictionary<string, IList<string>> StyleSheetPaths
        {
            get { return _styleSheetPaths; }
        }

        /// <summary>
        /// Add file to required styles
        /// </summary>
        /// <param name="virtualPath"></param>
        public void Require(string virtualPath)
        {
            var checkExist = _styleSheetPaths[RequiredKey].Contains(virtualPath);

            if (!checkExist)
                _styleSheetPaths[RequiredKey].Add(virtualPath);

        }

        /// <summary>
        /// Include file to styles
        /// </summary>
        /// <param name="virtualPath"></param>
        public void Include(string virtualPath)
        {
            var checkExist = _styleSheetPaths[IncludedKey].Contains(virtualPath) && _styleSheetPaths[RequiredKey].Contains(virtualPath);

            if (!checkExist)
                _styleSheetPaths[IncludedKey].Add(virtualPath);
        }

        /// <summary>
        /// Implement IDisposable.Dispose()
        /// </summary>
        public void Dispose()
        {
            var styleContexts = HttpContext.Current.Items[StyleContextItems] as Stack<StyleContext> ?? new Stack<StyleContext>();

            styleContexts.Push(this);

            HttpContext.Current.Items[StyleContextItems] = styleContexts;
        }
    }
}
