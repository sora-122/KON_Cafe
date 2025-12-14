using VContainer;
using VContainer.Unity;
using UnityEngine;

public class HomeLifetimeScope : LifetimeScope
{
    [SerializeField] private HomeView _view;

    protected override void Configure(IContainerBuilder builder)
    {
        // MasterDataInstaller などで登録された Core機能 は親Scopeから継承される前提
        // ここではこのシーン固有ものを登録する

        builder.RegisterComponent(_view);
        builder.RegisterEntryPoint<HomePresenter>();
    }
}
