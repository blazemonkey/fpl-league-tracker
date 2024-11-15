﻿using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class Standings
{
    public League League { get; set; }
    [JsonPropertyName("standings")]
    public PlayerResults Results { get; set; }
}
