import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpService } from 'src/app/services/http.service';

@Component({
  selector: 'app-league',
  templateUrl: './league.component.html',
  styleUrls: ['./league.component.scss']
})
export class LeagueComponent {
  seasonId: number = 0;
  leagueId: number = 0;
  name: string = '';
  stats: any[] = [];
  selectedStat: any;
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
          }});

        this.httpService.getStats().subscribe({next: (values: any) => 
          {
              this.stats = values;
              this.selectedStat = this.stats[0];
          }});

        this.httpService.getCharts().subscribe({next: (values: any) => 
          {
              this.charts = values;
              this.selectedChart = this.charts[0];
          }});
    });
  }
}
