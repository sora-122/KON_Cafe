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

        // --- Model の登録 ---
        // スコープ内で1つ (Singleton的な扱い)
        builder.Register<CafeModel>(Lifetime.Scoped);

        // --- Presenter の登録 ---
        // EntryPoint (Startable) として登録
        // これにより Start() が自動で呼ばれる
        builder.RegisterEntryPoint<CafePresenter>(Lifetime.Scoped);
    }
}
