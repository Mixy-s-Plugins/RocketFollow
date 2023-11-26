using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RocketFollow
{
    public class Main : RocketPlugin<Config>
    {
        public static Main Instance { get; private set; }
        public Dictionary<CSteamID, Transform> PlayerTargetSelection { get; set; } =
            new Dictionary<CSteamID, Transform>();

        protected override void Load()
        {
            UseableGun.onProjectileSpawned += ProjectileSpawned;
            Instance = this;
        }

        protected override void Unload()
        {
            UseableGun.onProjectileSpawned -= ProjectileSpawned;
        }

        private void ProjectileSpawned(UseableGun sender, GameObject projectile)
        {
            if (sender.equippedGunAsset.id != 519) return;
            var player = UnturnedPlayer.FromPlayer(sender.player);
            if (PlayerTargetSelection.TryGetValue(player.CSteamID, out var target))
            {
                projectile.AddComponent<MyRocket>().Fire(target, Configuration.Instance.Speed, Configuration.Instance.RotationSpeed, Configuration.Instance.FollowDistance);
            }
        }

        private void Update()
        {
            foreach (var player in Provider.clients.Select(p => p.ToUnturnedPlayer()))
            {
                if (player.Player.input.keys[6])
                {
                    if (!Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out var hit, 10000f,
                            RayMasks.VEHICLE))
                    {
                        UnturnedChat.Say(player, "Hedef bulunamadı.");
                        return;
                    }
                    
                    if (PlayerTargetSelection.ContainsKey(player.CSteamID))
                    {
                        PlayerTargetSelection[player.CSteamID] = hit.transform;
                    }
                    else
                    {
                        PlayerTargetSelection.Add(player.CSteamID, hit.transform);
                    }
                    UnturnedChat.Say(player, "Hedef belirlendi.");
                }
            }
        }

        [RocketCommand("settarget", "")]
        public void TargetCommand(IRocketPlayer caller)
        {
            var player = caller as UnturnedPlayer;

            if (player.CurrentVehicle == null)
            {
                UnturnedChat.Say(player, "Araç bulunamadı.");
                return;
            }

            if (PlayerTargetSelection.ContainsKey(player.CSteamID))
            {
                PlayerTargetSelection[player.CSteamID] = player.CurrentVehicle.transform;
            }
            else
            {
                PlayerTargetSelection.Add(player.CSteamID, player.CurrentVehicle.transform);
            }
            
            UnturnedChat.Say(player, "Araç belirlendi.");
        }
    }
}