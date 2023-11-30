import { Component, Input, SimpleChanges } from '@angular/core';
import { HttpService } from 'src/app/services/http.service';

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

  loading: boolean = true;

  constructor(private httpService: HttpService) {

  }

  ngOnChanges(changes: SimpleChanges): void {
    this.getChart();    
  }

  getChart() {
    this.httpService.getChart(this.type, this.chartId, this.seasonId, this.leagueId).subscribe({ 
      next: (values: any) =>
        {
          if (!values.length)
            return;

        },
      complete: () => { this.loading = false; }});    
  }
}
