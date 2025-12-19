using HtmlAgilityPack;
using System;

namespace PetRenamer.PetNicknames.Lodestone.Structs;

public struct LodestoneSearchData
{
    public string? lodestoneID  { get; private set; }
    public string? imageURL     { get; private set; }
    public string? name         { get; private set; }
    public string? homeworld    { get; private set; }

    public LodestoneSearchData()
    {
        lodestoneID = string.Empty;
        imageURL    = string.Empty;
        name        = string.Empty;
        homeworld   = string.Empty;
    }

    public LodestoneSearchData(string lodestoneID, string imageURL, string name, string homeworld)
    {
        this.name        = name;
        this.homeworld   = homeworld;
        this.lodestoneID = lodestoneID;
        this.imageURL    = imageURL;
    }

    public LodestoneSearchData(HtmlNode baseNode) 
        => Parse(baseNode);

    private void Parse(HtmlNode baseNode)
    {
        foreach (HtmlNode node in baseNode.ChildNodes)
        {
            if (node.HasClass("entry__link"))
            {
                lodestoneID = node.GetAttributeValue("href", "");
            }

            if (node.HasClass("entry__chara__face"))
            {
                if (node.ChildNodes.Count != 0)
                {
                    HtmlNode img = node.ChildNodes[0];

                    if (img != null)
                    {
                        imageURL = img.GetAttributeValue("src", "");
                    }
                }
            }

            if (node.HasClass("entry__name"))
            {
                name = node.InnerText;
            }

            if (node.HasClass("entry__world"))
            {
                homeworld = node.InnerText;
            }

            Parse(node);
        }
    }

    public new string ToString()
        => $"Name: {name}{Environment.NewLine}Home World: {homeworld}{Environment.NewLine}Lodestone ID: {lodestoneID}{Environment.NewLine}Image URL: {imageURL}";
}
