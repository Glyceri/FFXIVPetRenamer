using Dalamud.Interface.Internal;
using Dalamud.Utility;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
internal class PetNewsWindow : PetWindow
{
    Vector2 baseSize = new Vector2(600, 400);

    PetNewsPost[] petNewsPosts = new PetNewsPost[]
    {
        new PetNewsPost("11-02-2024", "Feature Poll", "Hello Pettable Users! Over the past year I've added a lot of features.\nIn order to see what features are used and liked I'd like you lot to fill out a poll.", "Glyceri", "2442", "https://forms.gle/RRVw664hB7Jroox1A"),
        new PetNewsPost("10-02-2024", "Hiding Toolbar Buttons!", "Did you know you can now hide buttons in the toolbar?\nIt is very simple. Open the settings page and go to UI Settings.", "Glyceri", "2442", ""),
    };

    List<PetNewsPost> sortedPetNews = new List<PetNewsPost>();
    List<IDalamudTextureWrap> textures = new List<IDalamudTextureWrap>();

    public PetNewsWindow() : base("Pet News")
    {
        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = baseSize,
            MaximumSize = new Vector2(9999, 9999)
        };

        SortPetNews();
        CreateImagesForEach();
    }

    protected override void OnDispose()
    {
        foreach (IDalamudTextureWrap texture in textures)
            texture?.Dispose();
    }

    void SortPetNews()
    {
        foreach (PetNewsPost item in petNewsPosts)
        {
            sortedPetNews.Add(item);
        }

        sortedPetNews.Sort((p1, p2) => DateTime.Parse(p1.Date, CultureInfo.InvariantCulture).CompareTo(DateTime.Parse(p2.Date, CultureInfo.InvariantCulture)));
        sortedPetNews.Reverse();
    }

    void CreateImagesForEach()
    {
        int i = 0;
        foreach (PetNewsPost item in petNewsPosts)
        {
            if (short.TryParse(item.Image, out short result))
            {
                try
                {
                    string iconPath = RemapUtils.instance.GetTextureID(result).GetIconPath();
                    textures.Add(PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!);
                }
                catch (Exception ex) { textures.Add(null!); }
            }
            else
            {
                textures.Add(null!);
                try
                {
                    int current = i;
                    string path = NetworkedImageDownloader.instance.MakeTexturePath(($"PetNewsPost{i}", "NETWORKED"));
                    NetworkedImageDownloader.instance.AsyncDownload(item.Image, path,
                        () =>
                        {
                            lock (textures)
                            {
                                textures[current] = PluginHandlers.TextureProvider.GetTextureFromFile(new System.IO.FileInfo(path))!;
                            }
                        }, 
                        (e) => { });
                }
                catch (Exception ex) { PetLog.Log(ex); };
            }
            i++;
        }
    }

    public override void OnDraw()
    {
        if (BeginListBox($"##{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded * 2)))
        {
            Label("Welcome to the Pet News panel!", new Vector2(ContentAvailableX, BarSize));
            Label("This is where I post fun to know things, polls and community stuff.", new Vector2(ContentAvailableX, BarSize ));
            ImGui.EndListBox();
        }

        if (BeginListBox($"##{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded)))
        {
            HidePetNews();
            ImGui.EndListBox();
        }

        if (BeginListBox($"##{internalCounter++}", new Vector2(ContentAvailableX, ContentAvailableY)))
        {
            int i = 0;
            foreach (PetNewsPost item in sortedPetNews)
            {
                DrawPetNewsPost(item, i++);
            }

            ImGui.EndListBox();
        }
    }

    void HidePetNews()
    {
        Checkbox("Hide the Pet News button in the Toolbar.", ref PluginLink.Configuration.hideNewsButton);
    }

    void DrawPetNewsPost(PetNewsPost post, int i)
    {
        if (!BeginListBoxAutomatic($"##{internalCounter++}", new Vector2(ContentAvailableX, 131), true)) return;
        if (BeginListBoxAutomatic($"##{internalCounter++}", new Vector2(90, 90), false))
        {
            IDalamudTextureWrap tWrap = textures[i];
            if (tWrap != null)
            {
                DrawTexture(textures[i].ImGuiHandle);
            }
            ImGui.EndListBox();
            SameLine();
            if (BeginListBoxAutomatic($"##{internalCounter++}", new Vector2(ContentAvailableX, 90), true))
            {
                Label(post.Title, new Vector2(ContentAvailableX, BarSize));
                Label(post.Description, new Vector2(ContentAvailableX, BarSize * 2));
                ImGui.EndListBox();
            }
        }

        if (BeginListBoxAutomatic($"##{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded), true))
        {
            Label(post.Date, new Vector2(90, BarSize));
            if (post.ReadMore != string.Empty)
            {
                SameLine();
                if (Button("Read More", new Vector2(ContentAvailableX, BarSize)))
                {
                    Util.OpenLink(post.ReadMore);
                }
            }
            ImGui.EndListBox();
        }
        ImGui.EndListBox();
    }
}

public struct PetNewsPost
{
    public string Date { get; private set; } = "";
    public string Title { get; private set; } = "";
    public string Description { get; private set; } = "";
    public string Author { get; private set; } = "";
    public string Image { get; private set; } = "";
    public string ReadMore { get; private set; } = "";

    public PetNewsPost(string Date, string Title, string Description, string Author, string Image, string ReadMore)
    {
        this.Date = Date;
        this.Title = Title;
        this.Description = Description;
        this.Author = Author;
        this.Image = Image;
        this.ReadMore = ReadMore;
    }
}
