using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json.Linq;

namespace Parrot.client.Classes
{

    public class OwnerList : MonoBehaviour
    {

        public const string Url = "https://raw.githubusercontent.com/Scrypto34/Parrot.Client-Admins/main/data.json";

        private static readonly Dictionary<string, string> owners = new Dictionary<string, string>();

        public static bool Loaded { get; private set; }

        private static OwnerList instance;
        private static bool loading;

        private static OwnerList Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject holder = new GameObject("ParrotOwnerList");
                    DontDestroyOnLoad(holder);
                    holder.hideFlags = HideFlags.HideAndDontSave;
                    instance = holder.AddComponent<OwnerList>();
                }

                return instance;
            }
        }

        public static void EnsureLoaded()
        {
            if (Loaded || loading || Instance == null)
                return;

            loading = true;
            instance.StartCoroutine(Fetch());
        }

        private static IEnumerator Fetch()
        {
            using UnityWebRequest request = UnityWebRequest.Get(Url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    owners.Clear();

                    foreach (JToken entry in JArray.Parse(request.downloadHandler.text))
                    {
                        string id = (string)entry["user-id"];
                        if (!string.IsNullOrEmpty(id))
                            owners[id] = (string)entry["name"] ?? "Owner";
                    }

                    Loaded = true;
                }
                catch (Exception exc)
                {
                    Debug.LogWarning($"{PluginInfo.Name} // Could not read the owner list: {exc.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"{PluginInfo.Name} // Could not load the owner list: {request.error}");
            }

            loading = false;
        }

        public static bool HasAccess() =>
            TryGetName(out _);

        public static bool TryGetName(out string name)
        {
            name = null;
            string id = PhotonNetwork.LocalPlayer?.UserId;
            return !string.IsNullOrEmpty(id) && owners.TryGetValue(id, out name);
        }

        public static bool IsOwner(string userId) =>
            !string.IsNullOrEmpty(userId) && owners.ContainsKey(userId);

        public static bool IsOwnerActor(int actorNumber)
        {
            try
            {
                Photon.Realtime.Player p = PhotonNetwork.CurrentRoom?.GetPlayer(actorNumber);
                return p != null && IsOwner(p.UserId);
            }
            catch { return false; }
        }
    }
}
