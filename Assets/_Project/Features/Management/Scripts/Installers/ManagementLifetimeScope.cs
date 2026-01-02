using VContainer;
using VContainer.Unity;
using UnityEngine;

public class ManagementLifetimeScope : LifetimeScope
{
    [SerializeField] private ManagementView _view;

    protected override void Configure(IContainerBuilder builder)
    {
        // 1. View の登録
        if (_view != null)
        {
            builder.RegisterComponent(_view);
        }
        else
        {
            // ヒエラルキーにアタッチし忘れたときの保険
            builder.RegisterComponentInHierarchy<ManagementView>();
        }

        // 2. Model の登録
        builder.Register<ManagementModel>(Lifetime.Scoped);

        // 3. Presenter の登録 (EntryPoint)
        builder.RegisterEntryPoint<ManagementPresenter>(Lifetime.Scoped);
    }
}
