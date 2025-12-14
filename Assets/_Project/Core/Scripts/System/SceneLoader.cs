using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SceneLoader : ISceneLoader
{
    private bool _isLoading = false;

    public async void LoadScene(string sceneName)
    {
        // ロード中は入力を受け付けない
        if (_isLoading) return;
        _isLoading = true;

        Debug.Log($"[SceneLoader] Loading Scene: {sceneName}");

        // 非同期でロード
        await SceneManager.LoadSceneAsync(sceneName);

        _isLoading = false;
    }
}
