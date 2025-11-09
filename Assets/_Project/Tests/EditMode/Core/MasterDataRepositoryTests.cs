using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MasterDataRepository のロジックを検証するEditModeテスト
/// </summary>
public class MasterDataRepositoryTests
{
    private IMasterDataRepository _repository;
    private MenuItemMaster _mockMaster;

    private MenuItemData _itemA;
    private MenuItemData _itemB;

    [SetUp]
    public void SetUp()
    {
        // --- 1．テスト用のダミーデータ (SO) を作成 ---
        // (通常SOはUnityエディターで作成するが、テストではScriptableObject.CreateInstance でメモリ上に作成)

        _mockMaster = ScriptableObject.CreateInstance<MenuItemMaster>();

        _itemA = ScriptableObject.CreateInstance<MenuItemData>();
        // リフレクション等で private Field をセットする代わりに、
        // テストを容易にするために Item ID をセットするメソッドを MenuItemData に用意する
        // (※またはJsonUtility.FromJsonOverwrite で疑似的にデータを設定する)
        // (※ここでは簡潔さのため、もし MenuItemData の setter が internal/public ならそれを使う想定。
        //    もし private のみなら、このテストコードはコンパイルエラーになるか、
        //    MenuItemData に #if UNITY_EDITOR ... internal void SetItemIdForTest() ... #endif
        //    といったテスト用メソッドが必要になる。)

        // 仮: テストのため、MenuItemData に以下のようなメソッドを追加したと仮定します。
        // public void SetDataForTest(string id, float time) { _itemId = id; _creationTimeSeconds = time;}

        // _itemA.SetDataForTest("coffee", 3.0f);
        // _itemB.SetDataForTest("tea", 3.0f);

        // (上記が難しい場合、テスト用のアセットを Unityエディター で作成し、
        //  AssetDataBase.LoadAssetPath<T> で読み込む方法もあります)

        // --- 2．リポジトリを初期化 ---
        // (ここでは簡略化のため、_mockMaster に直接 List をセットする前提)
        // _mockMaster.Items = new List<MenuItemData> { _itemA, _itemB };

        // _repository = new MasterDataRepository(_mockMaster);

        // 【重要】
        // SO の動的生成とセッターの準備が複雑なため、
        // このテストは「Unityエディター で MenuItemData.asset(全メニュー[コーヒー、
        // 紅茶、パフェ等]) と MenuItemMaster.asset(データベース) ファイルを作成した後」
        // に実行することを前提とし、Installer 経由で取得するロジックをテストします。
    }

    [Test]
    public void Test_RepositoryCanGetItemById()
    {
        // --- 1．準備 (Arrange) ---
        // Unity Editor で
        // "Assets/_Project/Core/Resources/MasterData/MenuItemMaster.asset"
        // が正しく設定されていることを前提とします。

        var master = Resources.Load<MenuItemMaster>("MasterData/MenuItemMaster");
        Assert.IsNotNull(master, "前提条件: MenuItemMaster.asset が Resources/MasterData/ に存在しません。");
        Assert.Greater(master.Items.Count, 0, "前提条件: MenuItemMaster にアイテムが1つも登録されていません。");

        var repository = new MasterDataRepository(master);

        // --- 2．実行 (Act) ---
        // GDD に存在するはずの "coffee" (ItemID) を取得してみる
        // (MenuItemData アセットの ItemID フィールドに "coffee" が設定されている必要があります。)
        string targetId = "coffee";
        var result = repository.GetMenuItemById(targetId);

        // --- 3．検証 (Assert) ---
        Assert.IsNotNull(result, $"検証失敗: ID '{targetId}' でアイテムを取得できませんでした。");
        Assert.AreEqual(targetId, result.ItemId, "検証失敗: 取得したアイテムの ID が異なります。");

        Debug.Log($"[Test Success] 検証成功: ID '{targetId}' で {result.DisplayName} を取得できました。");
    }
}
