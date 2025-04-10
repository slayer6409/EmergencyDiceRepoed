using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class RandUpgradeMulti : IEffect
{
    public string Name => "Random Upgrade Multi";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Everyone Gets an Upgrade!";

    public void Use(PlayerAvatar roller)
    {
        foreach (var player in GameDirector.instance.PlayerList)
        {
            Networker.Instance.photonView.RPC("addUpgradeRandomRPC", RpcTarget.Others,player.photonView.ViewID);
            Networker.Instance.addUpgradeRandomRPC(PlayerAvatar.instance.photonView.ViewID);
        }
    }

}