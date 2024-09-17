using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public Pang pangPrefab;
    public Transform pangParent;

    public Block blockPrefab;
    public Transform blockParent;

    public ObjectPool<Pang> pangs;
    public ObjectPool<Block> blocks;

    private void Start()
    {
        pangs = new(pangPrefab, pangParent);
        blocks = new(blockPrefab, blockParent);

        pangs.dequeueFunc = DequeuePang;
        pangs.enqueueFunc = EnqueuePang;

        blocks.dequeueFunc = DequeueBlock;
        blocks.enqueueFunc = EnqueueBlock;
    }

    private void DequeuePang(Pang _pang)
    {
        _pang.transform.position = new(99, 99);
    }

    private void EnqueuePang(Pang _pang)
    {
        _pang.transform.position = new(99, 99);
        _pang.TargetBlock = null;
    }

    private void DequeueBlock(Block _block)
    {
        _block.transform.position = new(99, 99);
    }

    private void EnqueueBlock(Block _block)
    {
        _block.transform.position = new(99, 99);
    }
}
