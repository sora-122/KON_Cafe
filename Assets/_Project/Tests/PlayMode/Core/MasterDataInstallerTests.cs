using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;
// using VContainer.Tests;
using System.Collections;

/// <summary>
/// MasterDataInstaller の結合テスト (Play Mode)
/// VContainer.Tests ユーティリティに依存しないバージョン
/// </summary>
public class MasterDataInstallerTests
{
    private IObjectResolver _container; // コンテナをフィールドで保持しておく

    private IEnumerator BuildScopeAsync<T>() where T : LifetimeScope
    {
        // 1．新しい GameObject を作成
        var go = new GameObject("TestScope");

        // 2．テスト対象の Installer (LifetimeScope) をアタッチ
        var scope = go.AddComponent<T>();

        // 3．スコープをビルド (DIコンテナを構築)
        // (非同期でビルドが完了するのを待つ場合があるため、1フレーム待機)
        scope.Build();
        yield return null;

        // ビルドしたコンテナを取得してフィールドに保存
        _container = LifetimeScope.Find<T>().Container;
    }

    /// <summary>
    /// Installer が全てのマスターデータリポジトリを正しく登録できるか
    /// </summary>
    [UnityTest]
    public IEnumerator Installer_CorrectlyRegisters_AllRepositories()
    {
        // --- 1．準備 (Arrange) ---
        // MasterDataInstaller をビルドし、コンテナを _container にセット
        yield return BuildScopeAsync<MasterDataInstaller>();

        Assert.IsNotNull(_container, "コンテナの取得に失敗しました。");

        // --- 2．実行 (Act) & 検証 (Assert) ---

        // (A) IMasterDataRepository
        IMasterDataRepository itemRepository = null;
        try
        {
            itemRepository = _container.Resolve<IMasterDataRepository>();
        }
        catch (VContainerException ex)
        {
            // 取得に失敗した場合 (例外が発生した場合)
            Debug.LogError(ex);
            Assert.Fail("IMasterDataRepository の Resolve に失敗しました。MasterDataRepository を確認してください。");
        }
        Assert.IsNotNull(itemRepository, "Resolve された ItemRepository が null です。");
        Assert.IsInstanceOf<MasterDataRepository>(itemRepository, "Resolve された型が MasterDataRepository ではありません。");

        // (B) IRankDataRepository
        IRankDataRepository rankRepository = null;
        try
        {
            rankRepository = _container.Resolve<IRankDataRepository>();
        }
        catch (VContainerException ex)
        {
            // 取得に失敗した場合 (例外が発生した場合)
            Debug.LogError(ex);
            Assert.Fail("IRankDataRepository の Resolve に失敗しました。RankDataRepository を確認してください。");
        }
        Assert.IsNotNull(rankRepository, "Resolve された RankRepository が null です。");
        Assert.IsInstanceOf<RankDataRepository>(rankRepository, "Resolve された型が RankDataRepository ではありません。");

        Debug.Log("[Test Success] MasterDataInstaller 結合テスト成功。すべてのリポジトリが正しく登録されています。");
    }
}
