using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全てのアニマルデータを保持するデータベース
/// </summary>
[CreateAssetMenu(fileName = "AnimalMaster", menuName = "KONCafe/Master/Animal Master")]
public class AnimalMaster : ScriptableObject
{
    [SerializeField] private List<AnimalData> _animals;
    public IReadOnlyList<AnimalData> Animals => _animals;
}
