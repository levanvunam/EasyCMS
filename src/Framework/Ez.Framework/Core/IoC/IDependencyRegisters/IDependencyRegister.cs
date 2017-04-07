using SimpleInjector;

namespace Ez.Framework.Core.IoC.IDependencyRegisters
{
    /// <summary>
    /// Interface for dependency register
    /// </summary>
    public interface IDependencyRegister
    {
        void Register(Container container);
    }
}
