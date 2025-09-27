using UnityEngine;
using UnityEngine.Pool;

public class ObjectManager : Singleton<ObjectManager>
{
    [SerializeField] private Pang pangPrefab;
    [SerializeField] private Transform pangParent;

    [SerializeField] private Block blockPrefab;
    [SerializeField] private Transform blockParent;

    public IObjectPool<Pang> PangPool { get; private set; }
    public IObjectPool<Block> BlockPool { get; private set; }

    private void Start()
    {
        PangPool = new ObjectPool<Pang>(CreatePang, OnGetPang, OnRelesePang, OnDestroyPang);
        BlockPool = new ObjectPool<Block>(CreateBlock, OnGetBlock, OnReleseBlock, OnDestroyPang);
    }

    #region Pang
    private Pang CreatePang() => Instantiate(pangPrefab, pangParent);

    private void OnGetPang(Pang handle)
    {
        handle.transform.SetParent(pangParent);
        handle.transform.position = new(99, 99);

        handle.gameObject.SetActive(true);
    }

    private void OnRelesePang(Pang handle)
    {
        if (this == null) return;

        handle.transform.SetParent(transform);
        handle.transform.position = new(99, 99);

        handle.particle.SetActive(false);
    }

    private void OnDestroyPang(Pang handle) => Destroy(handle.gameObject);
    #endregion

    #region Block
    private Block CreateBlock() => Instantiate(blockPrefab, blockParent);

    private void OnGetBlock(Block handle)
    {
        handle.transform.SetParent(blockParent);
        handle.transform.position = new(99, 99);

        handle.gameObject.SetActive(true);
    }

    private void OnReleseBlock(Block handle)
    {
        if (this == null) return;

        handle.transform.SetParent(transform);
        handle.transform.position = new(99, 99);

        handle.gameObject.SetActive(false);

        if (handle.TargetPang != null)
        {
            PangPool.Release(handle.TargetPang);

            handle.TargetPang = null;
        }
    }

    private void OnDestroyPang(Block handle) => Destroy(handle.gameObject);
    #endregion
}
