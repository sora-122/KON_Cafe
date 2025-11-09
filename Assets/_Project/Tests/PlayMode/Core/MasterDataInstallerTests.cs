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
    }
    
    /// <summary>
    /// MasterDataInstaller を含む LifetimeScope をビルドし、
    /// IMasterDataRepository がコンテナから正しく取得できるかを検証する
    /// </summary>
    [UnityTest]
    public IEnumerator Installer_CorrectlyRegisters_IMasterDataRepository()
    {
        // --- 1．準備 (Arrange) ---
        yield return BuildScopeAsync<MasterDataInstaller>();

        // スコープからコンテナを取得 (VContainer 1.x の標準的な取得方法)
        var container = LifetimeScope.Find<MasterDataInstaller>().Container;
        Assert.IsNotNull(container, "コンテナの取得に失敗しました。");

        // --- 2．実行 (Act) ---
        IMasterDataRepository repository = null;
        try
        {
            repository = container.Resolve<IMasterDataRepository>();
        }
        catch (VContainerException ex)
        {
            // 取得に失敗した場合 (例外が発生した場合)
            Debug.LogError(ex);
            Assert.Fail("DIコンテナからの Resolve に失敗しました。MasterDataRepository を確認してください。");
        }

        // --- 3．検証 (Assert) ---
        Assert.IsNotNull(repository, "Resolve された Repository が null です。");
        Assert.IsInstanceOf<MasterDataRepository>(repository, "Resolve された型が MasterDataRepository ではありません。");

        Debug.Log("[Test Success] MasterDataRepository 結合テスト成功。");
    }
}
