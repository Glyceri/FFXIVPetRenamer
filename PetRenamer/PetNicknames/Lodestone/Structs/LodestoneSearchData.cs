using HtmlAgilityPack;

namespace PetRenamer.PetNicknames.Lodestone.Structs;

public struct LodestoneSearchData
{
    public string? LodestoneID { get; private set; }
    public string? ImageURL    { get; private set; }
    public string? Name        { get; private set; }
    public string? Homeworld   { get; private set; }

    public LodestoneSearchData(HtmlNode baseNode)
    {
        Parse(baseNode);
    }

    private void Parse(HtmlNode baseNode)
    {
        foreach (HtmlNode node in baseNode.ChildNodes)
        {
            if (node.HasClass("entry__link"))
            {
                LodestoneID = node.GetAttributeValue("href", string.Empty);
            }

            if (node.HasClass("entry__chara__face"))
            {
                if (node.ChildNodes.Count != 0)
                {
                    HtmlNode img = node.ChildNodes[0];

                    if (img != null)
                    {
                        ImageURL = img.GetAttributeValue("src", string.Empty);
                    }
                }
            }

            if (node.HasClass("entry__name"))
            {
                Name = node.InnerText;
            }

            if (node.HasClass("entry__world"))
            {
                Homeworld = node.InnerText;
            }

            Parse(node);
        }
    }

    public override string ToString()
        => $"Name: {Name}\nHome World: {Homeworld}\nLodestone ID: {LodestoneID}\nImage URL: {ImageURL}";
}
