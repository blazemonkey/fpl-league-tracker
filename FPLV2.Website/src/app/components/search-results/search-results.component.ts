import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DomLayoutType, RowSelectedEvent } from 'ag-grid-community';
import { HttpService } from 'src/app/services/http.service';

@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.scss']
})
export class SearchResultsComponent implements OnInit {
  name: string = '';
  loading: boolean = true;
  error: boolean = false;

  rowSelection: 'single' | 'multiple' = 'single';
  domLayout: DomLayoutType = 'autoHeight';
  rows: any[] = [];
  columns: any[] = [ 
    { headerName: '', field: 'leagueId', width: 100, cellStyle: { 'color': 'var(--main-color)', 'font-weight': 'bold' } },
    { headerName: '', field: 'adminPlayerName', flex: 1 },
    { headerName: '', field: 'name', flex: 2 },    
  ];

  constructor(private route: ActivatedRoute, private httpService: HttpService, private router: Router) {
    // Access query parameters
    this.route.queryParams.subscribe(params => {
      this.name = params['name'];
      this.name = decodeURI(this.name);      
    });
  }

  ngOnInit() {
    this.httpService.searchLeagues(this.name).subscribe({ 
      next: values => 
        { 
          if (values.length == 1) 
          {
            var league = values[0];
            this.router.navigate([`/season/${league.seasonId}/league/${league.leagueId}`], { replaceUrl: true });
          }
          else
            this.rows = values; 

          this.loading = false;
        },
      error: () => { this.loading = false; this.error = true; }});    
    }

  onRowSelected(event: RowSelectedEvent) {
    this.router.navigate([`/season/${event.data.seasonId}/league/${event.data.leagueId}`]);
  }
}
