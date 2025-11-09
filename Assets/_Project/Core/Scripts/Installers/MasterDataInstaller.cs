using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MasterDataInstaller : LifetimeScope
{
    // 定数は 'k_' プレフィックス
    // Resources フォルダからロードするためのパス
    private const string k_MasterDataPath = "MasterData/MenuItemMaster";

    protected override void Configure(IContainerBuilder builder)
    {
        // 1. Resources から MenuItemMaster (SO) をロードする
        var masterData = Resources.Load<MenuItemMaster>(k_MasterDataPath);

        if (masterData == null)
        {
            Debug.LogError($"[MasterDataInstaller] {k_MasterDataPath} に MenuItemMaster が見つかりません。");
            return;
        }

        // 2. IMasterDataRepository が要求されたら、
        //    MasterDataRepository のインスタンスを生成して返すよう DI コンテナに登録する
        //    コンストラクタ (masterData) を明示的に渡す

        // 2.1. 具象クラスの MasterDataRepository を Singleton として登録する
        builder.Register<MasterDataRepository>(Lifetime.Singleton)
            // 2.2. MasterDataRepository のコンストラクタが MenuItemMaster 型を要求したら、
            //      ここでロードした masterData 変数を渡す
            .WithParameter<MenuItemMaster>(masterData)
            // 2.3. 最後に、 IMasterDataRepository インターフェースとして登録する
            .As<IMasterDataRepository>();
    }
}
