using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class GlitchyPlayer : IEffect
{
    public string Name => "Glitchy Player";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "You feel glitchy";

    public void Use(PlayerAvatar roller)
    {
        Networker.Instance.photonView.RPC("makePlayerGlass", RpcTarget.All, roller.gameObject.GetComponent<PhotonView>().ViewID, roller.playerDeathHead.photonView.ViewID, false);
    }
}
public class FixPlayer : IEffect
{
    public string Name => "Glitchy Player Fix";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "You feel better";

    public void Use(PlayerAvatar roller)
    {
        Networker.Instance.photonView.RPC("makePlayerGlass", RpcTarget.All, roller.gameObject.GetComponent<PhotonView>().ViewID, roller.playerDeathHead.photonView.ViewID, true);
    }
}