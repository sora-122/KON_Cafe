using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MasterDataInstaller : LifetimeScope
{
    // 定数は 'k_' プレフィックス
    // Resources フォルダからロードするためのパス
    private const string k_ItemMasterPath = "MasterData/MenuItemMaster";
    private const string k_AnimalMasterPath = "MasterData/AnimalMaster";
    private const string k_RankTablePath = "MasterData/RankExperienceTable";

    protected override void Configure(IContainerBuilder builder)
    {
        // --- 1. MasterDataRepository 関連 (Menu & Animal) ---

        // 1-A. Resources から MenuItemMaster (SO) をロード
        var itemMaster = Resources.Load<MenuItemMaster>(k_ItemMasterPath);
        if (itemMaster == null)
        {
            Debug.LogError($"[MasterDataInstaller] {k_ItemMasterPath} に MenuItemMaster が見つかりません。");
        }

        // 1-B. Resources から AnimalMaster (SO) をロード
        var animalMaster = Resources.Load<AnimalMaster>(k_AnimalMasterPath);
        if (animalMaster == null)
        {
            Debug.LogError($"[MasterDataInstaller] {k_AnimalMasterPath} に AnimalMaster が見つかりません。");
        }

        // 両方のマスターデータが揃っている場合のみリポジトリを登録する
        if (itemMaster != null && animalMaster != null)
        {
            // 2. IMasterDataRepository が要求されたら、
            //    MasterDataRepository のインスタンスを生成して返すよう DI コンテナに登録する

            // 2.1. 具象クラスの MasterDataRepository を Singleton として登録する
            builder.Register<MasterDataRepository>(Lifetime.Singleton)
                // 2.2. コンストラクタ引数1: MenuItemMaster を渡す
                .WithParameter<MenuItemMaster>(itemMaster)
                // 2.3. コンストラクタ引数2: AnimalMaster を渡す
                .WithParameter<AnimalMaster>(animalMaster)
                // 2.4. 最後に、 IMasterDataRepository インターフェースとして登録する
                .As<IMasterDataRepository>();
        }

        // --- 3．RankExperienceTable ---
        var rankTable = Resources.Load<RankExperienceTable>(k_RankTablePath);
        if (rankTable == null)
        {
            Debug.LogError($"[MasterDataInstaller] {k_RankTablePath} に RankExperienceTable が見つかりません。");
        }
        else
        {
            // RankDataRepository を登録する
            builder.Register<RankDataRepository>(Lifetime.Singleton)
                .WithParameter<RankExperienceTable>(rankTable)
                .As<IRankDataRepository>();
        }

        // PlayerDataRepository の登録
        builder.Register<PlayerDataRepository>(Lifetime.Singleton)
            .As<IPlayerDataRepository>();

        // UserModel の登録
        builder.Register<UserModel>(Lifetime.Singleton);

        // SceneLoader の登録
        builder.Register<SceneLoader>(Lifetime.Singleton)
            .As<ISceneLoader>();
    }
}
