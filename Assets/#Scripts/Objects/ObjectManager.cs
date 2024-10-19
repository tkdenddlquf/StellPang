using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    public Pang pangPrefab;
    public Transform pangParent;

    public Block blockPrefab;
    public Transform blockParent;

    public ObjectPool<Pang> pangs;
    public ObjectPool<Block> blocks;

    private void Start()
    {
        pangs = new(pangPrefab, pangParent)
        {
            DeAction = DequeuePang,
            EnAction = EnqueuePang
        };

        blocks = new(blockPrefab, blockParent)
        {
            DeAction = DequeueBlock,
            EnAction = EnqueueBlock
        };
    }

    private void DequeuePang(Pang _pang)
    {
        _pang.gameObject.SetActive(true);

        _pang.transform.position = new(99, 99);
    }

    private void EnqueuePang(Pang _pang)
    {
        _pang.gameObject.SetActive(false);

        _pang.transform.position = new(99, 99);
        _pang.TargetBlock = null;
    }

    private void DequeueBlock(Block _block)
    {
        _block.gameObject.SetActive(true);

        _block.transform.position = new(99, 99);
    }

    private void EnqueueBlock(Block _block)
    {
        _block.gameObject.SetActive(false);

        _block.transform.position = new(99, 99);
    }
}
