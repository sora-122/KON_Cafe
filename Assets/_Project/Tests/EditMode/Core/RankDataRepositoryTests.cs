using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// RankDataRepository のロジックを検証する EditMode テスト
/// </summary>

public class RankDataRepositoryTests
{
    // テスト対象のリポジトリ
    private IRankDataRepository _repository;

    // テストで使用するロード済みテーブル
    private RankExperienceTable _loadedTable;

    /// <summary>
    /// 各テストの実行前に呼ばれるセットアップ
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // --- 1．準備 (Arrange) ---
        // このテストは「Unity Editor でアセットが正しく作られていること」を前提として
        // Resources.Load を使用します。

        // (前提: _Project/Core/Resources/MasterData/RankExperienceTable.asset が作成済み)
        _loadedTable = Resources.Load<RankExperienceTable>("MasterData/RankExperienceTable");

        // --- 検証 (Assert) ---
        // テストの前提条件が満たされているかを確認
        Assert.IsNotNull(_loadedTable, "前提条件: RankExperienceTable.asset が Resources/MasterData/ に存在しません。");
        Assert.IsNotNull(_loadedTable.RankDataList, "前提条件: RankDataList が null です。");
        Assert.Greater(_loadedTable.RankDataList.Count, 1, "前提条件: RankExperienceTable.asset に最低2ランク分 (Rank 1, Rank 2) のデータがありません。");

        // --- 2．テスト対象のクラスを初期化 ---
        _repository = new RankDataRepository(_loadedTable);
    }

    /// <summary>
    /// 指定したランクのデータを正しく取得できるか
    /// </summary>
    [Test]
    public void GetRankData_ReturnsCorrectData_ForRank2()
    {
        // --- 1．準備 (Arrange) ---
        // (前提: _loadedTable に Rank 2 のデータが正しく入力されている)
        // SetUp でロードしたアセットから、Rank 2 の期待値を取得
        RankData expectedData = _loadedTable.RankDataList.First(data => data.Rank == 2);

        // --- 2．実行 (Act) ---
        RankData actualData = _repository.GetRankData(2);

        // --- 3．検証 (Assert) ---
        Assert.AreEqual(expectedData.Rank, actualData.Rank, "取得したランクが 2 ではありません。");
        Assert.AreEqual(expectedData.TotalExpToNextRank, actualData.TotalExpToNextRank, "Rank 2 の必要経験値が異なります。");
        Assert.AreEqual(expectedData.VisitorCountBonus, actualData.VisitorCountBonus, "Rank 2 の来客数ボーナスが異なります。");

        Debug.Log($"[Test Success] GetRankData(2) の検証に成功しました。");
    }

    /// <summary>
    /// 最大ランクを正しく取得できるか
    /// </summary>
    [Test]
    public void GetMaxRank_ReturnsCorrectMaxRank()
    {
        // --- 1．準備 (Arrange) ---
        // SetUp でロードしたアセットから、期待される最大ランクを取得
        int expectedMaxRank = _loadedTable.RankDataList.Max(data => data.Rank);

        // --- 2．実行 (Act) ---
        int actualMaxRank = _repository.GetMaxRank();

        // --- 3．検証 (Assert) ---
        Assert.AreEqual(expectedMaxRank, actualMaxRank, "取得した最大ランクがアセットの定義と異なります。");

        Debug.Log($"[Test Success] GetMaxRank() の検証に成功しました。(MaxRank {actualMaxRank})");
    }
}