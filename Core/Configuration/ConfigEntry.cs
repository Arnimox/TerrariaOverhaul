﻿using System;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerrariaOverhaul.Core.Configuration;

public class ConfigEntry<T> : IConfigEntry
{
	private static Regex? defaultDisplayNameRegex;

	private static Regex DefaultDisplayNameRegex => defaultDisplayNameRegex ??= new(@"([A-Z][a-z]+)(?=[A-Z])", RegexOptions.Compiled);

	private readonly Func<T> defaultValueGetter;

	private T? localValue;
	private T? remoteValue;

	public string Name { get; }
	public string Category { get; }
	public ConfigSide Side { get; }
	public bool IsHidden { get; set; }
	//public bool RequiresRestart { get; set; }
	public string[] ExtraCategories { get; set; } = Array.Empty<string>();
	public LocalizedText? DisplayName { get; internal set; }
	public LocalizedText? Description { get; internal set; }
	public Mod? Mod { get; private set; }

	public Type ValueType => typeof(T);
	public T DefaultValue => defaultValueGetter();

	public T? LocalValue {
		get => ModifyGetValue(localValue);
		set => localValue = ModifySetValue(value);
	}
	public T? RemoteValue {
		get => ModifyGetValue(remoteValue);
		set => remoteValue = ModifySetValue(value);
	}

	public T? Value {
		get {
			if (Side == ConfigSide.Both && Main.netMode == NetmodeID.MultiplayerClient) {
				return RemoteValue;
			}

			return LocalValue;
		}
		set {
			if (Side == ConfigSide.Both && Main.netMode == NetmodeID.MultiplayerClient) {
				RemoteValue = value;
			} else {
				LocalValue = value;
			}
		}
	}

	object? IConfigEntry.Value {
		get => Value;
		set => Value = (T?)value;
	}
	object? IConfigEntry.LocalValue {
		get => LocalValue;
		set => LocalValue = (T?)value;
	}
	object? IConfigEntry.RemoteValue {
		get => RemoteValue;
		set => RemoteValue = (T?)value;
	}
	object IConfigEntry.DefaultValue => DefaultValue!;

	public ConfigEntry(ConfigSide side, string category, string name, Func<T> defaultValueGetter)
	{
		Name = name;
		Category = category;
		Side = side;
		this.defaultValueGetter = defaultValueGetter;
		RemoteValue = DefaultValue;
		LocalValue = DefaultValue;
	}

	protected virtual T? ModifyGetValue(T? value) => value;

	protected virtual T? ModifySetValue(T? value) => value;

	public void Initialize(Mod mod)
	{
		Mod = mod;
		DisplayName = Language.GetOrRegister(
			$"Mods.{Mod.Name}.Configuration.{Category}.{Name}.DisplayName",
			() => DefaultDisplayNameRegex.Replace(Name, "$1 ")
		);
		Description = Language.GetOrRegister(
			$"Mods.{Mod.Name}.Configuration.{Category}.{Name}.Description",
			() => string.Empty
		);
	}

	public static implicit operator T?(ConfigEntry<T> configEntry) => configEntry.Value;
}
