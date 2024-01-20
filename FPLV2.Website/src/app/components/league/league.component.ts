import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpService } from 'src/app/services/http.service';

@Component({
  selector: 'app-league',
  templateUrl: './league.component.html',
  styleUrls: ['./league.component.scss']
})
export class LeagueComponent {
  loading: boolean = true;
  error: boolean = false;
  
  seasonId: number = 0;
  leagueId: number = 0;
  name: string = '';
  stats: any[] = [];
  selectedStatType: any;
  selectedStat: any;
  players: any[] = [];
  selectedPlayer: any;
  teams: any[] = [];
  charts: any[] = [];
  selectedChart: any;

  constructor(private route: ActivatedRoute, private httpService: HttpService, private router: Router) {
    // Access route parameters
    this.route.paramMap.subscribe(params => {
      var seasonIdParam = params.get('seasonId');
      if (seasonIdParam)
        this.seasonId = +seasonIdParam;

      var leagueIdParam = params.get('leagueId');
      if (leagueIdParam)
        this.leagueId = +leagueIdParam;

        this.httpService.getLeagueSummary(this.seasonId, this.leagueId).subscribe({next: (values: any) => 
          {
              this.name = values.name;
              this.players = values.players;
              this.selectedPlayer = this.players[0];
              this.teams = values.teams;
          }});

        this.httpService.getStats().subscribe({next: (values: any) => 
          {
              this.stats = values;
              this.selectedStatType = 1;
              this.selectedStat = this.stats.find(x => x.type == this.selectedStatType);
          }});

        this.httpService.getCharts().subscribe({next: (values: any) => 
          {
              this.charts = values;
              this.selectedChart = this.charts[0];
          }});
    });
  }

  changeType() {
    this.selectedStat = this.stats.find(x => x.type == this.selectedStatType);
  }

  expandStats() {

  }

  expandChart() {
    if (!this.selectedChart)
      return;

    this.router.navigate([`/season/${this.seasonId}/league/${this.leagueId}/chart/${this.selectedChart.type}/${this.selectedChart.id}`]);
  }

  expandPicks() {
    this.router.navigate([`/season/${this.seasonId}/league/${this.leagueId}/picks`], {
      state: { players: this.players, teams: this.teams }
    });
  }
}
