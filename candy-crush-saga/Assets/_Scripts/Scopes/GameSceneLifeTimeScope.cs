using Interface;
using VContainer;
using VContainer.Unity;

namespace Scopes
{
    public class GameSceneLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ScoreManager>().As<IScoreManager>();
        }
    }
}
