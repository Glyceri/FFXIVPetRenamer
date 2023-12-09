using HtmlAgilityPack;
using PetRenamer.Core.Networking.Attributes;
using PetRenamer.Core.Singleton;
using System;

namespace PetRenamer.Core.Networking.NetworkingElements.Lodestone.LodestoneElement;

[Networked]
public class LodestoneCharacterSearchElement : LodestoneNetworkedBase, ISingletonBase<LodestoneCharacterSearchElement>
{
    public static LodestoneCharacterSearchElement instance { get; set; } = null!;

    public void SearchCharacter((string, uint) character, Action<SearchData> callbackSucces = null!, Action<Exception> callbackError = null!)
    {
        (string, string) chara = NetworkedImageDownloader.instance.RemapCharacterData(ref character);
        string URL = $"https://na.finalfantasyxiv.com/lodestone/character/?q={chara.Item1.Replace(" ", "+")}&worldname={chara.Item2}";
        GetDocument(URL, (document) => OnDocument(document, callbackSucces, callbackError), (exception) => callbackError?.Invoke(exception));
    }

    void OnDocument(HtmlDocument document, Action<SearchData> callbackSucces = null!, Action<Exception> callbackError = null!)
    {
        if (document == null)
        {
            callbackError?.Invoke(new Exception("Document is Null"));
            return;
        }

        HtmlNode rootNode = document.DocumentNode;

        try
        {
            HtmlNode? listNode = GetNode(rootNode, "ldst__window");
            if (listNode == null)
            {
                callbackError?.Invoke(new Exception("List Node is not found in HTML document."));
                return;
            }
            HtmlNode? entryNode = GetNode(listNode, "entry");
            if (entryNode == null)
            {
                callbackError?.Invoke(new Exception("Entry Node is not found in HTML document."));
                return;
            }
            SearchData data;
            try
            {
                data = new SearchData(entryNode);
            }
            catch (Exception e)
            {
                callbackError?.Invoke(new Exception("Search Data unable to be made: " + e.Message));
                return;
            }
            callbackSucces?.Invoke(data);
        }
        catch (Exception e)
        {
            callbackError?.Invoke(e);
            return;
        }
    }
}

public struct SearchData
{
    public string? lodestoneID { get; private set; }
    public string? imageURL { get; private set; }
    public string? name { get; private set; }
    public string? homeworld { get; private set; }

    public new string ToString() => $"Name: {name}\nHome World: {homeworld}\nLodestone ID: {lodestoneID}\nImage URL: {imageURL}";

    public SearchData() 
    { 
        lodestoneID = string.Empty;
        imageURL = string.Empty;
        name = string.Empty;
        homeworld = string.Empty;
    }

    public SearchData(string lodestoneID, string imageURL, string name, string homeworld)
    {
        this.name = name;
        this.homeworld = homeworld;
        this.lodestoneID = lodestoneID;
        this.imageURL = imageURL;
    }

    public SearchData(HtmlNode baseNode) => Parse(baseNode);

    void Parse(HtmlNode baseNode)
    {
        foreach (HtmlNode node in baseNode.ChildNodes)
        {
            if (node.HasClass("entry__link")) lodestoneID = node.GetAttributeValue("href", "");
            if (node.HasClass("entry__chara__face"))
            {
                if (node.ChildNodes.Count != 0)
                {
                    HtmlNode img = node.ChildNodes[0];
                    if (img != null) imageURL = img.GetAttributeValue("src", "");
                }
            }
            if (node.HasClass("entry__name")) name = node.InnerText;
            if (node.HasClass("entry__world")) homeworld = node.InnerText;
            Parse(node);
        }
    }
}
