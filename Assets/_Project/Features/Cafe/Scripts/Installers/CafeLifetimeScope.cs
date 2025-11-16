using VContainer;
using VContainer.Unity;

/// <summary>
/// カフェ運営シーン専用の LifetimeScope
/// </summary>
public class CafeLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // --- View の登録 ---
        // このシーンのヒエラルキーに存在する CafeView を見つけ、
        // ICafeView としてコンテナに登録する
        builder.RegisterComponentInHierarchy<CafeView>().As<ICafeView>();

        // --- Presenter / Model の登録 ---
        // (※将来のタスクで、ここに Presenter と Model を登録していく)
        // builder.Register<CafePresenter>(Lifetime.Scoped).AsSelf();
        // builder.Register<CafeModel>(Lifetime.Scoped).AsSelf();
    }
}
