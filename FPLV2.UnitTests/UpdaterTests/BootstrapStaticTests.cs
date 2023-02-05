using FPLV2.Updater.Models;
using System.Text.Json;

namespace FPLV2.UnitTests.UpdaterTests;

[TestClass]
public class BootstrapStaticTests : UpdaterTests
{
    #region Season Jsons
    public const string Season2022Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 2,
              "name": "Gameweek 2",
              "deadline_time": "2022-08-13T10:00:00Z",
              "is_current": false
            },
            {
              "id": 3,
              "name": "Gameweek 3",
              "deadline_time": "2022-08-20T10:00:00Z",
              "is_current": false
            },
            {
              "id": 4,
              "name": "Gameweek 4",
              "deadline_time": "2022-08-27T10:00:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }]
        }
        """;
    public const string Season2023Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2023-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 2,
              "name": "Gameweek 2",
              "deadline_time": "2023-08-13T10:00:00Z",
              "is_current": false
            },
            {
              "id": 3,
              "name": "Gameweek 3",
              "deadline_time": "2023-08-20T10:00:00Z",
              "is_current": false
            },
            {
              "id": 4,
              "name": "Gameweek 4",
              "deadline_time": "2023-08-27T10:00:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2024-05-28T13:30:00Z",
              "is_current": false
            }]
        }
        """;
    #endregion

    #region Team Jsons
    public const string Team2022Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 7,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
    public const string Team2022v2Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 14,
                  "id": 12,
                  "name": "Liverpool",
                  "short_name": "LIV"
                },
                {
                  "code": 43,
                  "id": 13,
                  "name": "Man City",
                  "short_name": "MCI"
                }
            ]
        }
        """;
    public const string Team2023Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2023-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2024-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 13,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                },
                {
                  "code": 37,
                  "id": 2,
                  "name": "Aston Villa",
                  "short_name": "AVL"
                }
            ]
        }
        """;
    public const string Team2023v2Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2023-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2024-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 24,
                  "id": 12,
                  "name": "Liverpool",
                  "short_name": "LIV"
                },
                {
                  "code": 53,
                  "id": 13,
                  "name": "Man City",
                  "short_name": "MCI"
                }
            ]
        }
        """;
    #endregion

    #region Element Jsons
    public const string Element2022Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 75,
                  "code": 223340,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 1,
                  "cost_change_start_fall": -1,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.5",
                  "ep_this": "4.5",
                  "event_points": 6,
                  "first_name": "Bukayo",
                  "form": "6.0",
                  "id": 13,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2023-01-13T15:00:07.612275Z",
                  "now_cost": 81,
                  "photo": "223340.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Saka",
                  "selected_by_percent": "20.5",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 2864171,
                  "transfers_in_event": 76897,
                  "transfers_out": 2891608,
                  "transfers_out_event": 55853,
                  "value_form": "0.7",
                  "value_season": "12.3",
                  "web_name": "Saka",
                  "minutes": 1534,
                  "goals_scored": 6,
                  "assists": 8,
                  "clean_sheets": 8,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 4,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 339,
                  "influence": "468.8",
                  "creativity": "567.8",
                  "threat": "517.0",
                  "ict_index": "155.4",
                  "starts": 18,
                  "expected_goals": "5.80730",
                  "expected_assists": "2.62779",
                  "expected_goal_involvements": "8.43509",
                  "expected_goals_conceded": "15.99530",
                  "influence_rank": 14,
                  "influence_rank_type": 3,
                  "creativity_rank": 7,
                  "creativity_rank_type": 5,
                  "threat_rank": 22,
                  "threat_rank_type": 13,
                  "ict_index_rank": 10,
                  "ict_index_rank_type": 6,
                  "corners_and_indirect_freekicks_order": 2,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 1,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.34072,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.15417,
                  "expected_goal_involvements_per_90": 0.49489,
                  "expected_goals_conceded_per_90": 0.93845,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 15,
                  "now_cost_rank_type": 7,
                  "form_rank": 15,
                  "form_rank_type": 7,
                  "points_per_game_rank": 14,
                  "points_per_game_rank_type": 6,
                  "selected_rank": 19,
                  "selected_rank_type": 8,
                  "starts_per_90": 1.05606,
                  "clean_sheets_per_90": 0.46936
                },
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 100,
                  "code": 206325,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 0,
                  "cost_change_start_fall": 0,
                  "dreamteam_count": 1,
                  "element_type": 2,
                  "ep_next": "3.7",
                  "ep_this": "3.2",
                  "event_points": 6,
                  "first_name": "Oleksandr",
                  "form": "3.2",
                  "id": 313,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2022-12-20T01:00:07.388064Z",
                  "now_cost": 50,
                  "photo": "206325.jpg",
                  "points_per_game": "4.5",
                  "second_name": "Zinchenko",
                  "selected_by_percent": "7.3",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 49,
                  "transfers_in": 1865110,
                  "transfers_in_event": 14505,
                  "transfers_out": 3077704,
                  "transfers_out_event": 5053,
                  "value_form": "0.6",
                  "value_season": "9.8",
                  "web_name": "Zinchenko",
                  "minutes": 811,
                  "goals_scored": 0,
                  "assists": 1,
                  "clean_sheets": 6,
                  "goals_conceded": 5,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 5,
                  "bps": 217,
                  "influence": "143.8",
                  "creativity": "118.3",
                  "threat": "84.0",
                  "ict_index": "34.6",
                  "starts": 10,
                  "expected_goals": "0.55510",
                  "expected_assists": "0.88711",
                  "expected_goal_involvements": "1.44221",
                  "expected_goals_conceded": "8.53440",
                  "influence_rank": 239,
                  "influence_rank_type": 93,
                  "creativity_rank": 165,
                  "creativity_rank_type": 37,
                  "threat_rank": 212,
                  "threat_rank_type": 44,
                  "ict_index_rank": 237,
                  "ict_index_rank_type": 74,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.0616,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.09845,
                  "expected_goal_involvements_per_90": 0.16005,
                  "expected_goals_conceded_per_90": 0.9471,
                  "goals_conceded_per_90": 0.55,
                  "now_cost_rank": 207,
                  "now_cost_rank_type": 28,
                  "form_rank": 89,
                  "form_rank_type": 29,
                  "points_per_game_rank": 38,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 47,
                  "selected_rank_type": 17,
                  "starts_per_90": 1.10974,
                  "clean_sheets_per_90": 0.66584
                }
            ]
        }
        """;
    public const string Element2022v2Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 75,
                  "code": 223340,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 1,
                  "cost_change_start_fall": -1,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.5",
                  "ep_this": "4.5",
                  "event_points": 6,
                  "first_name": "Bukayo",
                  "form": "6.0",
                  "id": 13,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2023-01-13T15:00:07.612275Z",
                  "now_cost": 81,
                  "photo": "223340.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Saka",
                  "selected_by_percent": "20.5",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 2864171,
                  "transfers_in_event": 76897,
                  "transfers_out": 2891608,
                  "transfers_out_event": 55853,
                  "value_form": "0.7",
                  "value_season": "12.3",
                  "web_name": "Saka",
                  "minutes": 1534,
                  "goals_scored": 6,
                  "assists": 8,
                  "clean_sheets": 8,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 4,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 339,
                  "influence": "468.8",
                  "creativity": "567.8",
                  "threat": "517.0",
                  "ict_index": "155.4",
                  "starts": 18,
                  "expected_goals": "5.80730",
                  "expected_assists": "2.62779",
                  "expected_goal_involvements": "8.43509",
                  "expected_goals_conceded": "15.99530",
                  "influence_rank": 14,
                  "influence_rank_type": 3,
                  "creativity_rank": 7,
                  "creativity_rank_type": 5,
                  "threat_rank": 22,
                  "threat_rank_type": 13,
                  "ict_index_rank": 10,
                  "ict_index_rank_type": 6,
                  "corners_and_indirect_freekicks_order": 2,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 1,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.34072,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.15417,
                  "expected_goal_involvements_per_90": 0.49489,
                  "expected_goals_conceded_per_90": 0.93845,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 15,
                  "now_cost_rank_type": 7,
                  "form_rank": 15,
                  "form_rank_type": 7,
                  "points_per_game_rank": 14,
                  "points_per_game_rank_type": 6,
                  "selected_rank": 19,
                  "selected_rank_type": 8,
                  "starts_per_90": 1.05606,
                  "clean_sheets_per_90": 0.46936
                },
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 100,
                  "code": 206325,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 0,
                  "cost_change_start_fall": 0,
                  "dreamteam_count": 1,
                  "element_type": 2,
                  "ep_next": "3.7",
                  "ep_this": "3.2",
                  "event_points": 6,
                  "first_name": "Oleksandr",
                  "form": "3.2",
                  "id": 313,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2022-12-20T01:00:07.388064Z",
                  "now_cost": 50,
                  "photo": "206325.jpg",
                  "points_per_game": "4.5",
                  "second_name": "Zinchenko",
                  "selected_by_percent": "7.3",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 49,
                  "transfers_in": 1865110,
                  "transfers_in_event": 14505,
                  "transfers_out": 3077704,
                  "transfers_out_event": 5053,
                  "value_form": "0.6",
                  "value_season": "9.8",
                  "web_name": "Zinchenko",
                  "minutes": 811,
                  "goals_scored": 0,
                  "assists": 1,
                  "clean_sheets": 6,
                  "goals_conceded": 5,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 5,
                  "bps": 217,
                  "influence": "143.8",
                  "creativity": "118.3",
                  "threat": "84.0",
                  "ict_index": "34.6",
                  "starts": 10,
                  "expected_goals": "0.55510",
                  "expected_assists": "0.88711",
                  "expected_goal_involvements": "1.44221",
                  "expected_goals_conceded": "8.53440",
                  "influence_rank": 239,
                  "influence_rank_type": 93,
                  "creativity_rank": 165,
                  "creativity_rank_type": 37,
                  "threat_rank": 212,
                  "threat_rank_type": 44,
                  "ict_index_rank": 237,
                  "ict_index_rank_type": 74,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.0616,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.09845,
                  "expected_goal_involvements_per_90": 0.16005,
                  "expected_goals_conceded_per_90": 0.9471,
                  "goals_conceded_per_90": 0.55,
                  "now_cost_rank": 207,
                  "now_cost_rank_type": 28,
                  "form_rank": 89,
                  "form_rank_type": 29,
                  "points_per_game_rank": 38,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 47,
                  "selected_rank_type": 17,
                  "starts_per_90": 1.10974,
                  "clean_sheets_per_90": 0.66584
                },
                {
                  "chance_of_playing_next_round": null,
                  "chance_of_playing_this_round": null,
                  "code": 444145,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 8,
                  "cost_change_start_fall": -8,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.3",
                  "ep_this": "5.8",
                  "event_points": 2,
                  "first_name": "Gabriel",
                  "form": "5.8",
                  "id": 19,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": null,
                  "now_cost": 68,
                  "photo": "444145.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Martinelli Silva",
                  "selected_by_percent": "47.0",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 5562985,
                  "transfers_in_event": 35286,
                  "transfers_out": 3037275,
                  "transfers_out_event": 114384,
                  "value_form": "0.9",
                  "value_season": "14.7",
                  "web_name": "Martinelli",
                  "minutes": 1528,
                  "goals_scored": 7,
                  "assists": 5,
                  "clean_sheets": 9,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 2,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 315,
                  "influence": "421.2",
                  "creativity": "424.0",
                  "threat": "748.0",
                  "ict_index": "159.3",
                  "starts": 18,
                  "expected_goals": "3.99210",
                  "expected_assists": "3.85744",
                  "expected_goal_involvements": "7.84954",
                  "expected_goals_conceded": "16.13830",
                  "influence_rank": 29,
                  "influence_rank_type": 13,
                  "creativity_rank": 22,
                  "creativity_rank_type": 16,
                  "threat_rank": 8,
                  "threat_rank_type": 3,
                  "ict_index_rank": 5,
                  "ict_index_rank_type": 3,
                  "corners_and_indirect_freekicks_order": 1,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": 2,
                  "direct_freekicks_text": "",
                  "penalties_order": 2,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.23514,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.22721,
                  "expected_goal_involvements_per_90": 0.46234,
                  "expected_goals_conceded_per_90": 0.95055,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 49,
                  "now_cost_rank_type": 26,
                  "form_rank": 18,
                  "form_rank_type": 10,
                  "points_per_game_rank": 13,
                  "points_per_game_rank_type": 5,
                  "selected_rank": 3,
                  "selected_rank_type": 1,
                  "starts_per_90": 1.06021,
                  "clean_sheets_per_90": 0.5301
                }
            ]
        }
        """;
    public const string Element2022v3Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 3,
                  "id": 1,
                  "name": "Arsenal",
                  "short_name": "ARS"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": 100,
                  "chance_of_playing_this_round": 75,
                  "code": 223340,
                  "cost_change_event": 0,
                  "cost_change_event_fall": 0,
                  "cost_change_start": 1,
                  "cost_change_start_fall": -1,
                  "dreamteam_count": 2,
                  "element_type": 3,
                  "ep_next": "6.5",
                  "ep_this": "4.5",
                  "event_points": 6,
                  "first_name": "Bukayo",
                  "form": "6.0",
                  "id": 13,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": "2023-01-13T15:00:07.612275Z",
                  "now_cost": 81,
                  "photo": "223340.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Saka",
                  "selected_by_percent": "20.5",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 1,
                  "team_code": 3,
                  "total_points": 100,
                  "transfers_in": 2864171,
                  "transfers_in_event": 76897,
                  "transfers_out": 2891608,
                  "transfers_out_event": 55853,
                  "value_form": "0.7",
                  "value_season": "12.3",
                  "web_name": "Saka",
                  "minutes": 1534,
                  "goals_scored": 6,
                  "assists": 8,
                  "clean_sheets": 8,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 4,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 7,
                  "bps": 339,
                  "influence": "468.8",
                  "creativity": "567.8",
                  "threat": "517.0",
                  "ict_index": "155.4",
                  "starts": 18,
                  "expected_goals": "5.80730",
                  "expected_assists": "2.62779",
                  "expected_goal_involvements": "8.43509",
                  "expected_goals_conceded": "15.99530",
                  "influence_rank": 14,
                  "influence_rank_type": 3,
                  "creativity_rank": 7,
                  "creativity_rank_type": 5,
                  "threat_rank": 22,
                  "threat_rank_type": 13,
                  "ict_index_rank": 10,
                  "ict_index_rank_type": 6,
                  "corners_and_indirect_freekicks_order": 2,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 1,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.34072,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.15417,
                  "expected_goal_involvements_per_90": 0.49489,
                  "expected_goals_conceded_per_90": 0.93845,
                  "goals_conceded_per_90": 0.82,
                  "now_cost_rank": 15,
                  "now_cost_rank_type": 7,
                  "form_rank": 15,
                  "form_rank_type": 7,
                  "points_per_game_rank": 14,
                  "points_per_game_rank_type": 6,
                  "selected_rank": 19,
                  "selected_rank_type": 8,
                  "starts_per_90": 1.05606,
                  "clean_sheets_per_90": 0.46936
                }
            ]
        }
        """;
    public const string Element2023Json = $$"""
        {
            "events": [
            {
              "id": 1,
              "name": "Gameweek 1",
              "deadline_time": "2022-08-05T17:30:00Z",
              "is_current": false
            },
            {
              "id": 38,
              "name": "Gameweek 38",
              "deadline_time": "2023-05-28T13:30:00Z",
              "is_current": false
            }],
            "teams": [
                {
                  "code": 14,
                  "id": 12,
                  "name": "Liverpool",
                  "short_name": "LIV"
                }
            ],
            "elements": [
                {
                  "chance_of_playing_next_round": null,
                  "chance_of_playing_this_round": null,
                  "code": 118748,
                  "cost_change_event": -1,
                  "cost_change_event_fall": 1,
                  "cost_change_start": -2,
                  "cost_change_start_fall": 2,
                  "dreamteam_count": 3,
                  "element_type": 3,
                  "ep_next": "5.0",
                  "ep_this": "5.5",
                  "event_points": 2,
                  "first_name": "Mohamed",
                  "form": "4.5",
                  "id": 283,
                  "in_dreamteam": false,
                  "news": "",
                  "news_added": null,
                  "now_cost": 128,
                  "photo": "118748.jpg",
                  "points_per_game": "5.6",
                  "second_name": "Salah",
                  "selected_by_percent": "29.0",
                  "special": false,
                  "squad_number": null,
                  "status": "a",
                  "team": 12,
                  "team_code": 14,
                  "total_points": 100,
                  "transfers_in": 3538849,
                  "transfers_in_event": 12537,
                  "transfers_out": 6416458,
                  "transfers_out_event": 141127,
                  "value_form": "0.4",
                  "value_season": "7.8",
                  "web_name": "Salah",
                  "minutes": 1597,
                  "goals_scored": 7,
                  "assists": 5,
                  "clean_sheets": 4,
                  "goals_conceded": 24,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 0,
                  "red_cards": 0,
                  "saves": 0,
                  "bonus": 10,
                  "bps": 234,
                  "influence": "423.6",
                  "creativity": "463.1",
                  "threat": "858.0",
                  "ict_index": "174.6",
                  "starts": 18,
                  "expected_goals": "8.86510",
                  "expected_assists": "2.99665",
                  "expected_goal_involvements": "11.86175",
                  "expected_goals_conceded": "25.12160",
                  "influence_rank": 28,
                  "influence_rank_type": 12,
                  "creativity_rank": 12,
                  "creativity_rank_type": 8,
                  "threat_rank": 5,
                  "threat_rank_type": 1,
                  "ict_index_rank": 4,
                  "ict_index_rank_type": 2,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": 2,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.4996,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.16888,
                  "expected_goal_involvements_per_90": 0.66848,
                  "expected_goals_conceded_per_90": 1.41574,
                  "goals_conceded_per_90": 1.35,
                  "now_cost_rank": 1,
                  "now_cost_rank_type": 1,
                  "form_rank": 45,
                  "form_rank_type": 21,
                  "points_per_game_rank": 16,
                  "points_per_game_rank_type": 8,
                  "selected_rank": 12,
                  "selected_rank_type": 5,
                  "starts_per_90": 1.0144,
                  "clean_sheets_per_90": 0.22542
                },
                    {
                  "chance_of_playing_next_round": 75,
                  "chance_of_playing_this_round": 50,
                  "code": 447203,
                  "cost_change_event": -1,
                  "cost_change_event_fall": 1,
                  "cost_change_start": -2,
                  "cost_change_start_fall": 2,
                  "dreamteam_count": 0,
                  "element_type": 4,
                  "ep_next": "1.9",
                  "ep_this": "1.5",
                  "event_points": 0,
                  "first_name": "Darwin",
                  "form": "2.0",
                  "id": 297,
                  "in_dreamteam": false,
                  "news": "Knock - 75% chance of playing",
                  "news_added": "2023-01-12T17:00:07.921257Z",
                  "now_cost": 88,
                  "photo": "447203.jpg",
                  "points_per_game": "4.0",
                  "second_name": "Núñez Ribeiro",
                  "selected_by_percent": "9.7",
                  "special": false,
                  "squad_number": null,
                  "status": "d",
                  "team": 12,
                  "team_code": 14,
                  "total_points": 52,
                  "transfers_in": 2904318,
                  "transfers_in_event": 3466,
                  "transfers_out": 3933135,
                  "transfers_out_event": 132020,
                  "value_form": "0.2",
                  "value_season": "5.9",
                  "web_name": "Darwin",
                  "minutes": 868,
                  "goals_scored": 5,
                  "assists": 3,
                  "clean_sheets": 1,
                  "goals_conceded": 14,
                  "own_goals": 0,
                  "penalties_saved": 0,
                  "penalties_missed": 0,
                  "yellow_cards": 1,
                  "red_cards": 1,
                  "saves": 0,
                  "bonus": 6,
                  "bps": 151,
                  "influence": "259.0",
                  "creativity": "190.2",
                  "threat": "719.0",
                  "ict_index": "115.6",
                  "starts": 10,
                  "expected_goals": "7.34030",
                  "expected_assists": "1.55296",
                  "expected_goal_involvements": "8.89326",
                  "expected_goals_conceded": "16.57190",
                  "influence_rank": 129,
                  "influence_rank_type": 12,
                  "creativity_rank": 101,
                  "creativity_rank_type": 10,
                  "threat_rank": 9,
                  "threat_rank_type": 6,
                  "ict_index_rank": 28,
                  "ict_index_rank_type": 7,
                  "corners_and_indirect_freekicks_order": null,
                  "corners_and_indirect_freekicks_text": "",
                  "direct_freekicks_order": null,
                  "direct_freekicks_text": "",
                  "penalties_order": null,
                  "penalties_text": "",
                  "expected_goals_per_90": 0.76109,
                  "saves_per_90": 0.0,
                  "expected_assists_per_90": 0.16102,
                  "expected_goal_involvements_per_90": 0.92211,
                  "expected_goals_conceded_per_90": 1.71828,
                  "goals_conceded_per_90": 1.45,
                  "now_cost_rank": 11,
                  "now_cost_rank_type": 6,
                  "form_rank": 170,
                  "form_rank_type": 20,
                  "points_per_game_rank": 69,
                  "points_per_game_rank_type": 11,
                  "selected_rank": 38,
                  "selected_rank_type": 7,
                  "starts_per_90": 1.03687,
                  "clean_sheets_per_90": 0.10369
                }
            ]
        }
        """;
    #endregion

    [TestMethod]
    public async Task RealDataTest()
    {
        var json = EmbeddedResourceHelper.GetResourceFromJson("bootstrap-static");
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = json });

        var model = JsonSerializer.Deserialize<BootstrapStatic>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        var seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(20, teams.Length);

        // grab a few random teams to check
        var rand = new Random();
        for (var i = 0; i < 5; i++)
        {
            var team = model.Teams[rand.Next(model.Teams.Length)];
            var dbTeam = teams.FirstOrDefault(x => x.TeamId == team.Id);
            Assert.IsNotNull(dbTeam);

            AssertTeam(dbTeam, team, seasons[0].Id);
        }

        var elements = await UnitOfWork.Elements.GetAll();

        // grab a few random elements to check
        for (var i = 0; i < 100; i++)
        {
            var element = model.Elements[rand.Next(model.Elements.Length)];
            var dbElement = elements.FirstOrDefault(x => x.ElementId == element.Id);
            Assert.IsNotNull(dbElement);

            var dbTeam = teams.FirstOrDefault(x => x.Id == dbElement.TeamId);
            Assert.IsNotNull(dbTeam);

            AssertElement(dbElement, element, dbTeam.Id);
        }
    }

    [TestMethod]
    public async Task SeasonSameTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Season2022Json });

        var seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Season2022Json });

        seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);
    }

    [TestMethod]
    public async Task SeasonNewTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Season2022Json });

        var seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(1, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);

        // call with different json
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Season2023Json });

        seasons = await UnitOfWork.Seasons.GetAll();
        Assert.AreEqual(2, seasons.Length);
        Assert.AreEqual("2022/23", seasons[0].Year);
        Assert.AreEqual("2023/24", seasons[1].Year);
    }

    [TestMethod]
    public async Task TeamSameSeasonSameTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2022Json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual("Aston Villa", teams[1].Name);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2022Json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual("Aston Villa", teams[1].Name);
    }

    [TestMethod]
    public async Task TeamDifferentSeasonSameTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2022Json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual("Aston Villa", teams[1].Name);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2022v2Json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Liverpool", teams[0].Name);
        Assert.AreEqual("Man City", teams[1].Name);
    }

    [TestMethod]
    public async Task TeamSameSeasonNewTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2022Json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual("Aston Villa", teams[1].Name);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2023Json });

        var seasons = await UnitOfWork.Seasons.GetAll();
        teams = await UnitOfWork.Teams.GetAll();

        Assert.AreEqual(2, seasons.Length);
        Assert.AreEqual(4, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(seasons[0].Id, teams[0].SeasonId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(seasons[0].Id, teams[1].SeasonId);
        Assert.AreEqual("Arsenal", teams[2].Name);
        Assert.AreEqual(seasons[1].Id, teams[2].SeasonId);
        Assert.AreEqual("Aston Villa", teams[3].Name);
        Assert.AreEqual(seasons[1].Id, teams[3].SeasonId);
    }

    [TestMethod]
    public async Task TeamDifferentSeasonDifferentTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2022Json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(2, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual("Aston Villa", teams[1].Name);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Team2023v2Json });

        var seasons = await UnitOfWork.Seasons.GetAll();
        teams = await UnitOfWork.Teams.GetAll();

        Assert.AreEqual(2, seasons.Length);
        Assert.AreEqual(4, teams.Length);
        Assert.AreEqual("Arsenal", teams[0].Name);
        Assert.AreEqual(seasons[0].Id, teams[0].SeasonId);
        Assert.AreEqual("Aston Villa", teams[1].Name);
        Assert.AreEqual(seasons[0].Id, teams[1].SeasonId);
        Assert.AreEqual("Liverpool", teams[2].Name);
        Assert.AreEqual(seasons[1].Id, teams[2].SeasonId);
        Assert.AreEqual("Man City", teams[3].Name);
        Assert.AreEqual(seasons[1].Id, teams[3].SeasonId);
    }

    [TestMethod]
    public async Task ElementSameTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Element2022Json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        var elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(3, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(2, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Element2022Json });

        elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(3, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(2, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);
    }

    [TestMethod]
    public async Task ElementNewTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Element2022Json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        var elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(3, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(2, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Element2022v2Json });

        teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(3, elements.Length);

        Assert.AreEqual(teams[0].Id, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(1, elements[0].ElementTeamId);
        Assert.AreEqual(3, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(teams[0].Id, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(1, elements[1].ElementTeamId);
        Assert.AreEqual(2, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        Assert.AreEqual(teams[0].Id, elements[2].TeamId);
        Assert.AreEqual(19, elements[2].ElementId);
        Assert.AreEqual(1, elements[2].ElementTeamId);
        Assert.AreEqual(3, elements[2].ElementType);
        Assert.AreEqual("Gabriel", elements[2].FirstName);
        Assert.AreEqual("Martinelli Silva", elements[2].SecondName);
        Assert.AreEqual("Martinelli", elements[2].WebName);

        //elements = await UnitOfWork.Elements.GetAllByTeamId(teams[1].Id);
        //Assert.AreEqual(12, elements[0].TeamId);
        //Assert.AreEqual(283, elements[0].ElementId);
        //Assert.AreEqual(teams[1].Id, elements[0].ElementTeamId);
        //Assert.AreEqual(3, elements[0].ElementType);
        //Assert.AreEqual("Mohamed", elements[0].FirstName);
        //Assert.AreEqual("Salah", elements[0].SecondName);
        //Assert.AreEqual("Salah", elements[0].WebName);

        //Assert.AreEqual(12, elements[1].TeamId);
        //Assert.AreEqual(297, elements[1].ElementId);
        //Assert.AreEqual(teams[1].Id, elements[1].ElementTeamId);
        //Assert.AreEqual(4, elements[1].ElementType);
        //Assert.AreEqual("Darwin", elements[1].FirstName);
        //Assert.AreEqual("Núñez Ribeiro", elements[1].SecondName);
        //Assert.AreEqual("Darwin", elements[1].WebName);
    }

    [TestMethod]
    public async Task ElementDifferentTest()
    {
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Element2022Json });

        var teams = await UnitOfWork.Teams.GetAll();
        Assert.AreEqual(1, teams.Length);

        var elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(2, elements.Length);
        Assert.AreEqual(1, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(teams[0].Id, elements[0].ElementTeamId);
        Assert.AreEqual(3, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);

        Assert.AreEqual(1, elements[1].TeamId);
        Assert.AreEqual(313, elements[1].ElementId);
        Assert.AreEqual(teams[0].Id, elements[1].ElementTeamId);
        Assert.AreEqual(2, elements[1].ElementType);
        Assert.AreEqual("Oleksandr", elements[1].FirstName);
        Assert.AreEqual("Zinchenko", elements[1].SecondName);
        Assert.AreEqual("Zinchenko", elements[1].WebName);

        // call again
        await ExecUpdaterFunction(new Models.MockHttpParameter() { RequestUrl = "bootstrap-static/", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = Element2022v3Json });

        elements = await UnitOfWork.Elements.GetAllByTeamId(teams[0].Id);
        Assert.AreEqual(1, elements.Length);
        Assert.AreEqual(1, elements[0].TeamId);
        Assert.AreEqual(13, elements[0].ElementId);
        Assert.AreEqual(teams[0].Id, elements[0].ElementTeamId);
        Assert.AreEqual(3, elements[0].ElementType);
        Assert.AreEqual("Bukayo", elements[0].FirstName);
        Assert.AreEqual("Saka", elements[0].SecondName);
        Assert.AreEqual("Saka", elements[0].WebName);
    }
}