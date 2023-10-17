using UnityEngine;
using UnityEngine.UI;

using Steamworks;

public class SteamProfileLoader : MonoBehaviour
{
    [SerializeField]
    new Text name = null;

    [Space, SerializeField]
    Text state = null;

    [Space, SerializeField]
    RawImage avatar = null;

    void Start()
    {
        name.text = $"{SteamFriends.GetPersonaName()}";
        state.text = $"{SteamFriends.GetPersonaState()}";

        avatar.texture = GetSteamAvatar(SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID()));
    }

    Texture2D GetSteamAvatar(int image)
    {
        Texture2D avatar = null;

        if(SteamUtils.GetImageSize(image, out uint width, out uint height))
        {
            byte[] pixels = new byte[width * height * 4];

            if(SteamUtils.GetImageRGBA(image, pixels, (int)(width * height * 4)))
            {
                avatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                avatar.LoadRawTextureData(pixels);
                avatar.Apply();
            }
        }

        return avatar;
    }
}
