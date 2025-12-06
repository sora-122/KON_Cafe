using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// MasterDataRepository のロジックを検証するEditModeテスト
/// </summary>
public class MasterDataRepositoryTests
{
    private IMasterDataRepository _repository;

    // テストで使用するロード済みテーブル
    private MenuItemMaster _loadedItemMaster;
    private AnimalMaster _loadedAnimalMaster;


    [SetUp]
    public void SetUp()
    {
        // --- 1．準備 (Arrange) ---
        // Unity Editor で作成した実データを使用する方針のため、Resources.Load を行う

        // A. メニューマスターのロード
        _loadedItemMaster = Resources.Load<MenuItemMaster>("MasterData/MenuItemMaster");
        Assert.IsNotNull(_loadedItemMaster, "前提条件: MenuItemMaster.asset が Resources/MasterData/ に存在しません。");
        // データが空でもテスト自体は動くように、Count チェックは一旦は外すか、Warn にする運用も可
        if (_loadedItemMaster.Items.Count == 0) Debug.LogWarning("MenuItemMaster にアイテムが登録されていません。");

        // B. アニマルマスターのロード
        _loadedAnimalMaster = Resources.Load<AnimalMaster>("MasterData/AnimalMaster");
        Assert.IsNotNull(_loadedAnimalMaster, "前提条件: AnimalMaster.asset が Resources/MasterData/ に存在しません。");

        // --- 2．テスト対象のクラスを初期化 ---
        _repository = new MasterDataRepository(_loadedItemMaster, _loadedAnimalMaster);
    }

    /// <summary>
    /// メニューデータを ID で取得できるか検証
    /// </summary>
    [Test]
    public void Test_RepositoryCanGetItemById()
    {
        // データが存在しない場合はスキップ
        if (_loadedItemMaster.Items.Count == 0)
        {
            Assert.Ignore("MenuItemMaster にデータが無いためテストをスキップします。");
            return;
        }

        // 期待値: リストの最初のアイテムを正解とする
        var expectedItem = _loadedItemMaster.Items[0];
        string targetId = expectedItem.ItemId;

        // 実行
        var result = _repository.GetMenuItemById(targetId);

        // 検証
        Assert.IsNotNull(result, $"検証失敗: ID '{targetId}' でアイテムを取得できませんでした。");
        Assert.AreEqual(expectedItem.ItemId, result.ItemId, "検証失敗: 取得したアイテムの ID が異なります。");

        Debug.Log($"[Test Success] GetMenuItemById 検証成功: ID '{targetId}' で {result.DisplayName} を取得できました。");
    }

    /// <summary>
    /// アニマルデータを ID で取得できるか検証
    /// </summary>
    [Test]
    public void Test_RepositoryCanGetAnimalById()
    {
        // データが存在しない場合はスキップ
        if (_loadedAnimalMaster.Animals.Count == 0)
        {
            Assert.Ignore("AnimalMaster にデータが無いためテストをスキップします。");
            return;
        }

        // 期待値: リストの最初のアイテムを正解とする
        var expectedAnimal = _loadedAnimalMaster.Animals[0];
        string targetId = expectedAnimal.Id;

        // 実行
        var result = _repository.GetAnimalById(targetId);

        // 検証
        Assert.IsNotNull(result, $"検証失敗: ID '{targetId}' でアニマルを取得できませんでした。");
        Assert.AreEqual(expectedAnimal.Id, result.Id, "検証失敗: 取得したアニマルの ID が異なります。");

        Debug.Log($"[Test Success] GetAnimalById 検証成功: ID '{targetId}' で {result.DisplayName} を取得できました。");
    }
}
