namespace Ez.Framework.Core.IoC.Attributes
{
    /// <summary>
    /// Life time for IoC
    /// </summary>
    public enum Lifetime
    {
        PerInstance,
        PerRequest,
        SingleTon
    }
}