using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaOverhaul.Common.ConfigurationScreen;
using TerrariaOverhaul.Core.Localization;

namespace TerrariaOverhaul.Common.MainMenuOverlays;

public class ConfigurationMenuButton : MenuButton
{
	public ConfigurationMenuButton(Text text) : base(text) { }

	protected override void OnClicked()
	{
		if (typeof(ModLoader).GetField("isLoading", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) is true) {
			return;
		}

		SoundEngine.PlaySound(SoundID.MenuOpen);
		Main.MenuUI.SetState(ConfigurationState.Instance);
		Main.menuMode = 888;
	}
}
