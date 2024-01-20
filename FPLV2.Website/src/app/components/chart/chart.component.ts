import { Component, Input, SimpleChanges } from '@angular/core';
import { HttpService } from 'src/app/services/http.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})

export class ChartComponent {
  @Input() type: number = 0;
  @Input() chartId: number = 0;
  @Input() seasonId: number = 0;
  @Input() leagueId: number = 0;

  chartData: any[] = [];
  xAxisTicks: any[] = [];
  loading: boolean = true;
  fullScreen: boolean = false;

  constructor(private route: ActivatedRoute, private httpService: HttpService) {
    this.route.paramMap.subscribe(params => {
      var seasonIdParam = params.get('seasonId');
      if (seasonIdParam)
        this.seasonId = +seasonIdParam;

      var leagueIdParam = params.get('leagueId');
      if (leagueIdParam)
        this.leagueId = +leagueIdParam;

      var typeParam = params.get('type');
      if (typeParam)
        this.type = +typeParam;     
      
      var chartIdParam = params.get('chartId');
      if (chartIdParam)
        this.chartId = +chartIdParam;

      // if it's called via Expand, then there will be values in all these parameters
      // otherwise it'll be called by the parent component, and we don't need to call getChart because thats handled in onChanges
      if (this.seasonId > 0 && this.leagueId > 0 && this.type > 0 && this.chartId > 0) {
        this.fullScreen = true;
        this.getChart();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.getChart();    
  }

  getChart() {
    this.httpService.getChart(this.type, this.chartId, this.seasonId, this.leagueId).subscribe({ 
      next: (values: any) =>
        {
          var data = [];
          var names = Object.keys(values);          
          for (var i = 0; i < names.length; i++) {
            var series = values[names[i]].map((series: { x: any; y: any; }) => ({ name: series.x, value: series.y }));            
            data.push({ name: names[i], series: series });

            // set the predefined ticks only if full screen mode
            if (i == 0)
              this.xAxisTicks = values[names[i]].map((series: { x: any; }) => series.x);
          }

          this.chartData = data;
        },
      complete: () => { this.loading = false; }});    
  }
}
