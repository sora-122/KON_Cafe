using VContainer.Unity;

public class HomePresenter : IStartable
{
    private readonly HomeView _view;
    private readonly UserModel _userModel;
    private readonly ISceneLoader _sceneLoader;
    private readonly IRankDataRepository _rankRepository;

    // コンストラクタインジェクション (依存性注入)
    public HomePresenter(
        HomeView view,
        UserModel userModel,
        ISceneLoader sceneLoader,
        IRankDataRepository rankRepository
        )
    {
        _view = view;
        _userModel = userModel;
        _sceneLoader = sceneLoader;
        _rankRepository = rankRepository;
    }

    public void Start()
    {
        // イベント購読
        _view.OnCafeButtonClicked += HandleCafeClicked;
        _view.OnRankButtonClicked += HandleRankClicked;
        _view.OnManagementButtonClicked += HandleManagementClicked;

        // 「カフェへ行く」の確認でYESを押した時
        _view.OnConfirmYesClicked += () =>
        {
            _sceneLoader.LoadScene(SceneName.Cafe);
        };
    }

    private void HandleCafeClicked()
    {
        // 確認ポップアップを表示
        _view.ShowConfirmPopup();
    }

    private void HandleManagementClicked()
    {
        // 経営画面即遷移 (確認無し)
        _sceneLoader.LoadScene(SceneName.Management);
    }

    private void HandleRankClicked()
    {
        // ユーザーデータ取得
        var data = _userModel.GetData();

        // 次のランクまでの必要経験値を取得
        var rankData = _rankRepository.GetRankData(data.Rank);
        int nextExp = rankData.TotalExpToNextRank;

        // View に表示指示
        _view.ShowRankPopup(data.Rank, data.Experience, nextExp);
    }
}
