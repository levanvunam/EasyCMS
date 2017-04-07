using System;
using System.Collections.Generic;
using System.Web;

namespace Ez.Framework.Core.Mvc.Context
{
    /// <summary>
    /// A context in which to add references to script files and blocks of script
    /// to be rendered to the view at a later point.
    /// </summary>
    public class ScriptContext : IDisposable
    {
        public const string ScriptContextItem = ":::ScriptContextItem:::";
        public const string ScriptContextItems = ":::ScriptContextItems:::";

        public const string RequiredKey = "RequiredKey";
        public const string IncludedKey = "IncludedKey";

        private readonly List<string> _scriptBlocks = new List<string>();
        private readonly Dictionary<string, List<string>> _scriptPaths = new Dictionary<string, List<string>>
            {
                {RequiredKey, new List<string>()},
                {IncludedKey, new List<string>()}
            };

        /// <summary>
        /// Gets the script blocks
        /// </summary>
        public IList<string> ScriptBlocks
        {
            get { return _scriptBlocks; }
        }

        /// <summary>
        /// Gets the script files
        /// </summary>
        public IDictionary<string, List<string>> ScriptPaths
        {
            get { return _scriptPaths; }
        }

        /// <summary>
        /// Add file to required scripts
        /// </summary>
        /// <param name="virtualPath"></param>
        public void Require(string virtualPath)
        {
            var checkExist = _scriptPaths[RequiredKey].Contains(virtualPath);

            if (!checkExist)
                _scriptPaths[RequiredKey].Add(virtualPath);
        }

        /// <summary>
        /// Add file to include scripts
        /// </summary>
        /// <param name="virtualPath"></param>
        public void Include(string virtualPath)
        {
            var checkExist = _scriptPaths[IncludedKey].Contains(virtualPath) && _scriptPaths[RequiredKey].Contains(virtualPath);

            if (!checkExist)
                _scriptPaths[IncludedKey].Add(virtualPath);
        }

        /// <summary>
        /// Implement IDisposable.Dispose()
        /// </summary>
        public void Dispose()
        {
            var scriptContexts = HttpContext.Current.Items[ScriptContextItems] as Stack<ScriptContext> ?? new Stack<ScriptContext>();

            scriptContexts.Push(this);

            HttpContext.Current.Items[ScriptContextItems] = scriptContexts;
        }
    }
}