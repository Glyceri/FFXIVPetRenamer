using KamiToolKit.Nodes;
using KamiToolKit.Classes;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes;

internal class VerminionImageNode : SimpleImageNode
{
    private uint _minionRace = 1;

    public VerminionImageNode()
    {
        WrapMode    = WrapMode.Stretch;
        IsVisible   = true;
        TexturePath = "ui/uld/iconVerminion.tex";
        TextureSize = new Vector2(24, 24);
        MinionRace  = 1;
    }

    public uint MinionRace
    {
        get => _minionRace;

        set
        {
            if (_minionRace == value)
            {
                return;
            }

            _minionRace = value;
            TextureSize = new Vector2(24, 24);

            switch (_minionRace)
            {
                case 1:  TextureCoordinates  = new Vector2(0, 0);   break;
                case 2:  TextureCoordinates  = new Vector2(72, 0);  break;
                case 3:  TextureCoordinates  = new Vector2(48, 0);  break;
                case 4:  TextureCoordinates  = new Vector2(24, 0);  break;
                default: TextureSize         = new Vector2(1, 1);   break;
            }
        }
    }
}
