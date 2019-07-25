namespace Cotidico.External
{
    public abstract class Module
    {
        public abstract void Load();

        public void Register<TRegister, TAs>()
            where TRegister : class, TAs
        {
        }
    }
}