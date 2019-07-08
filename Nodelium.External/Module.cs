namespace Nodelium.External
{
    public abstract class Module
    {
        public abstract void Load();

        public void Register<TRegister, TAs>()
            where TRegister : TAs, new()
        {
        }
    }
}