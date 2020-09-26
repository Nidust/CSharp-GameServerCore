namespace Core.Server.Builder
{
    public interface IStartup
    {
        void PreBuild();
        void PostBuild();
        void Run();
    }
}
