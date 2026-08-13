using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Terraria;
using Utils;

namespace DeterministicDrops.DropSystem;

internal sealed class DropStateStore(string savePath)
{
    private readonly string _savePath = savePath;

    private readonly Dictionary<string, DropState> _dropStates = [];

    public DropState GetState(short[][] itemIds)
    {
        string dropId = SerializeDropId(itemIds);

        if (!_dropStates.TryGetValue(dropId, out DropState state))
        {
            state = new DropState();
            _dropStates[dropId] = state;
        }

        return state;
    }

    public void Load()
    {
        string fileName = FileUtils.SanitizeFilename(Main.worldName);
        string path = Path.Combine(_savePath, "worlds", $"{fileName}.json");

        if (!File.Exists(path))
        {
            Mod.Instance.Log.Info($"No save file found for world: {fileName}");
            return;
        }

        string json = File.ReadAllText(path);
        var dropStates = JsonConvert.DeserializeObject<Dictionary<string, DropState>>(json);

        if (dropStates is null)
            return;

        _dropStates.Clear();

        foreach (var pair in dropStates)
        {
            _dropStates.Add(pair.Key, pair.Value);
        }

        Mod.Instance.Log.Info($"Loaded drop states for world: {fileName}");
    }

    public void Save()
    {
        string fileName = FileUtils.SanitizeFilename(Main.worldName);
        string directory = Path.Combine(_savePath, "worlds");

        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, $"{fileName}.json");

        string json = JsonConvert.SerializeObject(_dropStates, Formatting.Indented);

        File.WriteAllText(path, json);

        Mod.Instance.Log.Info($"Saved drop states for world: {fileName}");
    }

    private static string SerializeDropId(short[][] itemIds) => string.Join(";", itemIds.Select(id => string.Join(",", id)));
}

[method: JsonConstructor]
internal sealed class DropState(double dropProgress = 0, int nextDropCycle = 0)
{
    public double DropProgress { get; private set; } = dropProgress;
    public int NextDropCycle { get; private set; } = nextDropCycle;

    public void AddProgress(double progress) => DropProgress += progress;

    public void AdvanceCycle() => NextDropCycle++;
}
