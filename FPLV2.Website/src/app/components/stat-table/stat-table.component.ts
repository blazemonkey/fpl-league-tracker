import { Component, Input, SimpleChanges } from '@angular/core';
import { HttpService } from 'src/app/services/http.service';

@Component({
  selector: 'app-stat-table',
  templateUrl: './stat-table.component.html',
  styleUrls: ['./stat-table.component.scss']
})

export class StatTableComponent {
  @Input() type: number = 0;
  @Input() statId: number = 0;
  @Input() seasonId: number = 0;
  @Input() leagueId: number = 0;
  @Input() playerId: number = 0;

  gridOptions: any;
  rowSelection: 'single' | 'multiple' = 'single';
  columns: any[] = [];
  rows: any[] = [];
  loading: boolean = true;

  constructor(private httpService: HttpService) {
    this.gridOptions = {
      pagination: true,
      paginationPageSize: 10,
      domLayout: 'autoHeight',
      suppressRowClickSelection: true,
      suppressCellFocus: true,
      rowData: this.rows,
      columnDefs: this.columns
    };
  }

  onGridReady(params: any): void {    
    this.gridOptions.api = params.api;
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.getStat();    
  }

  getStat() {
    this.httpService.getStat(this.type, this.statId, this.seasonId, this.leagueId, this.playerId).subscribe({ 
      next: (values: any) =>
        {
          if (!values?.length)
            return;

          this.columns = [];
          var columns = Object.keys(values[0]);
          for (var i = 0; i < columns.length; i++)
          {
            var c = columns[i];
            var gameweekWidth = 110;
            this.columns.push({ headerName: c, field: c, flex: i == 0 ? 2 : 1 })
          }
          
          this.gridOptions.api.setColumnDefs(this.columns);
          this.rows = values;
          this.gridOptions.api.setRowData(this.rows);
        },
      complete: () => { this.loading = false; }});    
  }
}
