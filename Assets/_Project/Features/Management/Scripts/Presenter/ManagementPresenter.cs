using VContainer.Unity;
using UnityEngine;

public class ManagementPresenter : IStartable
{
    private readonly ManagementModel _model;
    private readonly ManagementView _view;
    private readonly ISceneLoader _sceneLoader;

    // コンストラクタインジェクション (依存性注入)
    public ManagementPresenter(
        ManagementModel model,
        ManagementView view,
        ISceneLoader sceneLoader // 親 Scope (MasterDataInstaller) から注入される
    )
    {
        _model = model;
        _view = view;
        _sceneLoader = sceneLoader;
    }

    public void Start()
    {
        // イベント購読
        _view.OnBackButtonClicked += HandleBackClicked;

        Debug.Log("[ManagementPresenter] 経営パート開始");
    }

    private void HandleBackClicked()
    {
        // Home へ戻る
        _sceneLoader.LoadScene(SceneName.Home);
    }
}
