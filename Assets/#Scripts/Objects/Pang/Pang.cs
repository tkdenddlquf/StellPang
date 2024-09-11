using UnityEngine;

public class Pang : MonoBehaviour
{
    public SpriteRenderer pangImage;
    public SpriteRenderer glow;

    private PangStateBase stateBase;

    public void SetType(PangType _type)
    {
        switch (_type)
        {
            case PangType.GangGi:
                stateBase = new PangState_GangGi(this, pangImage.sprite);
                break;

            case PangType.Kanna:
                stateBase = new PangState_Kanna(this, pangImage.sprite);
                break;

            case PangType.Yuni:
                stateBase = new PangState_Yuni(this, pangImage.sprite);
                break;

            case PangType.Hina:
                stateBase = new PangState_Hina(this, pangImage.sprite);
                break;

            case PangType.Shiro:
                stateBase = new PangState_Shiro(this, pangImage.sprite);
                break;

            case PangType.Lize:
                stateBase = new PangState_Lize(this, pangImage.sprite);
                break;

            case PangType.Tabi:
                stateBase = new PangState_Tabi(this, pangImage.sprite);
                break;

            case PangType.Buki:
                stateBase = new PangState_Buki(this, pangImage.sprite);
                break;

            case PangType.Rin:
                stateBase = new PangState_Rin(this, pangImage.sprite);
                break;

            case PangType.Nana:
                stateBase = new PangState_Nana(this, pangImage.sprite);
                break;

            case PangType.Riko:
                stateBase = new PangState_Riko(this, pangImage.sprite);
                break;
        }
    }
}

public enum PangType
{
    GangGi,
    Kanna,
    Yuni,
    Hina,
    Shiro,
    Lize,
    Tabi,
    Buki,
    Rin,
    Nana,
    Riko
}
