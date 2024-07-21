using System;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class ColourProfileConfig : ToggleConfig
{
    public ColourProfileConfig(in Configuration configuration, string label, string author, int index, bool active, Action<int> callback) : base(configuration, label, active, (value) => callback?.Invoke(index))
    {



    }


    public void SetValue(bool value)
    {

    }
}
