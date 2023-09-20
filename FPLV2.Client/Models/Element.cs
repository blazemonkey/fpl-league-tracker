using FPLV2.Database.Models;
using System.Text.Json.Serialization;

namespace FPLV2.Client.Models;

public class Element
{
    [JsonPropertyName("chance_of_playing_next_round")]
    public int? ChanceOfPlayingNextRound { get; set; }

    [JsonPropertyName("chance_of_playing_this_round")]
    public int? ChanceOfPlayingThisRound { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("cost_change_event")]
    public int CostChangeEvent { get; set; }

    [JsonPropertyName("cost_change_event_fall")]
    public int CostChangeEventFall { get; set; }

    [JsonPropertyName("cost_change_start")]
    public int CostChangeStart { get; set; }

    [JsonPropertyName("cost_change_start_fall")]
    public int CostChangeStartFall { get; set; }

    [JsonPropertyName("dreamteam_count")]
    public int DreamteamCount { get; set; }

    [JsonPropertyName("element_type")]
    public int ElementType { get; set; }

    [JsonPropertyName("ep_next")]
    public string EpNext { get; set; }

    [JsonPropertyName("ep_this")]
    public string EpThis { get; set; }

    [JsonPropertyName("event_points")]
    public int EventPoints { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("form")]
    public string Form { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("in_dreamteam")]
    public bool InDreamteam { get; set; }

    [JsonPropertyName("news")]
    public string News { get; set; }

    [JsonPropertyName("news_added")]
    public DateTime? NewsAdded { get; set; }

    [JsonPropertyName("now_cost")]
    public int NowCost { get; set; }

    [JsonPropertyName("photo")]
    public string Photo { get; set; }

    [JsonPropertyName("points_per_game")]
    public string PointsPerGame { get; set; }

    [JsonPropertyName("second_name")]
    public string SecondName { get; set; }

    [JsonPropertyName("selected_by_percent")]
    public string SelectedByPercent { get; set; }

    [JsonPropertyName("special")]
    public bool Special { get; set; }

    [JsonPropertyName("squad_number")]
    public object SquadNumber { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("team")]
    public int Team { get; set; }

    [JsonPropertyName("team_code")]
    public int TeamCode { get; set; }

    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }

    [JsonPropertyName("transfers_in")]
    public int TransfersIn { get; set; }

    [JsonPropertyName("transfers_in_event")]
    public int TransfersInEvent { get; set; }

    [JsonPropertyName("transfers_out")]
    public int TransfersOut { get; set; }

    [JsonPropertyName("transfers_out_event")]
    public int TransfersOutEvent { get; set; }

    [JsonPropertyName("value_form")]
    public string ValueForm { get; set; }

    [JsonPropertyName("value_season")]
    public string ValueSeason { get; set; }

    [JsonPropertyName("web_name")]
    public string WebName { get; set; }

    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }

    [JsonPropertyName("goals_scored")]
    public int GoalsScored { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("clean_sheets")]
    public int CleanSheets { get; set; }

    [JsonPropertyName("goals_conceded")]
    public int GoalsConceded { get; set; }

    [JsonPropertyName("own_goals")]
    public int OwnGoals { get; set; }

    [JsonPropertyName("penalties_saved")]
    public int PenaltiesSaved { get; set; }

    [JsonPropertyName("penalties_missed")]
    public int PenaltiesMissed { get; set; }

    [JsonPropertyName("yellow_cards")]
    public int YellowCards { get; set; }

    [JsonPropertyName("red_cards")]
    public int RedCards { get; set; }

    [JsonPropertyName("saves")]
    public int Saves { get; set; }

    [JsonPropertyName("bonus")]
    public int Bonus { get; set; }

    [JsonPropertyName("bps")]
    public int Bps { get; set; }

    [JsonPropertyName("influence")]
    public string Influence { get; set; }

    [JsonPropertyName("creativity")]
    public string Creativity { get; set; }

    [JsonPropertyName("threat")]
    public string Threat { get; set; }

    [JsonPropertyName("ict_index")]
    public string IctIndex { get; set; }

    [JsonPropertyName("influence_rank")]
    public int InfluenceRank { get; set; }

    [JsonPropertyName("influence_rank_type")]
    public int InfluenceRankType { get; set; }

    [JsonPropertyName("creativity_rank")]
    public int CreativityRank { get; set; }

    [JsonPropertyName("creativity_rank_type")]
    public int CreativityRankType { get; set; }

    [JsonPropertyName("threat_rank")]
    public int ThreatRank { get; set; }

    [JsonPropertyName("threat_rank_type")]
    public int ThreatRankType { get; set; }

    [JsonPropertyName("ict_index_rank")]
    public int IctIndexRank { get; set; }

    [JsonPropertyName("ict_index_rank_type")]
    public int IctIndexRankType { get; set; }

    [JsonPropertyName("corners_and_indirect_freekicks_order")]
    public int? CornersAndIndirectFreekicksOrder { get; set; }

    [JsonPropertyName("corners_and_indirect_freekicks_text")]
    public string CornersAndIndirectFreekicksText { get; set; }

    [JsonPropertyName("direct_freekicks_order")]
    public int? DirectFreekicksOrder { get; set; }

    [JsonPropertyName("direct_freekicks_text")]
    public string DirectFreekicksText { get; set; }

    [JsonPropertyName("penalties_order")]
    public int? PenaltiesOrder { get; set; }

    [JsonPropertyName("penalties_text")]
    public string PenaltiesText { get; set; }

    public static implicit operator Database.Models.Element(Element element)
    {
        var newElement = new Database.Models.Element()
        {
            Code = element.Code,            
            FirstName = element.FirstName,
            SecondName = element.SecondName,
            WebName = element.WebName,
            ElementId = element.Id,
            ElementTeamId = element.Team,
            ElementType = (ElementType)element.ElementType
        };

        return newElement;
    }
}
