using SimpleInjector;
using System.Collections.Generic;

namespace Ez.Framework.Core.IoC
{
    public static class HostContainer
    {
        private static Container _container;

        /// <summary>
        /// Set the container
        /// </summary>
        /// <param name="container"></param>
        public static void SetContainer(Container container)
        {
            _container = container;
        }

        /// <summary>
        /// Get instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T : class
        {
            return _container.GetInstance<T>();
        }

        /// <summary>
        /// Get all instances
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetInstances<T>() where T : class
        {
            return _container.GetAllInstances<T>();
        }
    }
}
