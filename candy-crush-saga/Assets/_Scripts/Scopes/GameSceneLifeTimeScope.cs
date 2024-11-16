using GameCore.Managers;
using Interface;
using VContainer;
using VContainer.Unity;

namespace Scopes
{
    public class GameSceneLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<GameManager>().As<IGameManager>();
        }
    }
}
