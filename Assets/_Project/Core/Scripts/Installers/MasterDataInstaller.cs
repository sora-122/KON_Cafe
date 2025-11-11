using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MasterDataInstaller : LifetimeScope
{
    // 定数は 'k_' プレフィックス
    // Resources フォルダからロードするためのパス
    private const string k_ItemMasterPath = "MasterData/MenuItemMaster";

    private const string k_RankTablePath = "MasterData/RankExperienceTable";

    protected override void Configure(IContainerBuilder builder)
    {
        // 1. Resources から MenuItemMaster (SO) をロードする
        var itemMaster = Resources.Load<MenuItemMaster>(k_ItemMasterPath);
        if (itemMaster == null)
        {
            Debug.LogError($"[MasterDataInstaller] {k_ItemMasterPath} に MenuItemMaster が見つかりません。");
        }
        else
        {
            // 2. IMasterDataRepository が要求されたら、
            //    MasterDataRepository のインスタンスを生成して返すよう DI コンテナに登録する
            //    コンストラクタ (masterData) を明示的に渡す

            // 2.1. 具象クラスの MasterDataRepository を Singleton として登録する
            builder.Register<MasterDataRepository>(Lifetime.Singleton)
                // 2.2. MasterDataRepository のコンストラクタが MenuItemMaster 型を要求したら、
                //      ここでロードした masterData 変数を渡す
                .WithParameter<MenuItemMaster>(itemMaster)
                // 2.3. 最後に、 IMasterDataRepository インターフェースとして登録する
                .As<IMasterDataRepository>();
        }

        // --- 2．RankExperienceTable ---
        var rankTable = Resources.Load<RankExperienceTable>(k_RankTablePath);
        if (rankTable == null)
        {
            Debug.LogError($"[MasterDataInstaller] {k_RankTablePath} に RankExperienceTable が見つかりません。");
        }
        else
        {
            // 1．と同じパターンで RankDataRepository を登録する
            builder.Register<RankDataRepository>(Lifetime.Singleton)
                .WithParameter<RankExperienceTable>(rankTable)
                .As<IRankDataRepository>();
        }
    }
}
