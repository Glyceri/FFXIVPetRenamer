using Dalamud.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Data;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetShareWindow;

internal class PetSharingWindow : PetWindow
{
    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;

    protected override string ID { get; } = "PetSharing";
    protected override Vector2 MinSize { get; } = new Vector2(300, 100);
    protected override Vector2 MaxSize { get; } = new Vector2(300, 100);
    protected override Vector2 DefaultSize { get; } = new Vector2(300, 100);
    protected override bool HasModeToggle { get; } = false;
    protected override string Title { get; } = Translator.GetLine("PetList.Sharing");

    readonly QuickButton ExportButton;
    readonly QuickButton ImportButton;

    public PetSharingWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IDataParser dataParser, in IDataWriter dataWriter) : base(windowHandler, dalamudServices, configuration, "PetSharing")
    {
        IsOpen = true;

        DataParser = dataParser;
        DataWriter = dataWriter;

        ContentNode.Style = new Style()
        {
            Flow = Flow.Vertical,
        };

        ContentNode.ChildNodes =
                        [
                            ExportButton = new QuickButton(in DalamudServices, Translator.GetLine("Export to Clipboard"))
                            {
                                Style = new Style()
                                {
                                    Size = new Size(120, 15),
                                    Anchor = Anchor.TopCenter,
                                    Margin = new EdgeSize(12, 1, 1, 1),
                                },
                            },
                            ImportButton = new QuickButton(in DalamudServices, Translator.GetLine("Import from Clipboard"))
                            {
                                Style = new Style()
                                {
                                    Size = new Size(120, 15),
                                    Anchor = Anchor.TopCenter,
                                    Margin = new EdgeSize(1),
                                },
                            },
                        ];

        ExportButton.Clicked += () => DalamudServices.Framework.Run(() =>
        {
            string data = DataWriter.WriteData();
            if (data.IsNullOrWhitespace())
            {
                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Warning,
                    Content = "No Data Available.\nYou need to log in to a character to export your data."
                });
            }
            else
            {
                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Success,
                    Content = "Data succesfully copied"
                });

                ImGui.SetClipboardText(data);
            }
        });

        ImportButton.Clicked += () => DalamudServices.Framework.Run(() =>
        {
            IDataParseResult parseResult = DataParser.ParseData(ImGui.GetClipboardText());

            if (!DataParser.ApplyParseData(parseResult, false))
            {
                string error = string.Empty;
                if (parseResult is InvalidParseResult invalidParseResult) error = invalidParseResult.Reason;

                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Warning,
                    Content = $"Failed to import data: {error}"
                });
            }
            else
            {
                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Success,
                    Content = "Successfully imported data"
                });
            }
        });
    }

    public override void OnDraw()
    {

    }
}
