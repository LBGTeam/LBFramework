using LBFramework.LBUtils;

namespace LBFramework
{
    public interface IPackageKitView
    {

        bool Ignore { get; }

        bool Enabled { get; }
        

        void OnUpdate();
        void OnGUI();

        void OnDispose();
        
    }
}