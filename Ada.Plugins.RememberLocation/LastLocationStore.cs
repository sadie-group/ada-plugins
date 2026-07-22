using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ada.Plugins.RememberLocation;

public record LastLocation(int RoomId, int X, int Y, int Direction);

public class LastLocationStore
{
    private readonly ConcurrentDictionary<long, LastLocation> _locations;
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private readonly string _filePath;
    private readonly ILogger<LastLocationStore> _logger;

    public LastLocationStore(IConfiguration config, ILogger<LastLocationStore> logger)
    {
        _logger = logger;

        _filePath = config.GetValue<string>("Plugins:RememberLocation:DataFile")
                    ?? Path.Combine(config.GetValue<string>("PluginDirectory") ?? ".", "remember-location.json");

        _locations = Load();
    }

    public bool TryGet(long playerId, out LastLocation? location)
    {
        return _locations.TryGetValue(playerId, out location);
    }

    public void Set(long playerId, LastLocation location)
    {
        _locations[playerId] = location;
    }

    public async Task SaveAsync()
    {
        await _saveLock.WaitAsync();

        try
        {
            var json = JsonSerializer.Serialize(_locations);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save last locations to {File}", _filePath);
        }
        finally
        {
            _saveLock.Release();
        }
    }

    private ConcurrentDictionary<long, LastLocation> Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<Dictionary<long, LastLocation>>(json);

                if (data != null)
                {
                    return new ConcurrentDictionary<long, LastLocation>(data);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load last locations from {File}", _filePath);
        }

        return new ConcurrentDictionary<long, LastLocation>();
    }
}
